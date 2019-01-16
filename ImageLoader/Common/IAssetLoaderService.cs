using System.Threading.Tasks;

namespace ImageLoader.Common
{
    public interface IAssetLoaderService
    {
        Task<byte[]> DownloadAsset(string url);
    }
}