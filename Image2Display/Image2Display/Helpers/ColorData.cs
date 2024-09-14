using Image2Display.Models;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Image2Display.Helpers;
public class ColorData
{
    /// <summary>
    /// 按指定的顺序遍历图片所有像素
    /// </summary>
    /// <param name="img">图片</param>
    /// <param name="rotate">遍历顺序</param>
    /// <param name="action">回调函数</param>
    /// <exception cref="NotImplementedException"></exception>
    private static void IterateImg(Image<Rgba32> img, int rotate, Action<Rgba32> action)
    {
        //主要开始1 主要结束1 次要开始2 次要结束2 是否先x后y
        var (m1,m2,p1,p2,xFirst) = rotate switch
        {
            0 => (0,img.Width-1, 0, img.Height - 1,true),
            1 => (img.Width - 1, 0, 0, img.Height - 1, true),
            2 => (0, img.Width - 1, img.Height - 1, 0, true),
            3 => (img.Width - 1, 0, img.Height - 1, 0, true),
            4 => (0, img.Height - 1, 0, img.Width - 1, false),
            5 => (img.Height - 1, 0, 0, img.Width - 1, false),
            6 => (0, img.Height - 1, img.Width - 1, 0, false),
            7 => (img.Height - 1, 0, img.Width - 1, 0, false),
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
                    Debug.WriteLine($"x:{x} y:{y}");
                    action(img[x, y]);
                }
            }
        }
        else
        {
            for (var x = p1; x != p2 + pStep; x += pStep)
            {
                for (var y = m1; y != m2 + mStep; y += mStep)
                {
                    Debug.WriteLine($"x:{x} y:{y}");
                    action(img[x, y]);
                }
            }
        }
    }

    /// <summary>
    /// 颜色反序存储
    /// </summary>
    /// <param name="n">数值</param>
    /// <param name="bits">位数</param>
    /// <returns>结果</returns>
    private static int ReverseBits(int n, int bits)
    {
        int result = 0;
        for (int i = 0; i < bits; i++)
        {
            result = (result << 1) | (n & 1);
            n >>= 1;
        }
        return result;
    }

    public static List<byte> Get1_2_4_8BitsImage(Image<Rgba32> img, int rotate, bool reverseBits, int bitsPerPixel, List<Rgba32> colors)
    {
        var l = new List<byte>();
        var count = 0;
        byte temp = 0;

        IterateImg(img, rotate, (color) =>
        {
            var index = colors.IndexOf(color);
            if (index == -1)
                throw new Exception("Color not found");

            // 反转位顺序
            index = reverseBits ? ReverseBits(index, bitsPerPixel) : index;

            // 将索引值填充到临时的byte变量中
            temp = (byte)(temp << bitsPerPixel);
            temp |= (byte)(index & ((1 << bitsPerPixel) - 1));

            count += bitsPerPixel;
            if (count == 8)
            {
                l.Add(temp);
                count = 0;
                temp = 0;
            }
        });

        return l;
    }

    public static List<byte> Get16BitsImage(Image<Rgba32> img, int rotate, bool reverseBits, bool isSmallEndian, List<Rgba32> colors)
    {
        var l = new List<byte>();
        ushort temp = 0;

        IterateImg(img, rotate, (color) =>
        {
            var index = colors.IndexOf(color);
            if (index == -1)
                throw new Exception("Color not found");

            // 反转位顺序
            index = reverseBits ? ReverseBits(index, 16) : index;

            // 将索引值填充到临时的byte变量中
            temp = (ushort)index;

            if (isSmallEndian)//小端字节反序
            {
                l.Add((byte)(temp & 0xFF));
                l.Add((byte)((temp >> 8) & 0xFF));
            }
            else
            {
                l.Add((byte)((temp >> 8) & 0xFF));
                l.Add((byte)(temp & 0xFF));
            }
        });

        return l;
    }

    public static List<byte> GetRGB444Image(Image<Rgba32> img, int rotate, bool reverseBits)
    {
        var l = new List<byte>();
        byte halfTemp = 0;
        bool isLastHalfByte = false;

        IterateImg(img, rotate, (color) =>
        {
            byte r = (byte)(color.R >> 4);
            byte g = (byte)(color.G >> 4);
            byte b = (byte)(color.B >> 4);
            if (reverseBits)
            {
                r = (byte)ReverseBits(r, 4);
                g = (byte)ReverseBits(g, 4);
                b = (byte)ReverseBits(b, 4);
            }

            if (isLastHalfByte)
            {
                halfTemp |= (byte)(r << 4);
                l.Add(halfTemp);
                isLastHalfByte = false;
                byte temp = (byte)(g << 4);
                temp |= b;
                l.Add(temp);
            }
            else
            {
                byte temp = (byte)(r << 4);
                temp |= g;
                l.Add(temp);
                halfTemp = (byte)(b << 4);
                isLastHalfByte = true;
            }
        });

        if (isLastHalfByte)
            l.Add(halfTemp);

        return l;
    }

    public static List<byte> GetRGB444HighEmptyImage(Image<Rgba32> img, int rotate, bool reverseBits, bool isSmallEndian)
    {
        var l = new List<byte>();

        IterateImg(img, rotate, (color) =>
        {
            byte r = (byte)(color.R >> 4);
            byte g = (byte)(color.G >> 4);
            byte b = (byte)(color.B >> 4);
            if (reverseBits)
            {
                r = (byte)ReverseBits(r, 4);
                g = (byte)ReverseBits(g, 4);
                b = (byte)ReverseBits(b, 4);
            }

            if(isSmallEndian)
            {
                byte temp = (byte)(g << 4);
                temp |= b;
                l.Add(temp);
                temp = r;
                l.Add(temp);
            }
            else
            {
                byte temp = r;
                l.Add(temp);
                temp = (byte)(g << 4);
                temp |= b;
                l.Add(temp);
            }
        });

        return l;
    }

    public static List<byte> GetRGB565Image(Image<Rgba32> img, int rotate, bool reverseBits, bool isSmallEndian)
    {
        var l = new List<byte>();

        IterateImg(img, rotate, (color) =>
        {
            byte r = (byte)(color.R >> 3);
            byte g = (byte)(color.G >> 2);
            byte b = (byte)(color.B >> 3);
            if (reverseBits)
            {
                r = (byte)ReverseBits(r, 5);
                g = (byte)ReverseBits(g, 6);
                b = (byte)ReverseBits(b, 5);
            }

            ushort temp = (ushort)(r << 11);
            temp |= (ushort)(g << 5);
            temp |= b;

            if (isSmallEndian)
            {
                l.Add((byte)(temp & 0xFF));
                l.Add((byte)((temp >> 8) & 0xFF));
            }
            else
            {
                l.Add((byte)((temp >> 8) & 0xFF));
                l.Add((byte)(temp & 0xFF));
            }
        });

        return l;
    }

    public static List<byte> GetRGB555HighEmptyImage(Image<Rgba32> img, int rotate, bool reverseBits, bool isSmallEndian)
    {
        var l = new List<byte>();

        IterateImg(img, rotate, (color) =>
        {
            byte r = (byte)(color.R >> 3);
            byte g = (byte)(color.G >> 3);
            byte b = (byte)(color.B >> 3);
            if (reverseBits)
            {
                r = (byte)ReverseBits(r, 5);
                g = (byte)ReverseBits(g, 5);
                b = (byte)ReverseBits(b, 5);
            }

            ushort temp = (ushort)(r << 10);
            temp |= (ushort)(g << 5);
            temp |= b;

            if (isSmallEndian)
            {
                l.Add((byte)(temp & 0xFF));
                l.Add((byte)((temp >> 8) & 0xFF));
            }
            else
            {
                l.Add((byte)((temp >> 8) & 0xFF));
                l.Add((byte)(temp & 0xFF));
            }
        });

        return l;
    }

    public static List<byte> GetRGB666HighEmptyImage(Image<Rgba32> img, int rotate, bool reverseBits, bool isSmallEndian)
    {
        var l = new List<byte>();

        IterateImg(img, rotate, (color) =>
        {
            byte r = (byte)(color.R >> 2);
            byte g = (byte)(color.G >> 2);
            byte b = (byte)(color.B >> 2);
            if (reverseBits)
            {
                r = (byte)ReverseBits(r, 6);
                g = (byte)ReverseBits(g, 6);
                b = (byte)ReverseBits(b, 6);
            }
            byte temp1 = (byte)(r >> 4);
            byte temp2 = (byte)(r << 4);
            temp2 |= (byte)(g >> 2);
            byte temp3 = (byte)(g << 6);
            temp3 |= b;

            if (isSmallEndian)
            {
                l.Add(temp3);
                l.Add(temp2);
                l.Add(temp1);
            }
            else
            {
                l.Add(temp1);
                l.Add(temp2);
                l.Add(temp3);
            }
        });

        return l;
    }

    public static List<byte> GetRGB888Image(Image<Rgba32> img, int rotate, bool reverseBits, bool isSmallEndian)
    {
        var l = new List<byte>();

        IterateImg(img, rotate, (color) =>
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            if (reverseBits)
            {
                r = (byte)ReverseBits(r, 8);
                g = (byte)ReverseBits(g, 8);
                b = (byte)ReverseBits(b, 8);
            }

            if (isSmallEndian)
            {
                l.Add(b);
                l.Add(g);
                l.Add(r);
            }
            else
            {
                l.Add(r);
                l.Add(g);
                l.Add(b);
            }
        });

        return l;
    }

    public static List<byte> GetARGB8888Image(Image<Rgba32> img, int rotate, bool reverseBits, bool isSmallEndian)
    {
        var l = new List<byte>();

        IterateImg(img, rotate, (color) =>
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;
            if (reverseBits)
            {
                r = (byte)ReverseBits(r, 8);
                g = (byte)ReverseBits(g, 8);
                b = (byte)ReverseBits(b, 8);
                a = (byte)ReverseBits(a, 8);
            }

            if (isSmallEndian)
            {
                l.Add(b);
                l.Add(g);
                l.Add(r);
                l.Add(a);
            }
            else
            {
                l.Add(a);
                l.Add(r);
                l.Add(g);
                l.Add(b);
            }
        });

        return l;
    }

    public static List<byte> GetRGBA8888Image(Image<Rgba32> img, int rotate, bool reverseBits, bool isSmallEndian)
    {
        var l = new List<byte>();

        IterateImg(img, rotate, (color) =>
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;
            byte a = color.A;
            if (reverseBits)
            {
                r = (byte)ReverseBits(r, 8);
                g = (byte)ReverseBits(g, 8);
                b = (byte)ReverseBits(b, 8);
                a = (byte)ReverseBits(a, 8);
            }

            if (isSmallEndian)
            {
                l.Add(a);
                l.Add(b);
                l.Add(g);
                l.Add(r);
            }
            else
            {
                l.Add(r);
                l.Add(g);
                l.Add(b);
                l.Add(a);
            }
        });

        return l;
    }

    public static string ListToCArray(List<byte> data, Image<Rgba32> img, int rotate, bool reverseBits, bool isSmallEndian,
        int mode, int subMode)
    {
        var sb = new StringBuilder();
        if(Utils.Settings.Language.Contains("ZH", StringComparison.CurrentCultureIgnoreCase))
            sb.Append("/*@注意：此文件由Image2Display生成，请保留此注释以便下次导入图片 */\n");
        else
            sb.Append("/*@Notice: This file is generated by Image2Display, keep this comment to import image next time */\n");
        sb.Append($"/*@Size: {img.Width}x{img.Height}, " +
            $"{rotate}{(reverseBits?1:0)}{(isSmallEndian?1:0)}{mode}{subMode} */\n");
        sb.Append("const uint8_t data[] = {\n");
        var count = 0;
        foreach (var item in data)
        {
            sb.Append($"0x{item:X2},");
            count++;
            if (count == 16)
            {
                sb.Append('\n');
                count = 0;
            }
        }
        sb.Append("};\n");
        return sb.ToString();
    }

    public static string ColorListToCArray(List<Rgba32> colors)
    {
        var sb = new StringBuilder();
        if (Utils.Settings.Language.Contains("ZH", StringComparison.CurrentCultureIgnoreCase))
            sb.Append("/*@注意：此文件由Image2Display生成，请保留此注释以便下次导入图片 */\n");
        else
            sb.Append("/*@Notice: This file is generated by Image2Display, keep this comment to import image next time */\n");
        sb.Append($"/*@Colors: {colors.Count}, RGBA */\n");
        sb.Append("const uint8_t colors[] = {\n");
        //每行一个颜色，每个颜色4个字节，分别是RGBA
        foreach (var color in colors)
        {
            sb.Append($"0x{color.R:X2},0x{color.G:X2},0x{color.B:X2},0x{color.A:X2},");
            sb.Append('\n');
        }
        sb.Append("};\n");
        return sb.ToString();
    }
}
