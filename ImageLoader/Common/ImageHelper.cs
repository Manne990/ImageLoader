using CoreGraphics;
using UIKit;

namespace ImageLoader.Common
{
    public static class ImageHelper
    {
        public static UIImage ScaleImage(UIImage sourceImage, CGSize targetSize)
        {
            // Check
            if (sourceImage == null)
            {
                return null;
            }

            // Init
            var thumbnailPoint = new CGPoint(0.0, 0.0);
            var scaledWidth = targetSize.Width;
            var scaledHeight = targetSize.Height;

            // Scale
            if (sourceImage.Size.Width != targetSize.Width || sourceImage.Size.Height != targetSize.Height)
            {
                var widthFactor = targetSize.Width / sourceImage.Size.Width;
                var heightFactor = targetSize.Height / sourceImage.Size.Height;
                var scaleFactor = widthFactor < heightFactor ? widthFactor : heightFactor;

                scaledWidth = sourceImage.Size.Width * scaleFactor;
                scaledHeight = sourceImage.Size.Height * scaleFactor;

                // center the image
                if (widthFactor < heightFactor)
                {
                    thumbnailPoint.Y = (targetSize.Height - scaledHeight) * 0.5f;
                }
                else
                {
                    if (widthFactor > heightFactor)
                    {
                        thumbnailPoint.X = (targetSize.Width - scaledWidth) * 0.5f;
                    }
                }
            }

            // Draw the scaled image
            UIGraphics.BeginImageContextWithOptions(targetSize, false, 0f);

            var thumbnailRect = CGRect.Empty;

            thumbnailRect.Location = thumbnailPoint;
            thumbnailRect.Size = new CGSize(scaledWidth, scaledHeight);

            sourceImage.Draw(thumbnailRect);

            var newImage = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            return newImage;
        }

        public static UIImage CropAndScaleImage(UIImage sourceImage, CGSize targetSize, CropOrigoTypes cropOrigo)
        {
            if (sourceImage == null)
            {
                return null;
            }

            // Init
            var thumbnailPoint = new CGPoint(0.0, 0.0);
            var scaledWidth = targetSize.Width;
            var scaledHeight = targetSize.Height;

            // Scale
            if (sourceImage.Size.Width != targetSize.Width || sourceImage.Size.Height != targetSize.Height)
            {
                var widthFactor = targetSize.Width / sourceImage.Size.Width;
                var heightFactor = targetSize.Height / sourceImage.Size.Height;
                var scaleFactor = widthFactor > heightFactor ? widthFactor : heightFactor;

                scaledWidth = sourceImage.Size.Width * scaleFactor;
                scaledHeight = sourceImage.Size.Height * scaleFactor;

                // center the image
                if (widthFactor > heightFactor)
                {
                    switch (cropOrigo)
                    {
                        case CropOrigoTypes.Top:
                            break;
                        case CropOrigoTypes.Center:
                            thumbnailPoint.Y = (targetSize.Height - scaledHeight) * 0.5f;
                            break;
                        case CropOrigoTypes.Bottom:

                            break;
                    }
                }
                else
                {
                    if (widthFactor < heightFactor)
                    {
                        thumbnailPoint.X = (targetSize.Width - scaledWidth) * 0.5f;
                    }
                }
            }

            // Draw the scaled image
            UIGraphics.BeginImageContextWithOptions(targetSize, false, 0f);

            var thumbnailRect = CGRect.Empty;

            thumbnailRect.Location = thumbnailPoint;
            thumbnailRect.Size = new CGSize(scaledWidth, scaledHeight);

            sourceImage.Draw(thumbnailRect);

            var newImage = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            return newImage;
        }
    }

    public enum CropOrigoTypes
    {
        Top,
        Center,
        Bottom
    }
}