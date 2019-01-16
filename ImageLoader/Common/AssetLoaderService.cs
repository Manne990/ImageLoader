using System.Net.Http;
using System.Threading.Tasks;

namespace ImageLoader.Common
{
    public class AssetLoaderService : IAssetLoaderService
    {
        // Public Methods
        public async Task<byte[]> DownloadAsset(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    return await httpClient.GetByteArrayAsync(url);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}