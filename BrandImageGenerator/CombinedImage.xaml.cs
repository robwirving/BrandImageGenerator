using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BrandImageGenerator.EventHelpers;

namespace BrandImageGenerator
{
    public partial class CombinedImage : UserControl
    {
        // Keep a local reference to more easily set-up/remove events
        private BitmapImage _bi;

        private string _twitterHandle;

        public CombinedImage()
        {
            InitializeComponent();
            
            // Need this only for when something fails, then there shouldn't be a null exception
            _bi = new BitmapImage();

            UpdateImage();

            BrandingImage.Source = GetBrandingImage();
            GuestBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ConfigurationManager.AppSettings.Get("BrandColor")));

            // Set-up initial download completed event handler
            _bi.DownloadCompleted += bi_DownloadCompleted;
        }
 
        private ImageSource GetBrandingImage()
        {
            var imagePath = ConfigurationManager.AppSettings.Get("BrandBaseImage");
            
            // Attempt to detect when a relative path was used, then assume file was in current directory
            if (!imagePath.Contains(":") && imagePath.StartsWith(@"\")) 
                imagePath = Directory.GetCurrentDirectory() + imagePath;

            return new BitmapImage(new Uri(imagePath));
        }

        public string TwitterHandle
        {
            get { return _twitterHandle; }
            set
            {
                _twitterHandle = value;
                UpdateImage();
            }
        }

        public event EventHandler<TwitterDownloadCompletedEventArgs> DownloadCompleted;

        private void UpdateImage()
        {
            var tUrl = new TwitterAvatarLookup().GetTwitterAvatarUrl(TwitterHandle ?? "msdevshow");
            tUrl = Regex.Replace(tUrl, "_normal", string.Empty);

            // If twitter returns default... twitter didn't find anything, we're gonna orange egg it.
            if (tUrl == "default.png") tUrl = "http://a0.twimg.com/sticky/default_profile_images/default_profile_3.png";

            _bi = new BitmapImage(new Uri(tUrl));
            _bi.DownloadCompleted += bi_DownloadCompleted;

            GuestImage.Source = _bi;
        }

        private void bi_DownloadCompleted(object sender, EventArgs e)
        {
            _bi.DownloadCompleted -= bi_DownloadCompleted;
            DownloadCompleted(this, new TwitterDownloadCompletedEventArgs {TwitterHandle = TwitterHandle});
        }
    }
}