using LetPortal.CMS.Core.Abstractions;

namespace LetPortal.CMS.Core.Implements
{
    public class SiteRequestAccessor : ISiteRequestAccessor
    {
        private SiteContext _context;

        public SiteRequestAccessor()
        {
            _context = new SiteContext();
        }

        public SiteContext Current => _context;
    }
}
