# (WIP)Image2display

[Chinese](./README.md) | English

<p align="center">
    <br>
    <img src="./Image2Display/Image2Display/Assets/logo.svg" width="150"/>
    <br>
</p>
<p align="center">
    <img alt="GitHub" src="https://img.shields.io/github/license/chenxuuu/image2display">
    <img alt="GitHub release (latest by date)" src="https://img.shields.io/github/v/release/chenxuuu/image2display">
    <img alt="GitHub top language" src="https://img.shields.io/github/languages/top/chenxuuu/image2display">
    <img alt="GitHub Test" src="https://github.com/chenxuuu/image2display/actions/workflows/test.yml/badge.svg">
</p>

A cross-platform image and font data processing tool for generating data usable by microcontrollers.

Supports Win10+, Linux, OSX

![image2display](Assets/en.gif)

## Download

All official versions: [GitHub Releases](https://github.com/chenxuuu/image2display/releases/latest)

CI snapshot version: [GitHub Action](https://nightly.link/chenxuuu/image2display/workflows/build/master)

## Features

- Image Preprocessing
  - Pre-crop, scale, and rotate images
  - Adjust image brightness, contrast, saturation, and replace transparent backgrounds
  - Binarize images with customizable thresholds and dithering algorithms
  - Quantize colors to reduce the number of palette colors, with support for custom color quantization dithering algorithms
- Image Modulation Export
  - Customize modulation methods, supporting palette and formats like RGB565, RGB888, ARGB8888, etc.
  - Customize traversal methods, supporting left-to-right, top-to-bottom, right-to-left, bottom-to-top, etc.
  - Export data endianness, supporting big-endian and little-endian
  - Bit order within colors, supporting reverse order
  - Support export as C language arrays and Bin files
- Font Processing
  - TBD
- Font Modulation Export
  - TBD
