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
        public ImageConvertViewModel()
        {
            if (Utils.ImageDataTemp != null)
            {
                Image = Utils.ImageDataTemp;
                Utils.ImageDataTemp = null;
                ImportImage();
            }
            Utils.ImportImageAction = (img) =>
            {
                Image = img;
                ImportImage();
            };
        }

        private ImageData? Image = null;


        /// <summary>
        /// 导入图片后，获取信息
        /// </summary>
        private void ImportImage()
        {
            if(Image == null)
            {
                Debug.WriteLine("Image is null");
                return;
            }
            var clolrList = Image.GetColors();
            Debug.WriteLine($"Color Count: {clolrList.Count}");
        }






        [RelayCommand]
        private void Test()
        {
            if (Image == null)
            {
                Debug.WriteLine("Image is null");
                return;
            }
            Debug.WriteLine($"{Image.Width},{Image.Height}");
        }
    }
}
