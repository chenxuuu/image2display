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
        [ObservableProperty]
        private string _testText = "0";


        [RelayCommand]
        private void Test()
        {
            Debug.WriteLine("RelayCommand Test");
            var i = int.Parse(TestText);
            i++;
            TestText = i.ToString();
        }
    }
}
