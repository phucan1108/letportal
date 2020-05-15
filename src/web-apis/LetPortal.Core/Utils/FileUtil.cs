namespace LetPortal.Core.Utils
{
    public static class FileUtil
    {
        public static string GetFileNameWithoutExt(string fileName)
        {
            return fileName.Substring(0, fileName.LastIndexOf(".", System.StringComparison.Ordinal));
        }

        public static string GetExtension(string fileName)
        {
            return fileName.Split(".")[^1];
        }
    }
}
