using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Image2Display.Helpers;
using Image2Display.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.ViewModels
{
    public partial class ImageConvertViewModel : ViewModelBase
    {
        public ImageConvertViewModel()
        {
            if (Utils.ImageDataTemp != null)
            {
                ImportImage(Utils.ImageDataTemp);
                Utils.ImageDataTemp = null;
            }
            else
            {
                Utils.ImportImageAction = ImportImage;
            }
        }

        private void ImportImage(ImageData img)
        {
            Image = img;
        }

        private ImageData? Image = null;


        [RelayCommand]
        private void Test()
        {
            if (Image == null)
            {
                Debug.WriteLine("Image is null");
                return;
            }
            Debug.WriteLine($"{Image.Width},{Image.Height}");
        }
    }
}
