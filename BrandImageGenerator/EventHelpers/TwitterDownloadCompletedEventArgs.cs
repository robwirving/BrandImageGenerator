using System;

namespace BrandImageGenerator.EventHelpers
{
    public class TwitterDownloadCompletedEventArgs : EventArgs
    {
        public string TwitterHandle { get; set; }
    }
}
