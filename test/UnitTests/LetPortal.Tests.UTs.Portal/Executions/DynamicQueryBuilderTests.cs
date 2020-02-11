using LetPortal.Portal.Executions;
using System.Collections.Generic;
using Xunit;

namespace LetPortal.Tests.UTs.Portal.Executions
{
    public class DynamicQueryBuilderTests
    {
        [Fact]
        public void Generate_Postgre_Query_With_Nothing_Test()
        {
            // Arrange
            string query = @"Select * From Person";
            DynamicQueryBuilder postgreDynamicQueryBuilder = new DynamicQueryBuilder();
            // Act
            DynamicQuery dynamicQuery = postgreDynamicQueryBuilder
                    .Init(
                        query,
                        new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>
                        {
                        })
                    .Build();
            // Assert
            Assert.NotNull(dynamicQuery.CombinedQuery);
        }

        [Fact]
        public void Generate_Postgre_Query_With_Filled_Params_Test()
        {
            // Arrange
            string query = @"Select * From Person Where Name={{data.name}}";
            DynamicQueryBuilder postgreDynamicQueryBuilder = new DynamicQueryBuilder();
            // Act
            DynamicQuery dynamicQuery = postgreDynamicQueryBuilder
                    .Init(
                        query,
                        new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>
                        {
                            new LetPortal.Portal.Models.DynamicLists.FilledParameter
                            {
                                Name = "data.name",
                                Value = "aaa"
                            }
                        })
                    .Build();
            // Assert
            Assert.NotEmpty(dynamicQuery.Parameters);
        }

        [Fact]
        public void Generate_Postgre_Query_With_Filled_Params_And_Search_Test()
        {
            // Arrange
            string query = @"Select * From Person Where Name={{data.name}}";
            DynamicQueryBuilder postgreDynamicQueryBuilder = new DynamicQueryBuilder();
            // Act
            DynamicQuery dynamicQuery = postgreDynamicQueryBuilder
                    .Init(
                        query,
                        new List<LetPortal.Portal.Models.DynamicLists.FilledParameter>
                        {
                            new LetPortal.Portal.Models.DynamicLists.FilledParameter
                            {
                                Name = "data.name",
                                Value = "aaa"
                            }
                        })
                    .Build();
            // Assert
            Assert.NotEmpty(dynamicQuery.Parameters);
        }
    }
}
