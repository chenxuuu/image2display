using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Image2Display.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.Helpers
{
    public class ImageHelper
    {
        public static Bitmap LoadFromResource(Uri resourceUri)
        {
            return new Bitmap(AssetLoader.Open(resourceUri));
        }

        public static async Task<Bitmap?> LoadFromWeb(Uri url)
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var data = await response.Content.ReadAsByteArrayAsync();
                return new Bitmap(new MemoryStream(data));
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
                return null;
            }
        }

        public static Bitmap LoadFromImageData(ImageData image)
        {
            return new Bitmap(image.GetStream());
        }
    }
}
