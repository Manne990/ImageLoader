using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ImageLoader.Common
{
    public class ImageLoader : IImageLoader
    {
        // Private Members
        private readonly IAssetLoaderService _assetLoader;
        private readonly ILoggerService _loggerService;
        private readonly IPlatformService _platformService;
        private nfloat _screenScale;


        // -----------------------------------------------------------------------------

        // Constructors
        public ImageLoader()
        {
            _assetLoader = new AssetLoaderService();
            _loggerService = new LoggerService();
            _platformService = new PlatformService();

            _screenScale = UIScreen.MainScreen.Scale;
        }


        // -----------------------------------------------------------------------------

        // Static Methods
        public static UIImage SafeLoadImage(string filename)
        {
            return UIImage.FromFile(filename) ?? UIImage.FromBundle("defaultbg.jpg");
        }


        // -----------------------------------------------------------------------------

        // Public Methods
        public async Task<UIImage> LoadImage(string url, ImageScaleTypes scaleType = ImageScaleTypes.ScaleToFit, bool invalidateCache = false)
        {
            return await LoadImageTask(url, -1, -1, scaleType, null, invalidateCache);
        }

        public void LoadImageAsync(string url, Action<UIImage> loadedListener, ImageScaleTypes scaleType = ImageScaleTypes.ScaleToFit, bool invalidateCache = false)
        {
            Task.Run(async () => await LoadImageTask(url, -1, -1, scaleType, loadedListener, invalidateCache));
        }

        public void LoadImageAsync(string url, int width, int height, Action<UIImage> loadedListener, ImageScaleTypes scaleType = ImageScaleTypes.Crop, bool invalidateCache = false)
        {
            Task.Run(async () => await LoadImageTask(url, width, height, scaleType, loadedListener, invalidateCache));
        }

        public void ScaleImageFromFileAsync(string filename, int width, int height, Action<UIImage> loadedListener, ImageScaleTypes scaleType = ImageScaleTypes.ScaleToFit)
        {
            Task.Run(async () => await ScaleImageFromFileTask(filename, width, height, loadedListener, scaleType));
        }


        // -----------------------------------------------------------------------------

        // Private Methods
        private async Task<UIImage> LoadImageTask(string url, int width, int height, ImageScaleTypes scaleType = ImageScaleTypes.Crop, Action<UIImage> loadedListener = null, bool invalidateCache = false)
        {
            var shouldScale = width > 0 && height > 0;
            var shouldCache = true;

            try
            {
                // Init
                _loggerService?.Trace($"Load Image '{url}', {width}, {height}, {scaleType}");

                // Construct the filename
                var filename = $"{Base32.ToBase32String($"{url}|{width}|{height}")}.png";

                if (shouldScale && invalidateCache == false)
                {
                    // Try to get a cached image
                    var cachedImageBytes = await _platformService.LoadBinaryFile(filename);
                    if (cachedImageBytes != null)
                    {
                        var cachedImage = ImageFromBytes(cachedImageBytes);

                        UIApplication.SharedApplication.InvokeOnMainThread(() => loadedListener?.Invoke(cachedImage));
                        return cachedImage;
                    }
                }

                // Load the image from the API
                var imageBytes = await _assetLoader.DownloadAsset(url);
                if (imageBytes == null)
                {
                    imageBytes = await _platformService.LoadBinaryFile("placeholder.png", true);
                }

                if (imageBytes == null)
                {
                    _loggerService?.LogError($"Error for file '{url}', Image could not be loaded!");
                    UIApplication.SharedApplication.InvokeOnMainThread(() => loadedListener?.Invoke(null));
                    return null;
                }

                var image = ImageFromBytes(imageBytes);
                if (image == null)
                {
                    _loggerService?.LogError($"Error for file '{url}', Image could not be parsed!");
                    UIApplication.SharedApplication.InvokeOnMainThread(() => loadedListener?.Invoke(null));
                    return null;
                }

                if (shouldScale)
                {
                    switch (scaleType)
                    {
                        case ImageScaleTypes.Crop:
                            image = ImageHelper.CropAndScaleImage(image, new CGSize(width, height), CropOrigoTypes.Center);
                            break;

                        case ImageScaleTypes.TopCrop:
                            image = ImageHelper.CropAndScaleImage(image, new CGSize(width, height), CropOrigoTypes.Top);
                            break;

                        case ImageScaleTypes.ScaleToFit:
                            image = ImageHelper.ScaleImage(image, new CGSize(width, height));
                            break;
                    }

                    if (shouldCache)
                    {
                        // Save the cached image
                        var bytes = ImageToBytes(image, "png");
                        if (bytes == null)
                        {
                            UIApplication.SharedApplication.InvokeOnMainThread(() => loadedListener?.Invoke(image));
                            return image;
                        }

                        await _platformService.SaveBinaryFile(filename, bytes);
                    }
                }

                // Return and Notify
                UIApplication.SharedApplication.InvokeOnMainThread(() => loadedListener?.Invoke(image));
                return image;
            }
            catch (Exception ex)
            {
                _loggerService?.LogError($"Error for file '{url}', {ex.Message}");
                _loggerService?.LogError(ex);
            }

            UIApplication.SharedApplication.InvokeOnMainThread(() => loadedListener?.Invoke(null));
            return null;
        }

        private async Task ScaleImageFromFileTask(string filename, int width, int height, Action<UIImage> loadedListener, ImageScaleTypes scaleType)
        {
            try
            {
                // Get the image bytes
                var imageBytes = await _platformService.LoadBinaryFile(filename, true);
                if (imageBytes == null)
                {
                    _loggerService?.LogError($"Error for file '{filename}', Image could not be found!");
                    return;
                }

                // Convert bytes to UIImage
                var image = ImageFromBytes(imageBytes);
                if (image == null)
                {
                    _loggerService?.LogError($"Error for file '{filename}', Image could not be parsed!");
                    return;
                }

                // Scale/Crop
                switch (scaleType)
                {
                    case ImageScaleTypes.Crop:
                        image = ImageHelper.CropAndScaleImage(image, new CGSize(width, height), CropOrigoTypes.Center);
                        break;

                    case ImageScaleTypes.TopCrop:
                        image = ImageHelper.CropAndScaleImage(image, new CGSize(width, height), CropOrigoTypes.Top);
                        break;

                    case ImageScaleTypes.ScaleToFit:
                        image = ImageHelper.ScaleImage(image, new CGSize(width, height));
                        break;
                }

                // Notify...
                loadedListener?.Invoke(image);
            }
            catch (Exception ex)
            {
                _loggerService?.LogError($"Error for file '{filename}', {ex.Message}");
                _loggerService?.LogError(ex);
            }

            return;
        }

        private static UIImage ImageFromBytes(byte[] imageBytes)
        {
            using (var data = NSData.FromArray(imageBytes))
            {
                return UIImage.LoadFromData(data);
            }
        }

        private static byte[] ImageToBytes(UIImage image, string fileType)
        {
            if (image == null)
            {
                return null;
            }

            return fileType == "png" ? PngImageToBytes(image) : JpegImageToBytes(image);
        }

        private static byte[] PngImageToBytes(UIImage image)
        {
            using (var imageData = image.AsPNG())
            {
                var myByteArray = new byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, myByteArray, 0, Convert.ToInt32(imageData.Length));

                return myByteArray;
            }
        }

        private static byte[] JpegImageToBytes(UIImage image)
        {
            using (var imageData = image.AsJPEG())
            {
                var myByteArray = new byte[imageData.Length];
                System.Runtime.InteropServices.Marshal.Copy(imageData.Bytes, myByteArray, 0, Convert.ToInt32(imageData.Length));

                return myByteArray;
            }
        }
    }
}