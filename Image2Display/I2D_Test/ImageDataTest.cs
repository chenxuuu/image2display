using Image2Display.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I2D_Test
{
    public class ImageDataTest
    {
        [Theory]
        [InlineData(1, 1)]
        [InlineData(100, 100)]
        [InlineData(1000, 20)]
        public void NewSize(int w,int h)
        {
            _ = new ImageData(w, h);
        }

        [Theory]
        [InlineData(1, 1,10,10)]
        [InlineData(100, 100, 10, 10)]
        public void CropOk(int x, int y,int w, int h)
        {
            var img = new ImageData(1000, 1000);
            Assert.True(img.Crop(x, y, w, h));
        }
        [Theory]
        [InlineData(100, 100, 10, 10000)]
        [InlineData(1000, 20, 10, 10)]
        public void CropFail(int x, int y, int w, int h)
        {
            var img = new ImageData(1000, 1000);
            Assert.False(img.Crop(x, y, w, h));
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(100, 100)]
        [InlineData(1000, 20)]
        public void Expand(int w, int h)
        {
            var img = new ImageData(1000, 1000);
            var r = img.Expand(w, h);
            if (w > img.Width || h > img.Height)
                Assert.True(r);
            else
                Assert.False(r);
        }
    }
}
