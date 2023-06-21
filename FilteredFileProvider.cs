using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Primitives;

public class FilteredFileProvider : IFileProvider
{
    Dictionary<string, string[]?> _directoryFiletypePairs;

    public FilteredFileProvider(Dictionary<string, string[]?> directoryFiletypePairs)
    {
        _directoryFiletypePairs = directoryFiletypePairs;
    }

    public IFileInfo GetFileInfo(string subpath)
    {
        foreach (string path in _directoryFiletypePairs.Keys)
        {
            string absolutePath = Directory.GetCurrentDirectory() + path + subpath;
            if (File.Exists(absolutePath))
            {
                string[] serveExtensions = _directoryFiletypePairs[path] ?? new string[] { };

                if (serveExtensions.Any(extensions => absolutePath.EndsWith(extensions)))
                {
                    // Console.WriteLine($"The \"{path}\" directory is serving \"{subpath.Substring(1)}\" on \"/\".");
                    return new PhysicalFileInfo(new FileInfo(absolutePath));
                }
                else
                {
                    // Console.WriteLine($"The \"{path}\" path can't serve \"{subpath.Substring(1)}\" because it doesn't end with any of the following extensions: { string.Join(", ", serveExtensions)}");
                    return new NotFoundFileInfo(subpath);
                }
            }
        }
        return new NotFoundFileInfo(subpath);
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