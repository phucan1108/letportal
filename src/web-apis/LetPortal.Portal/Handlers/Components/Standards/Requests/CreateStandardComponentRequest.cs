using LetPortal.Portal.Handlers.Components.Standards.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Components.Standards.Requests
{
    public class CreateStandardComponentRequest : IRequest<string>
    {
        private readonly CreateStandardComponentCommand _createStandardComponentCommand;

        public CreateStandardComponentRequest(CreateStandardComponentCommand createStandardComponentCommand)
        {
            _createStandardComponentCommand = createStandardComponentCommand;
        }

        public CreateStandardComponentCommand GetCommand()
        {
            return _createStandardComponentCommand;
        }
    }
}
