using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using BrandImageGenerator.EventHelpers;

namespace BrandImageGenerator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            if(!VerifyTwitterApiKeys())
            {
                const string msgText = "The Twitter API Keys have not been properly defined. \nPlease enter valid Twitter API keys in the configuration and restart the app to continue.";
                var msgTitle = ConfigurationManager.AppSettings.Get("ApplicationName") + " - Error";
                MessageBox.Show(msgText, msgTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Close();
                Application.Current.Shutdown();
            }

            InitializeComponent();
            CombinedImage.DownloadCompleted += CombinedImageDownloadCompleted;
            TwitterHandleTextBlock.Focus();

            Background =
                new SolidColorBrush(
                    (Color) ColorConverter.ConvertFromString(ConfigurationManager.AppSettings.Get("BrandColor")));
            Title = ConfigurationManager.AppSettings.Get("ApplicationName");
            
            
        }
 
        private bool VerifyTwitterApiKeys()
        {
            var settings = ConfigurationManager.AppSettings;
            var keys = settings.AllKeys.Where(x=> x.StartsWith("Twitter")).ToList();
            foreach(var key in keys)
            {
                if(settings.Get(key) == string.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        private void CombinedImageDownloadCompleted(object sender, TwitterDownloadCompletedEventArgs e)
        {
            var s = CombinedImage.RenderSize;
            s.Height = 375;
            s.Width = 750;

            CombinedImage.SaveTo(e.TwitterHandle, s);
        }

        private void SubmitButton_OnClick(object sender, RoutedEventArgs e)
        {
            CombinedImage.TwitterHandle = TwitterHandleTextBlock.Text;
            if (OpenWindowsExplorer.IsChecked ?? false)
            {
                OpenExplorerWindow(Directory.GetCurrentDirectory() + string.Format(@"\{0}.png", TwitterHandleTextBlock.Text));
            }
        }

        private static void OpenExplorerWindow(string filePath)
        {
            var explorerWindowProcess = new Process
            {
                StartInfo =
                {
                    FileName = "explorer.exe",
                    Arguments = "/select,\"" + filePath + "\""
                }
            };

            explorerWindowProcess.Start();
        }

        private void TwitterHandleTextBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                SubmitButton_OnClick(this, null);
            }
        }
    }
}