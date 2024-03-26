using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using Size = System.Windows.Size;

namespace ImageSorter.Extensions;

public static class ImageProcessingContextExtensions
{
    public static IImageProcessingContext ApplyScalingWaterMarkSimple(this IImageProcessingContext processingContext,
        string text, Color color, Size containerSize)
    {
        var imgSize = processingContext.GetCurrentSize();
        var font = SystemFonts.Get("Verdana").CreateFont(32f, FontStyle.Regular);
        var scalingFactor = (float) Math.Max(imgSize.Width / containerSize.Width, imgSize.Height / containerSize.Height);
        var scaledFont = new Font(font, scalingFactor * font.Size);
        var scaledSize = TextMeasurer.MeasureSize(text, new TextOptions(scaledFont));
        var center = new PointF(imgSize.Width / 2f - scaledSize.Width / 2f, imgSize.Height / 2f - scaledSize.Height / 2f);
        var textGraphicOptions = new DrawingOptions();
        return processingContext.DrawText(textGraphicOptions, text, scaledFont, color, center);
    }
}