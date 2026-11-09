using Avalonia.Media;
using System;

namespace Image2Display.Helpers
{
    /// <summary>
    /// 处理「系统字体下拉框里显示预览字体」这件事上的族名安全。
    ///
    /// Avalonia 11 的 Typeface 会把字体族名里的空白当成「字重/字形」的分隔符解析
    /// （<c>FontCollectionBase.GetImplicitTypeface()</c> 用 ' ' 作分隔符切族名），
    /// 只要族名里出现连续两个空格，<c>SpanStringTokenizer.SkipToNextToken()</c> 就会抛
    /// FormatException。这个异常发生在 <c>TextBlock</c> 的 MeasureOverride 里（测量下拉框每一项时），
    /// 没有任何地方接住，直接闪退（issue #8）。
    ///
    /// 所以把族名交给 FontFamily 之前必须先体检：解析不了、或者解析出来 Avalonia 根本配不到字形，
    /// 就宁可不给这项设预览字体（退回默认字体显示），也不能让布局阶段炸掉。
    /// </summary>
    public static class FontFamilyHelper
    {
        /// <summary>
        /// 尝试把 Skia 给出的字体族名变成可以安全塞进 TextBlock.FontFamily 的对象。
        /// 不安全的族名返回 null（调用方保持默认字体即可）。
        /// </summary>
        /// <param name="familyName">字体族名，来自 <c>SKFontManager.FontFamilies</c></param>
        /// <returns>可用的 FontFamily；不可用时返回 null</returns>
        public static FontFamily? CreatePreviewFontFamily(string? familyName)
        {
            if (string.IsNullOrWhiteSpace(familyName))
                return null;

            try
            {
                var family = new FontFamily(familyName);
                // 真解析一次字形：既会踩到上面那个 FormatException，
                // 也能挡掉「Avalonia 认不出这个名字」的字体（那样预览出来的也不是这个字体）。
                if (!FontManager.Current.TryGetGlyphTypeface(new Typeface(family), out _))
                    return null;
                return family;
            }
            catch (Exception)
            {
                // 这里是布局前的最后一道闸门，宁可少一个预览，不能闪退。
                return null;
            }
        }
    }
}
