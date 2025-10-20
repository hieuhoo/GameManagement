using System.Linq.Expressions;
using System.Reflection;

namespace BlazorAppIdolJav.SpecialComponent.ExtensionClass
{
    public sealed class Property<T>
    {
        public string Name(Expression<Func<T, object>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, int>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, int?>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, DateTime>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, DateTime?>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, bool>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, bool?>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, decimal>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, decimal?>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, long>> expression)
        {
            return NameProperty(expression);
        }
        public string Name(Expression<Func<T, long?>> expression)
        {
            return NameProperty(expression);
        }

        public string NameProperty<TR>(Expression<Func<T, TR>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null
                || propertyInfo.ReflectedType == null)
                throw new ArgumentException(string.Format("Expression '{0}' can't be cast to an Operand.", expression));
            return propertyInfo.Name;
        }

        public PropertyInfo GetProperty<TR>(Expression<Func<T, TR>> expression)
        {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null
                || propertyInfo.ReflectedType == null)
                throw new ArgumentException(string.Format("Expression '{0}' can't be cast to an Operand.", expression));
            return propertyInfo;
        }

        public string ActionName(Expression<Func<T, Action>> expression)
        {
            return MethodName(expression);
        }

        public string ActionName<TParam>(Expression<Func<T, Action<TParam>>> expression)
        {
            return MethodName(expression);
        }

        public string FuncName<TResult>(Expression<Func<T, Func<TResult>>> expression)
        {
            return MethodName(expression);
        }

        public string FuncName<TParam, TResult>(Expression<Func<T, Func<TParam, TResult>>> expression)
        {
            return MethodName(expression);
        }

        private bool IsNET45 = Type.GetType("System.Reflection.ReflectionContext", false) != null;

        private string MethodName(LambdaExpression expression)
        {
            var unaryExpression = (UnaryExpression)expression.Body;
            var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;

            if (IsNET45)
            {
                var methodCallObject = (ConstantExpression)methodCallExpression.Object;
                var methodInfo = (MethodInfo)methodCallObject.Value;
                return methodInfo.Name;
            }
            else
            {
                var methodInfoExpression = (ConstantExpression)methodCallExpression.Arguments.Last();
                var methodInfo = (MemberInfo)methodInfoExpression.Value;
                return methodInfo.Name;
            }

        }

        public Expression GetExpression(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var memberExpression = Expression.Property(parameter, propertyName);
            var expression = Expression.Lambda(memberExpression, parameter);
            return expression;
        }

        public dynamic GetData(T obj, string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var memberExpression = Expression.Property(parameter, propertyName);
            if (memberExpression == null)
                throw new ArgumentException("'property' không có body");
            PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null
                || propertyInfo.ReflectedType == null)
                throw new ArgumentException(string.Format("Expression can't be cast to an Operand."));
            return propertyInfo.GetValue(obj);
        }
    }
}
