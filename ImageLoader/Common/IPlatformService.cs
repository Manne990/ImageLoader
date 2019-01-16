using System.Threading.Tasks;

namespace ImageLoader.Common
{
    public interface IPlatformService
    {
        Task<byte[]> LoadBinaryFile(string fileName, bool fullPath = false);
        Task SaveBinaryFile(string fileName, byte[] data, bool flagAsNotBackup = true);
    }
}