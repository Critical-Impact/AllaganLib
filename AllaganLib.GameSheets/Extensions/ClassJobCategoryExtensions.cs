using Lumina.Excel.Sheets;

namespace AllaganLib.GameSheets.Extensions;

public static class ClassJobCategoryExtensions
{
    extension(ClassJobCategory classJobCategory)
    {
        public bool BST => classJobCategory.Unknown0;
    }
}