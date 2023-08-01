using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.Input;
using Image2Display.Helpers;
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
        [RelayCommand]
        private async Task ImportImage()
        {
            var r = await DialogHelper.ShowOpenFileDialogAsync(FilePickerFileTypes.ImageAll);
            if (r is null)
                return;

            var file = r.First();
            //todo
            Debug.WriteLine($"ImportImage, {file.Path}");
        }
    }
}
