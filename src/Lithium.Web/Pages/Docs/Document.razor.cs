using System.Text.RegularExpressions;
using Markdig;
using Markdig.Syntax;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Lithium.Web.Pages.Docs;

public partial class Document : ComponentBase
{
    private class Heading
    {
        public string Text { get; init; } = null!;
        public string Slug { get; init; } = null!;
        public int Level { get; init; }
    }

    private readonly List<DocModel> _docs = [];
    private string _htmlContent = null!;
    private List<Heading> _headings = [];
    private string? _currentSlug;
    private bool _isNavOpen;
    private bool _isTocOpen;
    private bool _isSearchModalOpen;
    private bool _isEditMode;
    private bool _isMetadataModalOpen;
    private readonly DocModel _doc = new();
    private DocModel _originalDoc = new();
    private string _docsPath = null!;
    private string _originalSlug = null!;

    [Parameter] public string? Slug { get; set; }

    [JSInvokable]
    public void OpenSearchModal()
    {
        _isSearchModalOpen = true;
        StateHasChanged();
    }

    private void ToggleNav()
    {
        _isNavOpen = !_isNavOpen;
        if (_isNavOpen) _isTocOpen = false;
    }

    private void ToggleToc()
    {
        _isTocOpen = !_isTocOpen;
        if (_isTocOpen) _isNavOpen = false;
    }

    private void CloseSidebars()
    {
        _isNavOpen = false;
        _isTocOpen = false;
    }

    protected override void OnInitialized()
    {
        _docsPath = Path.Combine(Env.WebRootPath, "docs");

        if (!Directory.Exists(_docsPath))
            Directory.CreateDirectory(_docsPath);

        var files = Directory.GetFiles(_docsPath, "*.md", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var markdown = File.ReadAllText(file);
            var meta = ParseYamlFrontMatter(markdown);
            var slug = Path.GetFileNameWithoutExtension(file);

            _docs.Add(new DocModel
            {
                Slug = slug,
                Title = meta.GetValueOrDefault("title", slug),
                Icon = meta.GetValueOrDefault("icon", "📄"),
                Category = meta.GetValueOrDefault("category", "General"),
                Content = MyRegex1().Replace(markdown, "").Trim()
            });
        }

        if (!string.IsNullOrEmpty(Slug)) return;
        
        var firstDoc = _docs.FirstOrDefault();

        if (firstDoc is not null)
            NavigationManager.NavigateTo($"/docs/{firstDoc.Slug}");
    }

    protected override Task OnParametersSetAsync()
    {
        if (Slug == _currentSlug)
            return Task.CompletedTask;

        _currentSlug = Slug;

        if (string.IsNullOrEmpty(Slug))
        {
            var firstDoc = _docs.FirstOrDefault();

            if (firstDoc is not null)
                NavigationManager.NavigateTo($"/docs/{firstDoc.Slug}");

            return Task.CompletedTask;
        }

        var doc = _docs.FirstOrDefault(d => d.Slug == Slug);

        if (doc is not null)
        {
            _doc.Title = doc.Title;
            _doc.Slug = doc.Slug;
            _doc.Category = doc.Category;
            _doc.Icon = doc.Icon;
            _doc.Content = doc.Content;
            _originalSlug = Slug;

            RenderMarkdown();
        }
        else
        {
            _htmlContent =
                "<div class='flex flex-col items-center justify-center h-64 text-center'><h2 class='text-2xl font-bold text-white! mb-2'>Page not found</h2><p class='text-lithium-400'>The documentation page you are looking for does not exist.</p></div>";
            _headings = [];
        }

        return Task.CompletedTask;
    }

