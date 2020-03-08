using System;
using System.Threading.Tasks;

namespace LetPortal.Tools.Features
{
    class Patch : IFeatureCommand
    {
        public string CommandName => "patch";

        public Task RunAsync(ToolsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
