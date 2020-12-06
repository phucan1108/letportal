using LetPortal.Core.Exceptions;

namespace LetPortal.CMS.Core.Exceptions
{
    public static class ErrorCodes
    {
        public static readonly ErrorCode TheAssemblyNotIncludeThemeRegistration = new ErrorCode
        {
            MessageCode = "CMS000000",
            MessageContent = "The assembly doesn't include any implemented IThemeRegistration"
        };

        public static readonly ErrorCode NotFoundSiteRoute = new ErrorCode
        {
            MessageCode = "CMS000001",
            MessageContent = "Not found any matched site route"
        };

        public static readonly ErrorCode NotFoundSite = new ErrorCode
        {
            MessageCode = "CMS000002",
            MessageContent = "This site isn't avaiblable, please contact with admin"
        };

        public static readonly ErrorCode NotImplementPageManifest = new ErrorCode
        {
            MessageCode = "CMS000003",
            MessageContent = "The target object isn't implemented IPageManifest"
        };
    }
}
