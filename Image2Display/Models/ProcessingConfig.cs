
using System;

namespace Image2Display.Models;
public class ImageProcessingConfig
{
    // 灰度处理
    public enum GrayscaleMode
    {
        NoProcessing,
        Monochrome,
        FourGrayLevels,
        SixteenGrayLevels,
        EightGrayLevels, // 3-bit
        ThirtyTwoGrayLevels // 5-bit
    }

    // 彩色色彩制式
    public enum ColorMode
    {
        Grayscale,
        Color256,
        Color4096,
        Color16Bit,
        Color24Bit,
        Color32Bit
    }

    // 抖动算法
    public enum DitheringAlgorithm
    {
        NoDithering,
        ErrorDiffusion,
        BlueNoise,
        // 可以添加更多抖动算法
    }

    // 图像修改操作
    [Flags]
    public enum ImageModification
    {
        None = 0,
        Rotate = 1,
        Scale = 2,
        Crop = 4,
        AddWatermark = 8,
        AddBorder = 16,
        AddBlankArea = 32
    }

    // 导出格式
    public enum ExportFormat
    {
        CArray,
        BinaryFile
    }

    // 属性
    public GrayscaleMode Grayscale { get; set; }
    public ColorMode Color { get; set; }
    public DitheringAlgorithm Dithering { get; set; }
    public ImageModification Modifications { get; set; }
    public ExportFormat ExportAs { get; set; }
}
