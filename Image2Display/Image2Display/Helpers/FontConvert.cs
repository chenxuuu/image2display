using Image2Display.Models;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.Helpers
{
    public class FontConvert
    {
        /// <summary>
        /// 获取格子像素图片
        /// </summary>
        /// <param name="data">每个点的数据</param>
        /// <param name="width">像素高度</param>
        /// <param name="height">像素宽度</param>
        /// <param name="image">图片缓冲区，没给的话会新建一个</param>
        /// <returns>图片缓冲</returns>
        public static ImageData GetImage(byte[] data, int width, int height,ImageData? image = null)
        {
            //像素尺寸
            var pixelSize = 8;
            //间隔尺寸
            var padding = 1;
            //间隔颜色
            var paddingColor = new Rgba32(45, 25, 70, 255);

            //计算一下实际尺寸
            var realWidth = width * pixelSize + (width - 1) * padding;
            var realHeight = height * pixelSize + (height - 1) * padding;
            //保证缓冲区存在
            image ??= new ImageData(realWidth, realHeight);
            //如果尺寸不一致，改一下尺寸
            if (image.Width != realWidth || image.Height != realHeight)
            {
                image.Reset(realWidth, realHeight);
            }
            //背景色黑色
            image.DrawRect(0, 0, realWidth, realHeight, new Rgba32(0, 0, 0, 255));

            //画横向的格子线
            for (var i = 0; i < width - 1; i++)
            {
                image.DrawRect((i * (pixelSize + padding)) + pixelSize, 0, padding, realHeight, paddingColor);
            }
            //画纵向的格子线
            for (var i = 0; i < height - 1; i++)
            {
                image.DrawRect(0, (i * (pixelSize + padding)) + pixelSize, realWidth, padding, paddingColor);
            }

            //画像素
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var index = y * width + x;
                    var color = new Rgba32(data[index], data[index], data[index], 255);
                    image.DrawRect(x * (pixelSize + padding), y * (pixelSize + padding), pixelSize, pixelSize, color);
                }
            }

            return image;
        }
    }
}
