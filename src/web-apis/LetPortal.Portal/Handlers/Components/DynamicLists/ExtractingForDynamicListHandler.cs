using LetPortal.Portal.Handlers.Components.DynamicLists.Requests;
using LetPortal.Portal.Models.DynamicLists;
using LetPortal.Portal.Providers.Databases;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.DynamicLists
{
    public class ExtractingForDynamicListHandler : IRequestHandler<ExtractingQueryForDynamicListRequest, PopulateQueryModel>
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        public ExtractingForDynamicListHandler(IDatabaseServiceProvider databaseServiceProvider)
        {
            _databaseServiceProvider = databaseServiceProvider;
        }

        public async Task<PopulateQueryModel> Handle(ExtractingQueryForDynamicListRequest request, CancellationToken cancellationToken)
        {
            var query = request.GetQuery().Query;
            foreach(var param in request.GetQuery().Parameters)
            {
                query = query.Replace("{{" + param.Name + "}}", param.Value);
            }

            var extractingResult = await _databaseServiceProvider.GetSchemasByQuery(request.GetQuery().DatabaseId, query);

            return new PopulateQueryModel { ColumnFields = extractingResult.ColumnFields };
        }
    }
}
