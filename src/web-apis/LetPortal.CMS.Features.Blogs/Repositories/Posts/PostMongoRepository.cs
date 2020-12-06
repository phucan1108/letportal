using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Shared;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.Core.Logger;
using LetPortal.Core.Persistences;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.CMS.Features.Blogs.Repositories.Posts
{
    public class PostMongoRepository : MongoGenericRepository<Post>, IPostRepository
    {
        private readonly IServiceLogger<PostMongoRepository> _logger;
        public PostMongoRepository(
            MongoConnection mongoConnection,
            IServiceLogger<PostMongoRepository> logger)
        {
            Connection = mongoConnection;
            _logger = logger;
        }

        public async Task<Post> GetByUrlPathAsync(string urlPath)
        {
            _logger.Info("Get post by url path " + urlPath);
            return await Collection.AsQueryable().FirstAsync(a => a.UrlPath == urlPath);
        }

        public async Task<PaginationData<Post>> GetPostsByBlogIdAndPaginationAsync(
            string blogId,
            Pagination pagination,
            bool ascending = false)
        {
            var filter = Builders<Post>.Filter.Eq(a => a.BlogId, blogId);
            var aggregateFluent = Collection.Aggregate();

            var totalFacet = AggregateFacet.Create("total",
            PipelineDefinition<Post, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<Post>()
            }));

            AggregateFacet<Post, Post> dataFacet;

            if (ascending)
            {
                dataFacet = AggregateFacet.Create("data",
                    PipelineDefinition<Post, Post>.Create(new[]
                    {
                        PipelineStageDefinitionBuilder.Sort(Builders<Post>.Sort.Ascending(x => x.CreatedDate)),
                        PipelineStageDefinitionBuilder.Skip<Post>(pagination.NumberPerPage * (pagination.CurrentPage -  1)),
                        PipelineStageDefinitionBuilder.Limit<Post>(pagination.NumberPerPage),
                    }));

            }
            else
            {
                dataFacet = AggregateFacet.Create("data",
                    PipelineDefinition<Post, Post>.Create(new[]
                    {
                        PipelineStageDefinitionBuilder.Sort(Builders<Post>.Sort.Descending(x => x.CreatedDate)),
                        PipelineStageDefinitionBuilder.Skip<Post>(pagination.NumberPerPage * (pagination.CurrentPage -  1)),
                        PipelineStageDefinitionBuilder.Limit<Post>(pagination.NumberPerPage),
                    }));
            }


            var aggregation = await aggregateFluent
                                    .Match(filter)
                                    .Facet(totalFacet, dataFacet)
                                    .ToListAsync();

            var total = aggregation
                            .First()
                            .Facets.First(a => a.Name == "total")
                            .Output<AggregateCountResult>()
                            .First()
                            .Count;

            var data = aggregation
                            .First()
                            .Facets.First(a => a.Name == "data")
                            .Output<Post>();

            return new PaginationData<Post>(data, total, pagination.CurrentPage, pagination.NumberPerPage, pagination.MaximumPage);
        }

        public async Task<PaginationData<Post>> GetPostsByTags(IEnumerable<string> tags, Pagination pagination, bool ascending = false)
        {
            var filter = Builders<Post>.Filter.AnyIn(a => a.Tags, tags);
            var aggregateFluent = Collection.Aggregate();

            var totalFacet = AggregateFacet.Create("total",
            PipelineDefinition<Post, AggregateCountResult>.Create(new[]
            {
                PipelineStageDefinitionBuilder.Count<Post>()
            }));

            AggregateFacet<Post, Post> dataFacet;

            if (ascending)
            {
                dataFacet = AggregateFacet.Create("data",
                    PipelineDefinition<Post, Post>.Create(new[]
                    {
                        PipelineStageDefinitionBuilder.Sort(Builders<Post>.Sort.Ascending(x => x.CreatedDate)),
                        PipelineStageDefinitionBuilder.Skip<Post>(pagination.NumberPerPage * (pagination.CurrentPage -  1)),
                        PipelineStageDefinitionBuilder.Limit<Post>(pagination.NumberPerPage),
                    }));

            }
            else
            {
                dataFacet = AggregateFacet.Create("data",
                    PipelineDefinition<Post, Post>.Create(new[]
                    {
                        PipelineStageDefinitionBuilder.Sort(Builders<Post>.Sort.Descending(x => x.CreatedDate)),
                        PipelineStageDefinitionBuilder.Skip<Post>(pagination.NumberPerPage * (pagination.CurrentPage -  1)),
                        PipelineStageDefinitionBuilder.Limit<Post>(pagination.NumberPerPage),
                    }));
            }


            var aggregation = await aggregateFluent
                                    .Match(filter)
                                    .Facet(totalFacet, dataFacet)
                                    .ToListAsync();

            var total = aggregation
                            .First()
                            .Facets.First(a => a.Name == "total")
                            .Output<AggregateCountResult>()
                            .First()
                            .Count;

            var data = aggregation
                            .First()
                            .Facets.First(a => a.Name == "data")
                            .Output<Post>();

            return new PaginationData<Post>(data, total, pagination.CurrentPage, pagination.NumberPerPage, pagination.MaximumPage);
        }
    }
}
