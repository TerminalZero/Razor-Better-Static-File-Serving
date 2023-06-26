using TZeroSolutions.AspNetCore.ApplicationBuilderExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddLogging(_ => _.ClearProviders().AddConsole());
builder.Services.AddSingleton<FilteredDirectoryService>();

builder.WebHost.UseStaticWebAssets();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseFilteredDirectory("/wwwroot")
    .ServingFileTypes(); // serve all files in wwwroot
app.UseFilteredDirectory("/Pages")
    .ServingFileTypes(".css",".js"); // serve only css and js files in Pages

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();