    private void RenderMarkdown()
    {
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseAbbreviations()
            .UseAutoIdentifiers()
            .UseCitations()
            .UseCustomContainers()
            .UseDefinitionLists()
            .UseEmphasisExtras()
            .UseFigures()
            .UseFooters()
            .UseFootnotes()
            .UseGridTables()
            .UseMathematics()
            .UseMediaLinks()
            .UsePipeTables()
            .UseListExtras()
            .UseTaskLists()
            .UseDiagrams()
            .UseAutoLinks()
            .UseEmojiAndSmiley()
            .UseAlertBlocks()
            .UseGenericAttributes()
            .Build();

        var document = Markdig.Markdown.Parse(_doc.Content, pipeline);

        _headings = document.Descendants<HeadingBlock>()
            .Select(h => new Heading
            {
                Text = h.Inline?.FirstChild?.ToString() ?? "Untitled",
                Slug = GenerateSlug(h.Inline?.FirstChild?.ToString() ?? ""),
                Level = h.Level
            })
            .ToList();

        _htmlContent = document.ToHtml(pipeline);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await JsRuntime.InvokeVoidAsync("lithiumDocs.init", DotNetObjectReference.Create(this));

        await JsRuntime.InvokeVoidAsync("lithiumDocs.initScrollSpy");
    }

    void IDisposable.Dispose()
    {
        JsRuntime.InvokeVoidAsync("lithiumDocs.dispose");
    }

    private void ToggleEditMode()
    {
        if (!_isEditMode)
        {
            _originalDoc = new DocModel
            {
                Title = _doc.Title,
                Slug = _doc.Slug,
                Category = _doc.Category,
                Icon = _doc.Icon,
                Content = _doc.Content
            };
        }

        _isEditMode = !_isEditMode;
    }

    private void CancelEdit()
    {
        _doc.Title = _originalDoc.Title;
        _doc.Slug = _originalDoc.Slug;
        _doc.Category = _originalDoc.Category;
        _doc.Icon = _originalDoc.Icon;
        _doc.Content = _originalDoc.Content;

        RenderMarkdown();

        _isEditMode = false;
    }

    private Task OnMarkdownValueChanged(string value)
    {
        _doc.Content = value;
        RenderMarkdown();
        return Task.CompletedTask;
    }

    private async Task SaveChanges()
    {
        var newFilePath = Path.Combine(_docsPath, $"{_doc.Slug}.md");

        if (_originalSlug != _doc.Slug)
        {
            if (File.Exists(newFilePath))
            {
                await JsRuntime.InvokeVoidAsync("alert",
                    $"A document with the slug '{_doc.Slug}' already exists. Please choose a unique slug.");
                return;
            }
        }

        if (_originalSlug != _doc.Slug)
        {
            var originalFilePath = Path.Combine(_docsPath, $"{_originalSlug}.md");

            if (File.Exists(originalFilePath))
                File.Delete(originalFilePath);
        }

        var frontMatter = $@"---
            title: ""{_doc.Title}""
            icon: ""{_doc.Icon}""
            category: ""{_doc.Category}""
            ---
            ";
        var fullContent = frontMatter + "\n" + _doc.Content;

        await File.WriteAllTextAsync(newFilePath, fullContent);

        _isEditMode = false;

        NavigationManager.NavigateTo($"/docs/{_doc.Slug}", true);
    }

    private static Dictionary<string, string> ParseYamlFrontMatter(string markdown)
    {
        var dict = new Dictionary<string, string>();

        var match = MyRegex().Match(markdown);
        if (!match.Success) return dict;

        var yaml = match.Groups[1].Value;

        foreach (var line in yaml.Split('\n'))
        {
            var parts = line.Split(':', 2);

            if (parts.Length is 2)
                dict[parts[0].Trim()] = parts[1].Trim().Trim('"');
        }

        return dict;
    }

    private static string GenerateSlug(string text)
    {
        var slug = text.ToLowerInvariant();
        slug = MyRegex2().Replace(slug, "-");
        slug = MyRegex3().Replace(slug, "");

        return slug;
    }

    [GeneratedRegex(@"^---\s*(.+?)\s*---", RegexOptions.Singleline)]
    private static partial Regex MyRegex();

    [GeneratedRegex(@"^---\s*(.+?)\s*---\s*", RegexOptions.Singleline)]
    private static partial Regex MyRegex1();
    
    [GeneratedRegex(@"\s+")]
    private static partial Regex MyRegex2();
    
    [GeneratedRegex(@"[^a-z0-9-]")]
    private static partial Regex MyRegex3();
}