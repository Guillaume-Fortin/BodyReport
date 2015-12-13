using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BodyReport.Models;
using Message;
using Microsoft.Data.Entity;
using System.Reflection;
using System.Linq.Expressions;

namespace BodyReport.Crud.Transformer
{
    public static class CriteriaTransformer
    {
        public static void CompleteQuery<TEntity>(ref IQueryable<TEntity> source, CriteriaField criteriaField) where TEntity : class
        {
            if (source == null || criteriaField == null)
                return;
            
            var criteriaFieldProperties = criteriaField.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

            object value;
            string fieldName;
            var type = typeof(TEntity);
            var properties = type.GetProperties();
            if (properties != null && criteriaFieldProperties != null)
            {
                foreach (var criteriaFieldProperty in criteriaFieldProperties)
                {
                    fieldName = criteriaFieldProperty.Name;
                    value = criteriaFieldProperty.GetValue(criteriaField, null);
                    if (value == null)
                        continue;

                    CompleteQueryWithCriteria(ref source, fieldName, value);
                }
            }
        }

        private static Expression StackExpression(List<Expression> expressionList)
        {
            Expression expression = null;
            foreach (var exp in expressionList)
            {
                if (expression == null)
                {
                    expression = exp;
                }
                else
                {
                    expression = Expression.And(expression, exp);
                }
            }
            return expression;
        }

        private static Expression AddEqualExpression<T>(ParameterExpression entityParameter, PropertyInfo entityProperty, T value, bool ignoreCase)
        {
            if(typeof(T) != typeof(string))
            {
                return Expression.Equal(
                        Expression.Property(entityParameter, entityProperty),
                        Expression.Constant(value)
                       );
            }
            else
            {
                MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);
                ConstantExpression c = Expression.Constant(value, typeof(string));
                MethodInfo mi = typeof(string).GetMethod("Equals", new Type[] { typeof(string), typeof(StringComparison) });
                var expression = Expression.Call(m, mi, c, Expression.Constant(ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
                return Expression.Equal(expression, Expression.Constant(true));
            }
        }

        private static Expression AddNotEqualExpression<T>(ParameterExpression entityParameter, PropertyInfo entityProperty, T value, bool ignoreCase)
        {
            if (typeof(T) != typeof(string))
            {
                return Expression.Equal(
                        Expression.Property(entityParameter, entityProperty),
                        Expression.Constant(value)
                       );
            }
            else
            {
                MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);
                ConstantExpression c = Expression.Constant(value, typeof(string));
                MethodInfo mi = typeof(string).GetMethod("Equals", new Type[] { typeof(string), typeof(StringComparison) });
                var expression = Expression.Call(m, mi, c, Expression.Constant(ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
                return Expression.Equal(expression, Expression.Constant(false));
            }
        }

        private static Expression AddStartsWithStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);
            ConstantExpression c = Expression.Constant(value, typeof(string));
            MethodInfo mi = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string), typeof(StringComparison) });
            return Expression.Call(m, mi, c, Expression.Constant(ignoreCase? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        }

        private static Expression AddEndsWithStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);
            ConstantExpression c = Expression.Constant(value, typeof(string));
            MethodInfo mi = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string), typeof(StringComparison) });
            return Expression.Call(m, mi, c, Expression.Constant(ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        }

        private static Expression AddContainsStringExpression(ParameterExpression entityParameter, PropertyInfo entityProperty, string value, bool ignoreCase)
        {
            MemberExpression m = Expression.MakeMemberAccess(entityParameter, entityProperty);
            ConstantExpression c = Expression.Constant(value, typeof(string));
            MethodInfo mi = typeof(string).GetMethod("Contains", new Type[] { typeof(string), typeof(StringComparison) });
            return Expression.Call(m, mi, c, Expression.Constant(ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
        }

        private static void CompleteQueryWithCriteria<TEntity>(ref IQueryable<TEntity> source, string fieldName, object criteria) where TEntity : class
        {
            var entityType = typeof(TEntity);
            var entityProperty = entityType.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public);

            if (entityProperty != null)
            {
                var entityParameter = Expression.Parameter(typeof(TEntity), "e");
                var propertyType = entityProperty.PropertyType;

                var expressionList = new List<Expression>();
                Expression expression;

                if (criteria is IntegerCriteria)
                {
                    IntegerCriteriaTreatment(criteria as IntegerCriteria, expressionList, propertyType, entityParameter, entityProperty);
                }
                else if (criteria is StringCriteria)
                {
                    StringCriteriaTreatment(criteria as StringCriteria, expressionList, propertyType, entityParameter, entityProperty);
                }

                expression = StackExpression(expressionList);
                if (expression != null)
                {
                    var lambda = Expression.Lambda(expression, entityParameter) as Expression<Func<TEntity, bool>>;
                    source = source.Where(lambda);
                }
            }
        }

        private static void IntegerCriteriaTreatment(IntegerCriteria criteria, List<Expression> expressionList, Type propertyType,
                                                     ParameterExpression entityParameter, PropertyInfo entityProperty)
        {
            if (criteria.EqualList != null)
            {
                foreach (int equalValue in criteria.EqualList)
                {
                    if (propertyType.Equals(typeof(int)))
                    {
                        expressionList.Add(AddEqualExpression(entityParameter, entityProperty, equalValue, false));
                    }
                }
            }
            if (criteria.NotEqualList != null)
            {
                foreach (int equalValue in criteria.NotEqualList)
                {
                    if (propertyType.Equals(typeof(int)))
                    {
                        expressionList.Add(AddNotEqualExpression(entityParameter, entityProperty, equalValue, false));
                    }
                }
            }
        }

        private static void StringCriteriaTreatment(StringCriteria criteria, List<Expression> expressionList, Type propertyType,
                                                    ParameterExpression entityParameter, PropertyInfo entityProperty)
        {
            foreach (string equalValue in criteria.EqualList)
            {
                if (propertyType.Equals(typeof(string)))
                {
                    expressionList.Add(AddEqualExpression(entityParameter, entityProperty, equalValue, criteria.IgnoreCase));
                }
            }
            if (criteria.NotEqualList != null)
            {
                foreach (string equalValue in criteria.NotEqualList)
                {
                    if (propertyType.Equals(typeof(string)))
                    {
                        expressionList.Add(AddNotEqualExpression(entityParameter, entityProperty, equalValue, criteria.IgnoreCase));
                    }
                }
            }
            if(criteria.StartsWithList != null)
            {
                foreach (string equalValue in criteria.NotEqualList)
                {
                    if (propertyType.Equals(typeof(string)))
                    {
                        expressionList.Add(AddStartsWithStringExpression(entityParameter, entityProperty, equalValue, criteria.IgnoreCase));
                    }
                }
            }
            if (criteria.EndsWithList != null)
            {
                foreach (string equalValue in criteria.NotEqualList)
                {
                    if (propertyType.Equals(typeof(string)))
                    {
                        expressionList.Add(AddEndsWithStringExpression(entityParameter, entityProperty, equalValue, criteria.IgnoreCase));
                    }
                }
            }
            if (criteria.ContainsList != null)
            {
                foreach (string equalValue in criteria.NotEqualList)
                {
                    if (propertyType.Equals(typeof(string)))
                    {
                        expressionList.Add(AddContainsStringExpression(entityParameter, entityProperty, equalValue, criteria.IgnoreCase));
                    }
                }
            }
        }
    }
}
