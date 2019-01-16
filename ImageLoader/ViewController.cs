using System;
using ImageLoader.Common;
using UIKit;

namespace ImageLoader
{
    public partial class ViewController : UIViewController
    {
        private UIImageView _imageView;
        private UIButton _cropButton, _scaleButton, _topCropButton;
        private Common.ImageLoader _imageLoader;

        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.BackgroundColor = UIColor.White;

            _imageLoader = new Common.ImageLoader();

            _imageView = new UIImageView
            {
                BackgroundColor = UIColor.Gray,
                ContentMode = UIViewContentMode.Center,
                ClipsToBounds = true
            };

            _cropButton = new UIButton();
            _cropButton.SetTitle("Crop", UIControlState.Normal);
            _cropButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            _cropButton.TouchUpInside += (sender, e) => 
            {
                LoadImage(ImageScaleTypes.Crop);
            };

            _scaleButton = new UIButton();
            _scaleButton.SetTitle("Scale", UIControlState.Normal);
            _scaleButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            _scaleButton.TouchUpInside += (sender, e) =>
            {
                LoadImage(ImageScaleTypes.ScaleToFit);
            };

            _topCropButton = new UIButton();
            _topCropButton.SetTitle("Top Crop", UIControlState.Normal);
            _topCropButton.SetTitleColor(UIColor.Black, UIControlState.Normal);
            _topCropButton.TouchUpInside += (sender, e) =>
            {
                LoadImage(ImageScaleTypes.TopCrop);
            };

            View.AddSubviews(_imageView, _cropButton, _scaleButton, _topCropButton);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            _imageView.Frame = new CoreGraphics.CGRect(20, 100, View.Frame.Width - 40, View.Frame.Width - 40);

            _cropButton.Frame = new CoreGraphics.CGRect(20, _imageView.Frame.Bottom + 20f, View.Frame.Width - 40, 30f);
            _scaleButton.Frame = new CoreGraphics.CGRect(20, _cropButton.Frame.Bottom + 10f, View.Frame.Width - 40, 30f);
            _topCropButton.Frame = new CoreGraphics.CGRect(20, _scaleButton.Frame.Bottom + 10f, View.Frame.Width - 40, 30f);

            LoadImage(ImageScaleTypes.Crop);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        private void LoadImage(ImageScaleTypes scaleType)
        {
            _imageLoader?.LoadImageAsync(
                "https://raw.githubusercontent.com/Manne990/PhotoViewerTest/master/iOS/Resources/bild2.jpg",
                (int)_imageView.Frame.Width,
                (int)_imageView.Frame.Height,
                (image) => _imageView.Image = image,
                scaleType
                , true);
        }
    }
}