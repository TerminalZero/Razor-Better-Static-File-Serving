/// <summary>
/// Fluent API for configuring static file serving using <see cref="FilteredFileProvider"/>.<br/>
/// For use with: <see cref="IApplicationBuilder.UseStaticFiles(StaticFileOptions)"/>.
/// </summary>
public class FileExtensionDirectoryMappings : StaticFileOptions
{
    Dictionary<string, string[]?> directoryFiletypePairs = new();
    int index = 0;

    public FileExtensionDirectoryMappings()
    {
        FileProvider = new FilteredFileProvider(directoryFiletypePairs);
    }

    /// <summary>
    /// Adds a directory to serve files from.<br/>
    /// Call ServingFileTypes after this method.
    /// </summary>
    public FileExtensionDirectoryMappings AddDirectory(string directory)
    {
        directoryFiletypePairs.Add(directory, null);
        index++;

        return this;
    }

    /// <summary>
    /// Leaving the <paramref name="fileTypes"/> parameter empty will serve all files in the directory.<br/>
    /// Call AddDirectory after this method.
    /// </summary>
    public FileExtensionDirectoryMappings ServingFileTypes(params string[] fileTypes)
    {
        fileTypes = fileTypes.Length > 0 ? fileTypes : new string[] {""};

        string? emptyKey = directoryFiletypePairs.ElementAtOrDefault(index - 1).Key;

        if (emptyKey == null)
        {
            throw new Exception("You must call AddDirectory before calling ServingFileTypes.");
        }

        directoryFiletypePairs[emptyKey] = fileTypes;

        return this;
    }
}
