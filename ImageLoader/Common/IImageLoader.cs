using System;
using System.Threading.Tasks;
using UIKit;

namespace ImageLoader.Common
{
    public interface IImageLoader
    {
        Task<UIImage> LoadImage(string url, ImageScaleTypes scaleType = ImageScaleTypes.ScaleToFit, bool invalidateCache = false);
        void LoadImageAsync(string url, Action<UIImage> loadedListener, ImageScaleTypes scaleType = ImageScaleTypes.ScaleToFit, bool invalidateCache = false);
        void LoadImageAsync(string url, int width, int height, Action<UIImage> loadedListener, ImageScaleTypes scaleType = ImageScaleTypes.Crop, bool invalidateCache = false);
        void ScaleImageFromFileAsync(string filename, int width, int height, Action<UIImage> loadedListener, ImageScaleTypes scaleType = ImageScaleTypes.ScaleToFit);
    }
}
