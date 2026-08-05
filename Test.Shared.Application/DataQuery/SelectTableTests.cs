

using Shared.Application.DataQuery;
using Xunit;

namespace Test.Shared.Application.DataQuery
{
    public class SelectTableTests
    {
        [Fact]
        public void Constructor_WithColumnName_SetColumnNameOnly()
        {
            var column = new JoinSelectColumn(" TestColumn ");

            Assert.Equal("TestColumn",column.ColumnName);
            Assert.Null(column.ColumnAliasName);
        }

        [Fact]
        public void Constructor_WithColumnNameAndAlias_SetColumnNameAndAlias()
        {
            var column = new JoinSelectColumn(" TestColumn ","[Test] ");

            Assert.Equal("TestColumn", column.ColumnName);
            Assert.Equal("Test",column.ColumnAliasName);
        }
    }
}
