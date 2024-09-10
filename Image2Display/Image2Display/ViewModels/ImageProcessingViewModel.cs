using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Image2Display.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.ViewModels
{
    public partial class ImageProcessingViewModel : ViewModelBase
    {
        //裁剪
        [ObservableProperty]
        private bool _cropImage = false;

        [ObservableProperty]
        private int _imageWidth = 0;
        [ObservableProperty]
        private int _imageHeight = 0;

        [ObservableProperty]
        private int _cropX1 = 0;
        [ObservableProperty]
        private int _cropY1 = 0;
        [ObservableProperty]
        private int _cropX2 = 0;
        [ObservableProperty]
        private int _cropY2 = 0;

        //缩放
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ManuallyScale))]
        private bool _imageScaling = false;
        [ObservableProperty]
        private int _imageWidthCroped = 0;
        [ObservableProperty]
        private int _imageHeightCroped = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ManuallyScale))]
        private int _quickScale = 0;
        [ObservableProperty]
        private bool _keepAspectRatio = false;
        [ObservableProperty]
        private bool _stretch = false;
        [ObservableProperty]
        private int _scaleWidth = 0;
        [ObservableProperty]
        private int _scaleHeight = 0;
        [ObservableProperty]
        private int _scaleAlgorithm = 0;
        public bool ManuallyScale
        {
            get => ImageScaling && QuickScale == 0;
        }

        //颜色处理
        [ObservableProperty]
        private bool _preserveTransparency = true;
        [ObservableProperty]
        private Color _transparencyColor = Colors.LightBlue;
        [ObservableProperty]
        private int _brightness = 100;
        [ObservableProperty]
        private int _contrast = 100;
        [ObservableProperty]
        private int _saturation = 100;
        [ObservableProperty]
        private bool _invertColors = false;

        //旋转和翻转
        [ObservableProperty]
        private int _rotateImage = 0;
        [ObservableProperty]
        private int _mirrorProcessing = 0;

        //颜色量化
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BinarizationThresholdShow))]
        private bool _binarization = false;
        [ObservableProperty]
        private int _binarizationThreshold = 128;
        [ObservableProperty]
        private bool _binarizationInversion = false;
        [ObservableProperty]
        private int _quantization = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BinarizationThresholdShow))]
        private int _quantizationAlgorithm = 0;
        public bool BinarizationThresholdShow
        {
            get => Binarization && QuantizationAlgorithm == 0;
        }

        //图片展示
        [ObservableProperty]
        private bool _isShowOriginalImage = false;
        [ObservableProperty]
        private Bitmap? _originalImage = null;
        [ObservableProperty]
        private Bitmap? _processedImage = null;

        //开启一个定时器，用于更新显示原图的变量
        private System.Timers.Timer ShowOriginalImageTimer = new();

        [RelayCommand]
        private void ShowOriginalImage()
        {
            IsShowOriginalImage = true;
            ShowOriginalImageTimer.Stop();
            ShowOriginalImageTimer.Start();
        }

        [RelayCommand]
        private async Task OpenImageFile()
        {
            var files = await DialogHelper.ShowOpenFileDialogAsync(FilePickerFileTypes.ImageAll, false);
            if (files.Count == 0)
                return;
            //读取图片，导入到OriginalImage内
            OriginalImage = new Bitmap(Path.GetFullPath(files[0].Path.LocalPath));
            ProcessedImage = new Bitmap(Path.GetFullPath(files[0].Path.LocalPath));
        }

        [RelayCommand]
        private async Task SaveImageFile()
        {
            //保存图片
            var path = await DialogHelper.ShowSaveFileDialogAsync("png");
            if (path == null)
                return;
            ProcessedImage?.Save(path);
        }

        /// <summary>
        /// 初始化ImageProcessingViewModel
        /// </summary>
        public ImageProcessingViewModel()
        {
            ShowOriginalImageTimer.Elapsed += (_,_) => IsShowOriginalImage = false;
            ShowOriginalImageTimer.Interval = 50;
            ShowOriginalImageTimer.AutoReset = false;
        }
    }
}
