using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddLogging(_ => _.ClearProviders().AddConsole());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new CompositeFileProvider(
        new PhysicalFileProvider("/wwwroot"),
        new PhysicalFileProvider("/Pages")
    )
});

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();