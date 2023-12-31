namespace TZeroSolutions.AspNetCore.ApplicationBuilderExtensions;

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

public static class ApplicationBuilderExtensions
{
    public static FilteredDirectory UseFilteredDirectory(this IApplicationBuilder app, params string[] directories)
    {
        return new FilteredDirectory(app, directories);
    }
}

public class FilteredDirectoryService
{
    public string[] Directories { get; set; } = { };
    public List<string> Files { get; set; } = new();

    public bool FileExists(string requestedPath)
    {
        if (Files.Contains(requestedPath))
            return true;

        string? requestedFile = Directories
            .Select(directory => Path.Combine(Directory.GetCurrentDirectory(), directory) + requestedPath)
            .Where(File.Exists)
            .FirstOrDefault();

        if (requestedFile != null)
        {
            Files.Add(requestedFile.Substring(requestedFile.LastIndexOf('/')));
            return true;
        }
        
        return false;
    }
}

public class FilteredDirectory
{
    IApplicationBuilder _app;
    string[] _directories;
    string[] _fileTypes = { };

    public FilteredDirectory(IApplicationBuilder app, string[] directories)
    {
        if (app.ApplicationServices.GetService<FilteredDirectoryService>() == null)
            throw new InvalidOperationException("You must add FilteredDirectoryService to the service collection before calling UseFilteredDirectory");

        _app = app;
        _directories = directories
            .Select(directory => directory.Substring(directory.IndexOf('/') == 0 ? 1 : 0))
            .ToArray();

        _app.ApplicationServices.GetRequiredService<FilteredDirectoryService>().Directories = _directories;
    }

    public IApplicationBuilder ServingFileTypes(params string[] fileTypes)
    {
        _fileTypes = fileTypes;

        _app.UseStaticFiles(new StaticFileOptions()
        {
            FileProvider = new FilteredFileProvider(new() { { _directories, _fileTypes } }),
            ServeUnknownFileTypes = true
        });

        return _app;
    }
}

public class FilteredFileProvider : IFileProvider
{
    IDictionary<string[], string[]> _directoryFiletypePairs;

    public FilteredFileProvider(Dictionary<string[], string[]> directoryFiletypePairs)
    {
        _directoryFiletypePairs = directoryFiletypePairs;
    }

    public IFileInfo GetFileInfo(string requestedPath)
    {
        List<string> allFiles = _directoryFiletypePairs.Keys
            .SelectMany(directories => directories
                .Select(directory => Path.Combine(Directory.GetCurrentDirectory(), directory) + requestedPath))
            .Where(File.Exists)
            .ToList();

        foreach (string file in allFiles)
        {
            if (_directoryFiletypePairs.Values.Any(fileTypes => fileTypes.Length == 0))
                return new PhysicalFileInfo(new FileInfo(file));

            string fileExt = Path.GetExtension(file);

            if (_directoryFiletypePairs.Values.Any(fileTypes => fileTypes.Contains(fileExt)))
                return new PhysicalFileInfo(new FileInfo(file));
        }

        return new NotFoundFileInfo(requestedPath);
    }

    public IDirectoryContents GetDirectoryContents(string subpath)
    {
        throw new NotImplementedException();
    }

    public IChangeToken Watch(string filter)
    {
        throw new NotImplementedException();
    }
}