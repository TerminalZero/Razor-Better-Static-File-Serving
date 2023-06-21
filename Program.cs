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

app.UseStaticFiles(new FileExtensionDirectoryMappings()
    .AddDirectory("/wwwroot").ServingFileTypes()
    .AddDirectory("/Pages").ServingFileTypes(".css", ".js")
);

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();