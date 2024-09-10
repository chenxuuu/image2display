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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Image2Display.ViewModels
{
    public partial class ImageProcessingViewModel : ViewModelBase
    {
        //处理错误信息
        [ObservableProperty]
        private bool _isError = false;
        [ObservableProperty]
        private string _errorMessage = "";



        //裁剪
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageWidthCroped), nameof(ImageHeightCroped))]
        private bool _cropImage = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageWidthCroped), nameof(ImageHeightCroped))]
        private int _imageWidth = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageWidthCroped), nameof(ImageHeightCroped))]
        private int _imageHeight = 0;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageWidthCroped))]
        private int _cropX1 = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageHeightCroped))]
        private int _cropY1 = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageWidthCroped))]
        private int _cropX2 = 0;
        [ObservableProperty]
        [NotifyPropertyChangedFor( nameof(ImageHeightCroped))]
        private int _cropY2 = 0;

        //缩放
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ManuallyScale))]
        private bool _imageScaling = false;
        public int ImageWidthCroped
        {
            get 
            {
                if (CropImage)
                    return CropX2 - CropX1 + 1;
                else
                    return ImageWidth;
            }
        }
        public int ImageHeightCroped
        {
            get
            {
                if (CropImage)
                    return CropY2 - CropY1 + 1;
                else
                    return ImageHeight;
            }
        }
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

        //实际的图片数据
        private ImageData? RealOriginalImage = null;
        private void RefreshOriginalImage()
        {
            if (RealOriginalImage == null)
                return;
            using var stream = RealOriginalImage.GetStream();
            OriginalImage = new Bitmap(stream);
        }

        private ImageData? RealProcessedImage = null;
        private void RefreshProcessedImage()
        {
            if (RealProcessedImage == null)
                return;
            using var stream = RealProcessedImage.GetStream();
            ProcessedImage = new Bitmap(stream);
        }

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

            //之前可能存在的数据需要释放
            RealOriginalImage?.Dispose();
            RealProcessedImage?.Dispose();

            //读取图片，导入到OriginalImage内
            RealOriginalImage = new ImageData(files[0].Path.LocalPath);
            ImageWidth = RealOriginalImage.Width;
            ImageHeight = RealOriginalImage.Height;
            //复制图片数据到Processed
            RealProcessedImage = new ImageData(RealOriginalImage);

            //刷新到UI
            RefreshOriginalImage();
            RefreshProcessedImage();

            //初始化其他变量
            ImageWidth = RealOriginalImage.Width;
            ImageHeight = RealOriginalImage.Height;
            CropX1 = 0;
            CropY1 = 0;
            CropX2 = ImageWidth - 1;
            CropY2 = ImageHeight - 1;
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


            PropertyChanged += (sender, e) =>
            {
                var dismissList = new List<string>
                {
                    nameof(IsError),
                    nameof(IsProcessing),
                    nameof(ImageWidthCroped),
                    nameof(ImageHeightCroped),
                    nameof(ManuallyScale),
                    nameof(ProcessedImage),
                    nameof(IsShowOriginalImage),
                };
                //某个变量被更改
                var name = e.PropertyName;
                if(name == null || dismissList.Contains(name))
                    return;

                Debug.WriteLine($"Property Changed: {name}");

                //预览图片
                if (RealOriginalImage != null)
                    NeedProcess = true;
            };

            //开启一个线程，用于处理图片
            Task.Run(async () =>
            {
                while (true)
                {
                    if (NeedProcess)
                    {
                        NeedProcess = false;
                        await Task.Delay(100);
                        if(NeedProcess)
                            continue;
                        if (RealOriginalImage == null)
                            continue;
                        IsProcessing = true;

                        //备份一下上一次处理的图片
                        var lastImg = RealProcessedImage;
                        try
                        {
                            var (img,error) = ProcessImage();
                            if(img != null)
                            {
                                RealProcessedImage = img;
                                RefreshProcessedImage();
                                lastImg?.Dispose();
                            }
                            else
                            {
                                ErrorMessage = error!;
                            }
                            IsError = img == null;
                        }
                        catch(Exception e)
                        {
                            IsError = true;
                            ErrorMessage = e.Message;
                        }
                        IsProcessing = false;
                    }
                    await Task.Delay(100);
                }
            });
        }



        [ObservableProperty]
        private bool _isProcessing = false;
        private bool NeedProcess = false;

        private (ImageData?,string?) ProcessImage()
        {
            if(RealOriginalImage == null)
                return (null,"image data is null");

            //备份一份图片用于处理
            using var img = new ImageData(RealOriginalImage);
            //裁剪
            if (CropImage)
            {
                if(!img.Crop(CropX1, CropY1, CropX2 - CropX1, CropY2 - CropY1))
                    return (null, "crop failed");
            }


            return (new ImageData(img),null);
        }
    }
}
