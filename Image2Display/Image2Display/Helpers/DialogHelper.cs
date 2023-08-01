using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls.Primitives;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia;
using Image2Display.ViewModels;
using Avalonia.Platform.Storage;

namespace Image2Display.Helpers
{
    public class DialogHelper
    {
        // from
        // https://github.com/amwx/FluentAvalonia/blob/98d1b8c7fdac9dd82b483ed82979b25fd0e9ff8a/samples/FAControlsGallery/Services/DialogHelper.cs
        public static async Task ShowUnableToOpenLinkDialog(Uri uri)
        {
            var copyLinkButton = new TaskDialogCommand
            {
                Text = Utils.GetI18n<string>("CopyLink"),
                IconSource = new SymbolIconSource { Symbol = Symbol.Link },
                Description = uri.ToString(),
                ClosesOnInvoked = false
            };

            var td = new TaskDialog
            {
                Content = Utils.GetI18n<string>("OpenLinkFail"),
                SubHeader = Utils.GetI18n<string>("OhNo"),
                Commands =
            {
                copyLinkButton
            },
                Buttons =
            {
                TaskDialogButton.OKButton
            },
                IconSource = new SymbolIconSource { Symbol = Symbol.ImportantFilled }
            };

            copyLinkButton.Click += async (s, __) =>
            {
                TopLevel Owner = TopLevel.GetTopLevel(td.XamlRoot)!;
                await Owner.Clipboard!.SetTextAsync(uri.ToString());

                var flyout = new Flyout
                {
                    Content = Utils.GetI18n<string>("Copied")
                };

                var comHost = td.FindDescendantOfType<TaskDialogCommandHost>()!;

                FlyoutBase.SetAttachedFlyout(comHost, flyout);
                FlyoutBase.ShowAttachedFlyout(comHost);

                DispatcherTimer.RunOnce(() => flyout.Hide(), TimeSpan.FromSeconds(1));
            };

            var app = Application.Current!.ApplicationLifetime;
            if (app is IClassicDesktopStyleApplicationLifetime desktop)
            {
                td.XamlRoot = desktop.MainWindow;
            }
            else if (app is ISingleViewApplicationLifetime single)
            {
                td.XamlRoot = TopLevel.GetTopLevel(single.MainView);
            }

            await td.ShowAsync(true);
        }

        /// <summary>
        /// 显示一个提示框
        /// </summary>
        /// <param name="Title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="symbol">符号，默认警告符号</param>
        /// <returns>Task</returns>
        public static async Task ShowNotifyDialog(string Title, string content, Symbol symbol = Symbol.Alert)
        {
            var td = new TaskDialog
            {
                Content = content,
                SubHeader = Title,
                Buttons =
                {
                    TaskDialogButton.OKButton
                },
                IconSource = new SymbolIconSource { Symbol = symbol }
            };

            var app = Application.Current!.ApplicationLifetime;
            if (app is IClassicDesktopStyleApplicationLifetime desktop)
            {
                td.XamlRoot = desktop.MainWindow;
            }
            else if (app is ISingleViewApplicationLifetime single)
            {
                td.XamlRoot = TopLevel.GetTopLevel(single.MainView);
            }

            await td.ShowAsync(true);
        }

        /// <summary>
        /// 打开文件选择框
        /// </summary>
        /// <param name="type">已知的FilePickerFileType类型枚举</param>
        /// <param name="multiple">是否允许多选文件，默认禁止</param>
        /// <returns>文件列表</returns>
        public static async Task<IReadOnlyList<IStorageFile>> ShowOpenFileDialogAsync
        (
            FilePickerFileType type,
            bool multiple = false
        )
        {
            var options = new FilePickerOpenOptions
            {
                AllowMultiple = multiple,
                FileTypeFilter = new List<FilePickerFileType> { type }
            };
            var app = Application.Current!.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var m = app!.MainWindow;
            return await m!.StorageProvider.OpenFilePickerAsync(options);
        }

        /// <summary>
        /// 打开文件选择框
        /// </summary>
        /// <param name="explain">文件类型解释</param>
        /// <param name="types">文件拓展名列表，如"*.txt"</param>
        /// <param name="multiple">是否允许多选文件，默认禁止</param>
        /// <returns>文件列表</returns>
        public static async Task<IReadOnlyList<IStorageFile>> ShowOpenFileDialogAsync
        (
            string explain,
            IEnumerable<string> types,
            bool multiple = false
        )
        {
            FilePickerFileType list = new(explain)
            {
                Patterns = types.ToArray(),
                AppleUniformTypeIdentifiers = new[] { "public.image" },
                MimeTypes = new[] { "application/octet-stream" }
            };
            return await ShowOpenFileDialogAsync(list, multiple);
        }
    }
}
