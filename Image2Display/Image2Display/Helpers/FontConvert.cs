using Image2Display.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.Helpers;

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
    public static ImageData GetImage(byte[] data, int width, int height, ImageData? image = null)
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

    public static byte[] GetData(SKTypeface font, int size, char c, int width, int height, int offsetx, int offsety)
    {
        //Debug.WriteLine($"Font: {font.FamilyName}, Size: {size}, Char: {c}, Width: {width}, Height: {height}, OffsetX: {offsetx}, OffsetY: {offsety}");
        //创建画布
        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        //Debug.WriteLine("Surface Created");
        //创建画笔
        using var paint = new SKPaint
        {
            Color = SKColors.White,
            TextSize = size,
            Typeface = font,
            TextAlign = SKTextAlign.Center,
            IsAntialias = true,
        };
        //Debug.WriteLine("Paint Created");
        //画布上画字
        surface.Canvas.DrawText(c.ToString(), width/2 + offsetx, height - offsety, paint);
        //Debug.WriteLine("Text Drawn");
        //获取像素数据
        IntPtr data = surface.PeekPixels().GetPixels();
        //Debug.WriteLine("Pixels Get");
        var result = new byte[width * height];

        // 获取像素数据
        using var pixmap = surface.PeekPixels();
        //Debug.WriteLine("pixmap got");
        if (pixmap != null)
        {
            //Debug.WriteLine("get data");
            // 遍历像素数据
            for (int y = 0; y < pixmap.Height; y++)
            {
                for (int x = 0; x < pixmap.Width; x++)
                {
                    IntPtr pixel = pixmap.GetPixels(x, y);
                    var buff = new byte[pixmap.BytesPerPixel];
                    System.Runtime.InteropServices.Marshal.Copy(pixel, buff, 0, buff.Length);
                    result[x + y * width] = buff[0];
                }
            }
        }
        //Debug.WriteLine("Data got");

        return result;
    }

    /// <summary>
    /// 数据二值化处理（仅用于预览）
    /// </summary>
    public static void ThresholdImage(byte[] data,  byte threshold)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = data[i] > threshold ? (byte)255 : (byte)0;
        }
    }

    /// <summary>
    /// 图片转换为特定灰度的图片（仅用于预览）
    /// </summary>
    public static void GrayScaleImage(byte[] data, int bit)
    {
        for (int i = 0; i < data.Length; i++)
        {
            //这个灰度位数，有多少种颜色
            var number = 1 << bit;
            //每个颜色的间隔
            var interval = 255 / (number - 1);
            //把每个格子的灰度对齐到最近的颜色
            data[i] = (byte)((data[i] / interval) * interval);
        }
    }

    /// <summary>
    /// 反转图像颜色（仅用于预览）
    /// </summary>
    public static void InvertImage(byte[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = (byte)(255 - data[i]);
        }
    }

    private static void IteratePixel(byte[] data, int rotate, int width, int height, Action<byte> action)
    {
        //主要开始1 主要结束1 次要开始2 次要结束2 是否先x后y
        var (m1, m2, p1, p2, xFirst) = rotate switch
        {
            0 => (0, width - 1, 0, height - 1, true),
            1 => (width - 1, 0, 0, height - 1, true),
            2 => (0, width - 1, height - 1, 0, true),
            3 => (width - 1, 0, height - 1, 0, true),
            4 => (0, height - 1, 0, width - 1, false),
            5 => (height - 1, 0, 0, width - 1, false),
            6 => (0, height - 1, width - 1, 0, false),
            7 => (height - 1, 0, width - 1, 0, false),
            _ => throw new NotImplementedException(),
        };

        var mStep = m1 < m2 ? 1 : -1;
        var pStep = p1 < p2 ? 1 : -1;
        if (xFirst)
        {
            for (var y = p1; y != p2 + pStep; y += pStep)
            {
                for (var x = m1; x != m2 + mStep; x += mStep)
                {
                    action(data[x + y * width]);
                }
            }
        }
        else
        {
            for (var x = p1; x != p2 + pStep; x += pStep)
            {
                for (var y = m1; y != m2 + mStep; y += mStep)
                {
                    action(data[x + y * width]);
                }
            }
        }
    }

    /// <summary>
    /// 获取处理后的数据
    /// </summary>
    /// <returns></returns>
    public static List<byte> GetResultData(
        byte[] raw, int width, int height,
        bool isGray, int bit, byte threshold,
        bool isInvert, int byteOrder, bool bitOrderMSB)
    {
        //先取反
        if(isInvert)
            InvertImage(raw);

        var bitLength = 1;
        if (isGray)
            bitLength = bit;

        var result = new List<byte>();
        int bitIndex = 0;
        byte lastByte = 0;
        //按像素顺序遍历
        IteratePixel(raw, byteOrder, width, height, (b) =>
        {
            if(isGray)
            {
                //灰度处理
                b = (byte)(b >> (8 - bitLength));
            }
            else
            {
                //二值化处理
                b = b > threshold ? (byte)1 : (byte)0;
            }
            //反向数据
            if (!bitOrderMSB)
            {
                var temp = (byte)0;
                for (int i = 0; i < bitLength; i++)
                {
                    temp <<= 1;
                    temp |= (byte)(b & 1);
                    b >>= 1;
                }
                b = temp;
            }
            //添加到结果
            lastByte <<= bitLength;
            lastByte |= b;
            bitIndex += bitLength;
            if (bitIndex >= 8)
            {
                result.Add(lastByte);
                lastByte = 0;
                bitIndex = 0;
            }
        });
        //最后一个字节
        if (bitIndex > 0)
        {
            lastByte <<= 8 - bitIndex;
            result.Add(lastByte);
        }
        return result;
    }


    /// <summary>
    /// 将字体数据转换为C数组
    /// </summary>
    /// <param name="data">数据，每个字一个数组</param>
    /// <param name="width">字库宽度</param>
    /// <param name="height">字库高度</param>
    /// <param name="charset">实际用的字符集</param>
    /// <returns></returns>
    public static string ByteListToCArray(List<List<byte>> data, int width, int height, IList<char> charset)
    {
        var sb = new StringBuilder();
        if (Utils.Settings.Language.Contains("ZH", StringComparison.CurrentCultureIgnoreCase))
            sb.Append("/*@注意：此文件由Image2Display生成 */\n");
        else
            sb.Append("/*@Notice: This file is generated by Image2Display */\n");
        if (data.Count == 0 || data[0].Count == 0 || charset.Count == 0)
            sb.Append("/* no data */\n");
        else
        {
            sb.Append($"/*@Size: {width}x{height}, " +
                $"Char: {charset.Count}," +
                $"Data per char: {data[0].Count} */\n");
        }
        //每个字符列出来
        sb.Append("const char charset[] = \"");
        foreach (var c in charset)
        {
            //转义字符
            if (c == '\'')
                sb.Append("\\'");
            else if(c == '\\')
                sb.Append("\\\\");
            else
                sb.Append($"{c}");
        }
        sb.Append("\";\n\n");
        sb.Append("const uint8_t fonts[] = {\n");
        //每行一个字
        for (int i = 0; i < charset.Count; i++)
        {
            sb.Append($"/* {charset[i]} */\n");
            foreach (var b in data[i])
            {
                sb.Append($"0x{b:X2},");
            }
            sb.Append("\n\n");
        }
        sb.Append("};\n");
        return sb.ToString();
    }

    public static string ByteListToExplain(List<List<byte>> data, int width, int height, IList<char> charset)
    {
        var sb = new StringBuilder();
        if (Utils.Settings.Language.Contains("ZH", StringComparison.CurrentCultureIgnoreCase))
            sb.Append("/*@注意：此文件由Image2Display生成 */\n");
        else
            sb.Append("/*@Notice: This file is generated by Image2Display */\n");
        if (data.Count == 0 || data[0].Count == 0 || charset.Count == 0)
            sb.Append("/* no data */\n");
        else
        {
            sb.Append($"/*@Size: {width}x{height}, " +
                $"Char: {charset.Count}," +
                $"Data per char: {data[0].Count} */\n");
        }
        //每个字符列出来
        sb.Append("const char charset[] = \"");
        foreach (var c in charset)
        {
            //转义字符
            if (c == '\'')
                sb.Append("\\'");
            else if (c == '\\')
                sb.Append("\\\\");
            else
                sb.Append($"{c}");
        }
        sb.Append("\";\n");
        return sb.ToString();
    }
}
