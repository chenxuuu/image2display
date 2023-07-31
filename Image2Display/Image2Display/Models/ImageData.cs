using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image2Display.Models
{
    public class ImageData
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

        /// <summary>
        /// 调整图片大小（会被拉伸）
        /// </summary>
        /// <param name="width">目标宽度</param>
        /// <param name="height">目标高度</param>
        /// <returns>是否成功</returns>
        public bool Resize(int width, int height)
        {
            if (width < 0 || height < 0)
                return false;
            Raw.Mutate(ctx => ctx.Resize(width, height));
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
    }
}
