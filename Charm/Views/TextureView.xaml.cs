﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tiger;
using Tiger.Schema;

namespace Charm;

public partial class TextureView : UserControl
{
    public int CurrentSlice = 0;
    private Texture _currentTexture;

    public TextureView()
    {
        InitializeComponent();
    }

    public void LoadTexture(Texture textureHeader)
    {
        _currentTexture = textureHeader;
        SliceViewer.Visibility = textureHeader.IsVolume() ? Visibility.Visible : Visibility.Collapsed;
        if (!textureHeader.IsVolume())
            CurrentSlice = 0;

        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.StreamSource = textureHeader.IsVolume() ? textureHeader.GetVolumeSlice(CurrentSlice) : textureHeader.GetTexture();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        float widthDivisionRatio = (float)textureHeader.TagData.Width / 800;
        float heightDivisionRatio = (float)textureHeader.TagData.Height / 800;
        float transformRatio = Math.Max(heightDivisionRatio, widthDivisionRatio);
        int imgWidth = (int)Math.Floor(textureHeader.TagData.Width / transformRatio);
        int imgHeight = (int)Math.Floor(textureHeader.TagData.Height / transformRatio);
        bitmapImage.DecodePixelWidth = imgWidth;
        bitmapImage.DecodePixelHeight = imgHeight;
        bitmapImage.EndInit();
        bitmapImage.Freeze();

        TextureDisplayData data = new()
        {
            Hash = textureHeader.Hash,
            Image = bitmapImage,
            Dimensions = $"{textureHeader.GetDimension().GetEnumDescription()}: {textureHeader.TagData.Width}x{textureHeader.TagData.Height}x{textureHeader.TagData.Depth}",
            Format = $"{textureHeader.TagData.GetFormat().ToString()} ({(textureHeader.IsSrgb() ? "Srgb" : "Linear")})",
            CurrentSlice = CurrentSlice + 1,
            SliceAmount = textureHeader.TagData.Depth
        };

        DataContext = data;
    }

    private void Slice_OnBackClicked(object sender, RoutedEventArgs e)
    {
        if (!_currentTexture.IsVolume()) return;

        CurrentSlice = (CurrentSlice - 1 + _currentTexture.TagData.Depth) % _currentTexture.TagData.Depth;
        LoadTexture(_currentTexture);
    }

    private void Slice_OnForwardClicked(object sender, RoutedEventArgs e)
    {
        if (!_currentTexture.IsVolume()) return;

        CurrentSlice = (CurrentSlice + 1) % _currentTexture.TagData.Depth;
        LoadTexture(_currentTexture);
    }

    public void ExportCurrent()
    {
        if (_currentTexture is null)
            return;

        ConfigSubsystem config = CharmInstance.GetSubsystem<ConfigSubsystem>();
        string pkgName = PackageResourcer.Get().GetPackage(_currentTexture.Hash.PackageId).GetPackageMetadata().Name.Split(".")[0];
        string savePath = config.GetExportSavePath() + $"/Textures/{pkgName}";
        Directory.CreateDirectory($"{savePath}/");

        if (FlattenVolume.IsChecked.Value && _currentTexture.IsVolume())
            TextureExtractor.SaveTextureToFile($"{savePath}/{_currentTexture.Hash}", Texture.FlattenVolume(_currentTexture.GetScratchImage(true)));
        else
            _currentTexture.SavetoFile($"{savePath}/{_currentTexture.Hash}", CurrentSlice);
    }

    public struct TextureDisplayData
    {
        public FileHash Hash { get; set; }
        public ImageSource Image { get; set; }
        public string Dimensions { get; set; }
        public string Format { get; set; }

        // 3D textures
        public int CurrentSlice { get; set; }
        public int SliceAmount { get; set; }
    }
}

/// <summary>
/// Generic class for loading textures as ImageSource with a given max width/height
/// </summary>
public static class TextureLoader
{
    public static ImageSource LoadTexture(Texture texture, int maxWidth, int maxHeight)
    {
        if (texture == null)
            return null;

        try
        {
            var image = CreateImage(texture, maxWidth, maxHeight);
            image.Freeze();
            return image;
        }
        catch (Exception) // Rare case where a "not a cubemap cubemap" doesnt want to load in time
        {
            return null;
        }
    }

    private static ImageSource CreateImage(Texture texture, int maxWidth, int maxHeight)
    {
        using var unmanagedStream = texture.IsCubemap()
            ? texture.GetCubemapFace(0)
            : texture.GetTexture();

        float widthRatio = (float)texture.TagData.Width / maxWidth;
        float heightRatio = (float)texture.TagData.Height / maxHeight;
        float scaleRatio = Math.Max(widthRatio, heightRatio);
        int imgWidth = (int)Math.Floor(texture.TagData.Width / scaleRatio);
        int imgHeight = (int)Math.Floor(texture.TagData.Height / scaleRatio);

        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.StreamSource = unmanagedStream;
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.DecodePixelWidth = imgWidth;
        bitmap.DecodePixelHeight = imgHeight;
        bitmap.EndInit();

        return bitmap;
    }
}
