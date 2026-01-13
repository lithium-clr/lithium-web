using System.ComponentModel.DataAnnotations;

namespace Lithium.Web.Pages.Docs;

public sealed record DocModel
{
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Slug is required.")]
    [RegularExpression(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "Slug must be in kebab-case (e.g., 'my-first-doc').")]
    public string Slug { get; set; } = null!;

    [Required(ErrorMessage = "Category is required.")]
    public string Category { get; set; } = "General";

    public string Icon { get; set; } = "📄";
    public string Content { get; set; } = "";
}