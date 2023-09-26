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
        private ImageData? _imageData;

        [ObservableProperty]
        public Bitmap? _imageShow = null;
        [ObservableProperty]
        private bool _bitmapLoading = false;
        [ObservableProperty]
        public IImmutableSolidColorBrush? _bgColor = Brushes.White;


        //刷新图片
        private async Task RefreshImage()
        {
            if(_imageData == null)
            {
                ImageShow?.Dispose();
                ImageShow = null;
                return;
            }
            BitmapLoading = true;
            await Task.Run(() =>
            {
                ImageShow = ImageHelper.LoadFromImageData(_imageData!);
            });
            BitmapLoading = false;
        }

        [RelayCommand]
        private async Task ImportImage()
        {
            var r = await DialogHelper.ShowOpenFileDialogAsync(FilePickerFileTypes.ImageAll);
            if (r is null || r.Count == 0)
                return;

            var file = r.First();
            try
            {
                _imageData = new ImageData(file.Path.LocalPath);
                await RefreshImage();
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                return;
            }
        }

        [RelayCommand]
        private async Task ClearImage()
        {
            _imageData?.Dispose();
            _imageData = null;
            await RefreshImage();
        }

        [RelayCommand]
        private void ChangeBgColor()
        {
            if(BgColor == Brushes.White)
            {
                BgColor = Brushes.Black;
            }
            else
            {
                BgColor = Brushes.White;
            }
        }
    }
}
