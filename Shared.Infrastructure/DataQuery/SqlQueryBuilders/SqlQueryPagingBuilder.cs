namespace NDS.Shared.Infrastructure.DataQuery.SqlQueryBuilders
{
    internal class SqlQueryPagingBuilder
    {

        public string GetPagingClause(int skip, int take)
        {

            if (take > 0)
            {
                return $" OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY  ";

            }
            return string.Empty;
        }

    }
}
