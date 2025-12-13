using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;

namespace LetPortal.CMS.Core.Providers
{
    public interface IThemeProvider
    {
        Task<T> LoadAsync<T>(T target, string siteId);

        Task<Theme> LoadTheme(string themeId);
    }
}
