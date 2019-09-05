namespace LetPortal.Core.Versions
{
    public static class VersionExtensions
    {
        public static int GetNumber(this IVersion version)
        {
            return version.VersionNumber.ToVersionNumber();
        }

        public static int ToVersionNumber(this string versionNumber)
        {
            var splitted = versionNumber.Split(".");
            string majorNumber = splitted[0];
            string minorNumber = splitted[1].Length == 1 ? "00" + splitted[1] : splitted[1].Length == 2 ? "0" + splitted[1] : splitted[1];
            string patchNumber = splitted[0].Length == 1 ? "00" + splitted[0] : splitted[0].Length == 2 ? "0" + splitted[0] : splitted[0];
            string number = majorNumber + minorNumber + patchNumber;
            return int.Parse(number);
        }
    }
}
