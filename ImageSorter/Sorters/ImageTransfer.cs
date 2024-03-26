using System.IO;

namespace ImageSorter.Sorters;

public class ImageTransfer : IImageTransfer
{
    public void CopyImage(string sourcePath, string destFolder, string subFolder)
    {
        var destPath = $"{destFolder}/{subFolder}";
        if (!Directory.Exists(destPath))
        {
            Directory.CreateDirectory(destPath);
        }

        File.Copy(sourcePath, $"{destPath}/{Path.GetFileName(sourcePath)}");
    }
}