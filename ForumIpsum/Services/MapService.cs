using System;
using Microsoft.AspNetCore.StaticFiles;

namespace ForumIpsum.Services
{
    public class MapService
    {
        public string GetContentType(string extension)
        {
            var provider = new FileExtensionContentTypeProvider();
            return provider.TryGetContentType($"file.{extension}", out var result) ? result : "application/unknown";
        }
    }
}
