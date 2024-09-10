using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Dithering;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.Models
{
    public class ImageData : IDisposable
    {
        /// <summary>
        /// 图片的原始数据
        /// </summary>
        private Image<Rgba32> Raw;

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width => Raw.Width;
        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height => Raw.Height;

        /// <summary>
        /// 新建空的图片数据
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        public ImageData(int width, int height) => Raw = new Image<Rgba32>(width, height);

        /// <summary>
        /// 从已有图片数据导入图片
        /// </summary>
        /// <param name="path">文件路径</param>
        public ImageData(string path) => Raw = Image.Load<Rgba32>(path);

        /// <summary>
        /// 从已有图片数据导入图片
        /// </summary>
        /// <param name="img"></param>
        public ImageData(ImageData img)
        {
            using var stream = img.GetStream();
            Raw = Image.Load<Rgba32>(stream);
        }


        /// <summary>
        /// 裁剪图片区域
        /// </summary>
        /// <param name="x">左上角起始点位置x</param>
        /// <param name="y">左上角起始点位置y</param>
        /// <param name="width">裁剪区域宽度</param>
        /// <param name="height">裁剪区域高度</param>
        /// <returns>是否成功</returns>
        public bool Crop(int x, int y, int width, int height)
        {
            if (x < 0 || y < 0 || width < 0 || height < 0)
                return false;
            if (x + width > Raw.Width || y + height > Raw.Height)
                return false;
            Raw.Mutate(ctx => ctx.Crop(new Rectangle(x,y,width,height)));
            return true;
        }

        public bool Resize(int width, int height, bool stretch = false, IResampler? algorithm = null)
        {
            if (width <= 0 || height <= 0)
                return false;

            algorithm ??= KnownResamplers.NearestNeighbor;

            Raw.Mutate(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = stretch ? ResizeMode.Stretch : ResizeMode.Crop,
                Sampler = algorithm
            }));

            return true;
        }

        /// <summary>
        /// 拓展图片大小（向右下方拓展空白区域）
        /// </summary>
        /// <param name="width">目标宽度</param>
        /// <param name="height">目标高度</param>
        /// <returns>是否成功</returns>
        public bool Expand(int width, int height)
        {
            if (width < Width || height < Height)//不能小于当前尺寸
                return false;
            var n = new Image<Rgba32>(width, height);
            n.Mutate(ctx => ctx.DrawImage(Raw, new Point(0, 0), 1));
            Raw = n;
            return true;
        }

        public bool AddBackGround(int width, int height, Rgba32 color)
        {
            if (width < Width || height < Height)//不能小于当前尺寸
                return false;
            var n = new Image<Rgba32>(width, height);
            n.Mutate(ctx => ctx.Fill(color));
            n.Mutate(ctx => ctx.DrawImage(Raw, new Point(0, 0), 1));
            Raw = n;
            return true;
        }

        public bool SetBrightness(float value)
        {
            Raw.Mutate(ctx => ctx.Brightness(value));
            return true;
        }

        public bool SetContrast(float value)
        {
            Raw.Mutate(ctx => ctx.Contrast(value));
            return true;
        }

        public bool SetSaturation(float value)
        {
            Raw.Mutate(ctx => ctx.Saturate(value));
            return true;
        }

        public bool Invert()
        {
            Raw.Mutate(ctx => ctx.Invert());
            return true;
        }

        public bool Rotate(float degree)
        {
            Raw.Mutate(ctx => ctx.Rotate(degree));
            return true;
        }

        public bool Flip(bool horizontal, bool vertical)
        {
            if (horizontal)
                Raw.Mutate(ctx => ctx.Flip(FlipMode.Horizontal));
            if (vertical)
                Raw.Mutate(ctx => ctx.Flip(FlipMode.Vertical));
            return true;
        }

        public bool BinPalette(byte Threshold)
        {
            var th = (float)Threshold / 0x100f;
            Raw.Mutate(ctx => ctx.BinaryThreshold(th,BinaryThresholdMode.MaxChroma));
            return true;
        }

        public bool BinDither(IDither dither)
        {
            Raw.Mutate(ctx => ctx.Grayscale().BinaryDither(dither));
            return true;
        }

        //降颜色位数
        public bool ReduceColor(int colorCount, IQuantizer quantizer)
        {
            switch(quantizer)
            {
                case OctreeQuantizer _:
                    var qo = new OctreeQuantizer(new QuantizerOptions
                    {
                        MaxColors = colorCount
                    });
                    Raw.Mutate(ctx => ctx.Quantize(qo));
                    break;
                case WuQuantizer _:
                    var qw = new WuQuantizer(new QuantizerOptions
                    {
                        MaxColors = colorCount
                    });
                    Raw.Mutate(ctx => ctx.Quantize(qw));
                    break;
                case WebSafePaletteQuantizer _:
                    var qws = new WebSafePaletteQuantizer(new QuantizerOptions
                    {
                        MaxColors = colorCount
                    });
                    Raw.Mutate(ctx => ctx.Quantize(qws));
                    break;
                case WernerPaletteQuantizer _:
                    var qwe = new WernerPaletteQuantizer(new QuantizerOptions
                    {
                        MaxColors = colorCount
                    });
                    Raw.Mutate(ctx => ctx.Quantize(qwe));
                    break;
                default:
                    return false;
            }  
            return true;
        }

        private static Color[] GetPalette(int colorBits)
        {
            int colorsPerChannel = 1 << (colorBits / 3);
            int totalColors = colorsPerChannel * colorsPerChannel * colorsPerChannel;

            Color[] palette = new Color[totalColors];
            int index = 0;

            for (int r = 0; r < colorsPerChannel; r++)
            {
                for (int g = 0; g < colorsPerChannel; g++)
                {
                    for (int b = 0; b < colorsPerChannel; b++)
                    {
                        byte red = (byte)(r * 255 / (colorsPerChannel - 1));
                        byte green = (byte)(g * 255 / (colorsPerChannel - 1));
                        byte blue = (byte)(b * 255 / (colorsPerChannel - 1));

                        palette[index++] = Color.FromRgb(red, green, blue);
                    }
                }
            }

            return palette;
        }

        /// <summary>
        /// 获取Stream，用于显示或保存
        /// </summary>
        /// <returns>获取到的Stream</returns>
        public Stream GetStream()
        {
            var ms = new MemoryStream();
            Raw.SaveAsPng(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public void Dispose()
        {
            Raw?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
