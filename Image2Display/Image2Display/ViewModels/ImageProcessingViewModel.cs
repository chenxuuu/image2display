using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    }
}
