using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace PhoneStore.Extensions
{
    public static class ImageHelper
    {
        private static readonly ConcurrentDictionary<string, bool> ExistenceCache = new();

        public static string ResolveImage(this IWebHostEnvironment env, string? imageUrl, string fallbackText = "No Image", int width = 400, int height = 400)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return BuildPlaceholder(fallbackText, width, height);
            }

            if (imageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || imageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
                || imageUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                return imageUrl;
            }

            var relative = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine(env.WebRootPath ?? string.Empty, relative);

            var exists = ExistenceCache.GetOrAdd(fullPath, p =>
            {
                try { return File.Exists(p); }
                catch { return false; }
            });

            return exists ? imageUrl : BuildPlaceholder(fallbackText, width, height);
        }

        private static string BuildPlaceholder(string text, int width, int height)
        {
            var safe = string.IsNullOrWhiteSpace(text) ? "No Image" : text;
            return $"https://placehold.co/{width}x{height}/EEE/513061?text={Uri.EscapeDataString(safe)}";
        }
    }
}
