namespace FC.Codeflix.Catalog.EndToEndTests.Extensions;

internal static class StreamExtensions
{
    public static string ToContentString(this Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}