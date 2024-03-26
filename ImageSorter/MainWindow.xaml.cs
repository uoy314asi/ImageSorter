using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ImageSorter.Extensions;
using ImageSorter.Sorters;
using Microsoft.Win32;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Image = System.Windows.Controls.Image;
using Picture = SixLabors.ImageSharp.Image;
using SystemFonts = SixLabors.Fonts.SystemFonts;

namespace ImageSorter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IImageCategoryProvider _imageCategoryProvider;
    private readonly IImageTransfer _imageMover;
    private string _outputFolderPath = string.Empty;
    private bool _isImageLoading;

    public MainWindow()
    {
        _imageCategoryProvider = new ClarifAiCategoryProvider(new HttpClient());
        _imageMover = new ImageTransfer();
        _isImageLoading = false;
        InitializeComponent();
    }

    private async void SelectImages_OnClick(object sender, RoutedEventArgs e)
    {
        var selectDialog = new OpenFileDialog
        {
            Multiselect = true,
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = "Image Files(*.JPG;)|;*.JPG;"
        };
        if (selectDialog.ShowDialog() ?? false)
        {
            await TriggerDefineImagesCategoryAsync(selectDialog.FileNames);
        }
    }

    private async void ImagesContainer_OnDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is not string[] filePaths) return;
        await TriggerDefineImagesCategoryAsync(filePaths);
    }

    private async Task<string> DrawImageWithTextAsync(string imagePath, string imageText, CancellationToken cancellationToken = default)
    {
        using var image = await Picture.LoadAsync(imagePath, cancellationToken);
        var (width, height) = (image.Width * 0.3f, image.Height * 0.2f);
        var (positionX, positionY) = (image.Width / 2f - width / 2f, image.Height / 2f - height / 2f);
        image.Mutate(context =>
        {
            var rect = new RectangleF
            {
                Width = width,
                Height = height,
                X = positionX,
                Y = positionY
            };
            var brush = new RecolorBrush(Color.DarkGreen, Color.Transparent, 0.9f);
            context.Fill(brush, rect).ApplyScalingWaterMarkSimple(imageText, Color.White, ImagesContainer.RenderSize);
        });
        return image.ToBase64String(JpegFormat.Instance).Split(",").Last();
    }
    
    private async Task TriggerDefineImagesCategoryAsync(IReadOnlyCollection<string> imagePaths, CancellationToken cancellationToken = default)
    {
        if (_isImageLoading)
        {
            MessageBox.Show("Image is loading currently please wait", "Image loading", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (string.IsNullOrEmpty(_outputFolderPath))
        {
            MessageBox.Show("Select output folder before upload images");
            return;
        }

        _isImageLoading = true;
        ImageLoadingProgressBar.Maximum = imagePaths.Count;
        ImageLoadingProgressBar.Value = 0;
        ReadyImagesPrinter.Content = $"Ready {ImageLoadingProgressBar.Value}/{ImageLoadingProgressBar.Maximum} ({Math.Round(ImageLoadingProgressBar.Value / ImageLoadingProgressBar.Maximum, 3) * 100}%)";
        ReadyImagesPrinter.Visibility = Visibility.Visible;
        foreach (var path in imagePaths)
        {
            var category = await _imageCategoryProvider.DefineCategoryAsync(path, cancellationToken);
            var imageData = await DrawImageWithTextAsync(path, category, cancellationToken);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(Convert.FromBase64String(imageData));
            image.EndInit();
            var item = new ListBoxItem
            {
                Content = new Image
                {
                    Source = image
                }
            };
            ImagesContainer.Items.Add(item);
            _imageMover.CopyImage(path, _outputFolderPath, category);
            ImageLoadingProgressBar.Value += 1;
            ReadyImagesPrinter.Content = $"Ready {ImageLoadingProgressBar.Value}/{ImageLoadingProgressBar.Maximum} ({Math.Round(ImageLoadingProgressBar.Value / ImageLoadingProgressBar.Maximum, 3) * 100}%)";
        }

        _isImageLoading = false;
        ImageLoadingProgressBar.Value = 0;
        ImageLoadingProgressBar.Maximum = 1;
    }

    private void SelectOutputFolder_OnClick(object sender, RoutedEventArgs e)
    {
        var selectFolderDialog = new OpenFolderDialog
        {
            Multiselect = false
        };
        if (selectFolderDialog.ShowDialog() ?? false)
        {
            _outputFolderPath = selectFolderDialog.FolderName;
        }
    }
}