using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;

namespace ImageLoader.Common
{
    public class PlatformService : IPlatformService
    {
        public async Task<byte[]> LoadBinaryFile(string fileName, bool fullPath = false)
        {
            try
            {
                var filePath = fileName;

                if (fullPath == false)
                {
                    filePath = $"{GetLocalAppFolder()}/{fileName}";
                }

                if (File.Exists(filePath) == false)
                {
                    return null;
                }

                try
                {
                    File.SetLastAccessTime(filePath, DateTime.Now);
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine($"Could not set last accessed time for file '{fileName}'");
                }

                byte[] result;
                const int BufferSize = 4096;

                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, BufferSize, true))
                {
                    result = new byte[stream.Length];
                    await stream.ReadAsync(result, 0, result.Length);
                }

                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }

        public async Task SaveBinaryFile(string fileName, byte[] data, bool flagAsNotBackup = true)
        {
            try
            {
                var filePath = $"{GetLocalAppFolder()}/{fileName}";

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await stream.WriteAsync(data, 0, data.Length);
                    stream.Close();
                }

                File.SetLastAccessTime(filePath, DateTime.Now);

                if (flagAsNotBackup)
                {
                    NSFileManager.SetSkipBackupAttribute(filePath, true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private string GetLocalAppFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }
    }
}