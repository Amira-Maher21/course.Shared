using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Kernel.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IQueryable<TObject> GetByKeys<TObject>(this IQueryable<TObject> quarriable, IEnumerable<KeyValuePair<string, object>> keyValues) where TObject : class
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
                    var value = pair.Value == null ? null : Expression.Constant(converter.ConvertFrom(pair.Value.ToString()!));
                    expression = expression == null
                        ? Expression.Equal(property, value!)
                        : Expression.AndAlso(expression, Expression.Equal(property, value!));
                }
            }

            if (expression != null)
            {

                var compiledExpression = Expression.Lambda<Func<TObject, bool>>(expression, parameter);
                return quarriable.Where(compiledExpression);
            }

            return null;
        }

    }

}
