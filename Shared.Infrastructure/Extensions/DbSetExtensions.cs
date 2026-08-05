using Microsoft.EntityFrameworkCore;
using NDS.Shared.Infrastructure.Extensions;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace NDS.Shared.Infrastructure.Extensions
{
    public static class DbSetExtensions
    {

        public async static Task<TObject?> SingleOrDefaultAsync<TObject>(this DbSet<TObject> dbSet, IEnumerable<KeyValuePair<string, string>> keyValues) where TObject : class
        {
            if (keyValues == null || !keyValues.Any())
            {
                return null;
            }

            Expression expression = null!;

            var parameter = Expression.Parameter(typeof(TObject), "e");

            foreach (var pair in keyValues)
            {
                var property = Expression.PropertyOrField(parameter, pair.Key);


                if (property != null)
                {
                    var propertyType = ((PropertyInfo)property.Member).PropertyType;
                    var converter = TypeDescriptor.GetConverter(propertyType);
                    var value = Expression.Constant(converter.ConvertFrom(pair.Value));
                    expression = expression == null
                        ? Expression.Equal(property, value)
                        : Expression.AndAlso(expression, Expression.Equal(property, value));
                }
            }

            if (expression != null)
            {

                var compiledExpression = Expression.Lambda<Func<TObject, bool>>(expression, parameter);
                return await dbSet.SingleOrDefaultAsync(compiledExpression, CancellationToken.None);
            }

            return null;
        }


    }
}
