using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using FluentAvalonia.UI.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Image2Display.Models
{
    internal class SplashScreen : IApplicationSplashScreen
    {
        public string AppName => "Image2Display";

        public IImage AppIcon
        {
            get
            {
                using var stream = AssetLoader.Open(new Uri("avares://Image2Display/Assets/logo.png"));
                var bmp = new Bitmap(stream);
                return bmp;
            }
        }

        public object SplashScreenContent => null;

        public int MinimumShowTime => 500;

        public Task RunTasks(CancellationToken cancellationToken)
        {
            //nothing
            return Task.CompletedTask;
        }
    }
}
