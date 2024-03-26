namespace ImageSorter.Sorters;

public interface IImageCategoryProvider
{
    Task<string> DefineCategoryAsync(string filePath, CancellationToken cancellationToken = default);
}