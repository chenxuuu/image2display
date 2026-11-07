using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using Avalonia.Media;
using Image2Display.Helpers;
using Image2Display.ViewModels;
using System.Linq;

namespace I2D_Test
{
    /// <summary>
    /// issue #8「闪退」的回归测试。
    ///
    /// 复现条件：系统字体下拉框每一项都会把字体族名当预览字体塞进 FontFamily，
    /// 一旦族名里出现连续两个空格（例如某些字体的族名），Avalonia 的
    /// FontCollectionBase.GetImplicitTypeface → SpanStringTokenizer.SkipToNextToken
    /// 会在 TextBlock.MeasureOverride 里抛 FormatException，直接闪退。
    /// </summary>
    public class SystemFontListTest
    {
        /// <summary>
        /// 把下拉框里每一项真的布局一遍：不给预览字体也得能测，
        /// 给了预览字体也不能炸（本机会装一个族名带两个空格的测试字体来覆盖这条路径）。
        /// </summary>
        [AvaloniaFact]
        public void SystemFontItemsCanBeMeasured()
        {
            Image2Display.Utils.Initial();
            var vm = new FontConvertViewModel();
            Assert.NotEmpty(vm.SystemFontList);

            foreach (var item in vm.SystemFontList)
            {
                var tb = new TextBlock { Text = item.Content as string };
                if (item.FontFamily is not null)
                    tb.FontFamily = item.FontFamily;
                tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
        }

        /// <summary>
        /// 安全检查不能把预览功能整体砍掉：正常字体还是要带上自己的预览字体。
        /// </summary>
        [AvaloniaFact]
        public void SystemFontItemsKeepPreviewWhenPossible()
        {
            Image2Display.Utils.Initial();
            var vm = new FontConvertViewModel();
            Assert.NotEmpty(vm.SystemFontList);
            Assert.Contains(vm.SystemFontList, i => i.FontFamily is not null);
        }
    }

    public class FontFamilyHelperTest
    {
        /// <summary>
        /// 族名里带连续空格时，Avalonia 自己解析会抛 FormatException（见 issue #8 的调用栈），
        /// 助手必须把它挡下来，返回 null 而不是往外抛。
        /// </summary>
        [AvaloniaFact]
        public void RepeatedSpaceFamilyNameIsRejected()
        {
            var family = Record.Exception(() => FontFamilyHelper.CreatePreviewFontFamily("Datang  DoubleSpace"));
            Assert.Null(family);
            Assert.Null(FontFamilyHelper.CreatePreviewFontFamily("Datang  DoubleSpace"));
            Assert.Null(FontFamilyHelper.CreatePreviewFontFamily("A   B"));
            Assert.Null(FontFamilyHelper.CreatePreviewFontFamily(null));
            Assert.Null(FontFamilyHelper.CreatePreviewFontFamily("   "));
        }

        /// <summary>
        /// 普通族名（单个空格、无空格）要继续给出预览字体。
        /// </summary>
        [AvaloniaFact]
        public void NormalFamilyNameKeepsWorking()
        {
            Assert.NotNull(FontFamilyHelper.CreatePreviewFontFamily("A B"));
            Assert.NotNull(FontFamilyHelper.CreatePreviewFontFamily("SomeFont"));
            // 首尾空格 FontFamily 自己会 Trim，不会走到抛异常那条路
            Assert.NotNull(FontFamilyHelper.CreatePreviewFontFamily(" A B "));
        }
    }
}
