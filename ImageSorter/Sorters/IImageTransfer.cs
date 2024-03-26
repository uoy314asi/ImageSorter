namespace ImageSorter.Sorters;

public interface IImageTransfer
{
    void CopyImage(string sourcePath, string destFolder, string subFolder);
}