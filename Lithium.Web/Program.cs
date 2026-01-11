using Lithium.Web;

var builder = WebApplication.CreateBuilder(args);
builder.SetupBootstrap();

var app = builder.Build();
app.SetupBootstrap();
app.Run();