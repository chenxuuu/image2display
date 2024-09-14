

using Avalonia.Media;
using System;

namespace Image2Display.Helpers;

public class ByteOrder
{
    private static readonly SolidColorBrush GrayColor = new(new Color(0x7f, 0x7f, 0x7f, 0x7f));
    private static readonly SolidColorBrush RedColor = new(new Color(0x7f, 0xff, 0x00, 0x00));
    private static readonly SolidColorBrush GreenColor = new(new Color(0x7f, 0x00, 0xff, 0x00));
    private static readonly SolidColorBrush BlueColor = new(new Color(0x7f, 0x00, 0x00, 0xff));
    private static readonly SolidColorBrush YellowColor = new(new Color(0x7f, 0xff, 0xff, 0xff));
    private static readonly SolidColorBrush TransparentColor = new(new Color(0x00, 0x00, 0x00, 0x00));

    private static readonly SolidColorBrush EC = GrayColor;
    private static readonly SolidColorBrush RC = RedColor;
    private static readonly SolidColorBrush GC = GreenColor;
    private static readonly SolidColorBrush BC = BlueColor;
    private static readonly SolidColorBrush YC = YellowColor;
    private static readonly SolidColorBrush TC = TransparentColor;

    public static SolidColorBrush[] GetOrderColors(int colorMode, int fullColor, int byteOrder)
    {
        SolidColorBrush[] r;
        if (colorMode == 1)//调色板
        {
            r = [
                EC, EC, EC, EC, EC, EC, EC, EC,
                EC, EC, EC, EC, EC, EC, EC, EC,
                EC, EC, EC, EC, EC, EC, EC, EC,
                EC, EC, EC, EC, EC, EC, EC, EC,
                ];
        }
        else//全彩色
        {
            r = fullColor switch
            {
                0 => [
                        RC,RC,RC,RC, GC,GC,GC,GC,
                        BC,BC,BC,BC, RC,RC,RC,RC,
                        GC,GC,GC,GC, BC,BC,BC,BC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                    ],
                1 => byteOrder switch
                {
                    0 => [
                        EC,EC,EC,EC, RC,RC,RC,RC,
                        GC,GC,GC,GC, BC,BC,BC,BC, 
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    1 => [
                        GC,GC,GC,GC, BC,BC,BC,BC,
                        EC,EC,EC,EC, RC,RC,RC,RC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    _ => throw new NotImplementedException(),
                },
                2 => byteOrder switch
                {
                    0 => [
                        RC,RC,RC,RC, RC,GC,GC,GC,
                        GC,GC,GC,BC, BC,BC,BC,BC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    1 => [
                        GC,GC,GC,BC, BC,BC,BC,BC,
                        RC,RC,RC,RC, RC,GC,GC,GC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    _ => throw new NotImplementedException(),
                },
                3 => byteOrder switch
                {
                    0 => [
                        EC,RC,RC,RC, RC,RC,GC,GC,
                        GC,GC,GC,BC, BC,BC,BC,BC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    1 => [
                        GC,GC,GC,BC, BC,BC,BC,BC,
                        EC,RC,RC,RC, RC,RC,GC,GC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    _ => throw new NotImplementedException(),
                },
                4 => byteOrder switch
                {
                    0 => [
                        EC,EC,EC,EC, EC,EC,RC,RC,
                        RC,RC,RC,RC, GC,GC,GC,GC,
                        GC,GC,BC,BC, BC,BC,BC,BC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    1 => [
                        GC,GC,BC,BC, BC,BC,BC,BC,
                        RC,RC,RC,RC, GC,GC,GC,GC,
                        EC,EC,EC,EC, EC,EC,RC,RC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    _ => throw new NotImplementedException(),
                },
                5 => byteOrder switch
                {
                    0 => [
                        RC,RC,RC,RC, RC,RC,RC,RC,
                        GC,GC,GC,GC, GC,GC,GC,GC,
                        BC,BC,BC,BC, BC,BC,BC,BC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    1 => [
                        BC,BC,BC,BC, BC,BC,BC,BC,
                        GC,GC,GC,GC, GC,GC,GC,GC,
                        RC,RC,RC,RC, RC,RC,RC,RC,
                        TC,TC,TC,TC, TC,TC,TC,TC,
                        ],
                    _ => throw new NotImplementedException(),
                },
                6 => byteOrder switch
                {
                    0 => [
                        YC,YC,YC,YC, YC,YC,YC,YC,
                        RC,RC,RC,RC, RC,RC,RC,RC,
                        GC,GC,GC,GC, GC,GC,GC,GC,
                        BC,BC,BC,BC, BC,BC,BC,BC,
                        ],
                    1 => [
                        BC,BC,BC,BC, BC,BC,BC,BC,
                        GC,GC,GC,GC, GC,GC,GC,GC,
                        RC,RC,RC,RC, RC,RC,RC,RC,
                        YC,YC,YC,YC, YC,YC,YC,YC,
                        ],
                    _ => throw new NotImplementedException(),
                },
                7 => byteOrder switch
                {
                    0 => [
                        RC,RC,RC,RC, RC,RC,RC,RC,
                        GC,GC,GC,GC, GC,GC,GC,GC,
                        BC,BC,BC,BC, BC,BC,BC,BC,
                        YC,YC,YC,YC, YC,YC,YC,YC,
                        ],
                    1 => [
                        YC,YC,YC,YC, YC,YC,YC,YC,
                        BC,BC,BC,BC, BC,BC,BC,BC,
                        GC,GC,GC,GC, GC,GC,GC,GC,
                        RC,RC,RC,RC, RC,RC,RC,RC,
                        ],
                    _ => throw new NotImplementedException(),
                },
                _ => throw new NotImplementedException(),
            };
        }
        return r;
    }

    public static string[] GetOrderChars(int colorMode, int colorDepth, int fullColor, int byteOrder, int internalOrder)
    {
        string[] r;
        if (colorMode == 1)//调色板
        {
            r = colorDepth switch
            {
                0 => [
                    "0", "0",  "0", "0", "0", "0", "0", "0",
                    "0", "0",  "0", "0", "0", "0", "0", "0",
                    "0", "0",  "0", "0", "0", "0", "0", "0",
                    "0", "0",  "0", "0", "0", "0", "0", "0",
                ],
                1 => internalOrder switch
                {
                    0 => [
                        "1", "0",  "1", "0", "1", "0", "1", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                    ],
                    1 => [
                        "0", "1",  "0", "1", "0", "1", "0", "1",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                    ],
                    _ => throw new NotImplementedException(),
                },
                2 => internalOrder switch
                {
                    0 => [
                        "3", "2",  "1", "0", "3", "2", "1", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                    ],
                    1 => [
                        "0", "1",  "2", "3", "0", "1", "2", "3",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                    ],
                    _ => throw new NotImplementedException(),
                },
                3 => internalOrder switch
                {
                    0 => [
                        "7", "6",  "5", "4", "3", "2", "1", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                    ],
                    1 => [
                        "0", "1",  "2", "3", "4", "5", "6", "7",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                    ],
                    _ => throw new NotImplementedException(),
                },
                4 => internalOrder switch
                {
                    0 => byteOrder switch
                    {
                        0 => [
                            "F", "E",  "D", "C", "B", "A", "9", "8",
                            "7", "6",  "5", "4", "3", "2", "1", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "7", "6",  "5", "4", "3", "2", "1", "0",
                            "F", "E",  "D", "C", "B", "A", "9", "8",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    1 => byteOrder switch
                    {
                        0 => [
                            "0", "1",  "2", "3", "4", "5", "6", "7",
                            "8", "9",  "A", "B", "C", "D", "E", "F",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "8", "9",  "A", "B", "C", "D", "E", "F",
                            "0", "1",  "2", "3", "4", "5", "6", "7",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    _ => throw new NotImplementedException(),
                },
                _ => throw new NotImplementedException(),
            };
        }
        else//全彩色
        {
            r = fullColor switch
            {
                0 => internalOrder switch
                {
                    0 => [
                        "3", "2",  "1", "0", "3", "2", "1", "0",
                        "3", "2",  "1", "0", "3", "2", "1", "0",
                        "3", "2",  "1", "0", "3", "2", "1", "0",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                    ],
                    1 => [
                        "0", "1",  "2", "3", "0", "1", "2", "3",
                        "0", "1",  "2", "3", "0", "1", "2", "3",
                        "0", "1",  "2", "3", "0", "1", "2", "3",
                        "0", "0",  "0", "0", "0", "0", "0", "0",
                    ],
                    _ => throw new NotImplementedException(),
                },
                1 => internalOrder switch
                {
                    0 => byteOrder switch
                    {
                        0 => [
                            "-", "-",  "-", "-", "3", "2", "1", "0",
                            "0", "1",  "2", "3", "0", "1", "2", "3",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "0", "1",  "2", "3", "0", "1", "2", "3",
                            "-", "-",  "-", "-", "3", "2", "1", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    1 => byteOrder switch
                    {
                        0 => [
                            "-", "-",  "-", "-", "0", "1", "2", "3",
                            "0", "1",  "2", "3", "0", "1", "2", "3",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "0", "1",  "2", "3", "0", "1", "2", "3",
                            "-", "-",  "-", "-", "0", "1", "2", "3",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    _ => throw new NotImplementedException(),
                },
                2 => internalOrder switch
                {
                    0 => byteOrder switch
                    {
                        0 => [
                            "4", "3",  "2", "1", "0", "5", "4", "3",
                            "2", "1",  "0", "4", "3", "2", "1", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "2", "1",  "0", "4", "3", "2", "1", "0",
                            "4", "3",  "2", "1", "0", "5", "4", "3",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    1 => byteOrder switch
                    {
                        0 => [
                            "0", "1",  "2", "3", "4", "0", "1", "2",
                            "3", "4",  "5", "0", "1", "2", "3", "4",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "3", "4",  "5", "0", "1", "2", "3", "4",
                            "0", "1",  "2", "3", "4", "0", "1", "2",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    _ => throw new NotImplementedException(),
                },
                3 => internalOrder switch
                {
                    0 => byteOrder switch
                    {
                        0 => [
                            "-", "4",  "3", "2", "1", "0", "4", "3",
                            "2", "1",  "0", "4", "3", "2", "1", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "2", "1",  "0", "4", "3", "2", "1", "0",
                            "-", "4",  "3", "2", "1", "0", "4", "3",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    1 => byteOrder switch
                    {
                        0 => [
                            "-", "0",  "1", "2", "3", "4", "0", "1",
                            "2", "3",  "4", "0", "1", "2", "3", "4",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "2", "3",  "4", "0", "1", "2", "3", "4",
                            "-", "0",  "1", "2", "3", "4", "0", "1",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    _ => throw new NotImplementedException(),
                },
                4 => internalOrder switch
                {
                    0 => byteOrder switch
                    {
                        0 => [
                            "-", "-", "-", "-", "-", "-", "5", "4",
                            "3", "2",  "1", "0", "5", "4", "3", "2",
                            "1", "0",  "5", "4", "3", "2", "1", "0",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "1", "0",  "5", "4", "3", "2", "1", "0",
                            "3", "2",  "1", "0", "5", "4", "3", "2",
                            "-", "-", "-", "-", "-", "-", "5", "4",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    1 => byteOrder switch
                    {
                        0 => [
                            "-", "-", "-", "-", "-", "-", "0", "1",
                            "2", "3",  "4", "5", "0", "1", "2", "3",
                            "4", "5",  "0", "1", "2", "3", "4", "5",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        1 => [
                            "4", "5",  "0", "1", "2", "3", "4", "5",
                            "2", "3",  "4", "5", "0", "1", "2", "3",
                            "-", "-", "-", "-", "-", "-", "0", "1",
                            "0", "0",  "0", "0", "0", "0", "0", "0",
                        ],
                        _ => throw new NotImplementedException(),
                    },
                    _ => throw new NotImplementedException(),
                },
                <= 7 => internalOrder switch
                {
                    0 => [
                            "7", "6", "5", "4", "3", "2", "1", "0",
                            "7", "6", "5", "4", "3", "2", "1", "0",
                            "7", "6", "5", "4", "3", "2", "1", "0",
                            "7", "6", "5", "4", "3", "2", "1", "0",
                        ],
                    1 => [
                            "0", "1", "2", "3", "4", "5", "6", "7",
                            "0", "1", "2", "3", "4", "5", "6", "7",
                            "0", "1", "2", "3", "4", "5", "6", "7",
                            "0", "1", "2", "3", "4", "5", "6", "7",
                        ],
                    _ => throw new NotImplementedException(),
                },
                _ => throw new NotImplementedException(),
            };
        }
        return r;
    }
}
