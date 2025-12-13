using System;
using System.Threading.Tasks;
using LetPortal.Core.Tools;

namespace LetPortal.Tools.Features
{
    class Patch : IFeatureCommand
    {
        public string CommandName => "patch";

        public Task RunAsync(object context)
        {
            throw new NotImplementedException();
        }
    }
}
