using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Indigo.Utility
{
    public static class FastFactory<T>
    {
        public static T CreateInstance()
            => FactoryImpl<T>.Create();

        public static T CreateInstance<TArg0>(TArg0 arg0)
            => FactoryImpl<T, TArg0>.Create(arg0);

        public static T CreateInstance<TArg0, TArg1>(TArg0 arg0, TArg1 arg1)
            => FactoryImpl<T, TArg0, TArg1>.Create(arg0, arg1);

        public static T CreateInstance<TArg0, TArg1, TArg2>(TArg0 arg0, TArg1 arg1, TArg2 arg2)
            => FactoryImpl<T, TArg0, TArg1, TArg2>.Create(arg0, arg1, arg2);

        public static T CreateInstance<TArg0, TArg1, TArg2, TArg3>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            => FactoryImpl<T, TArg0, TArg1, TArg2, TArg3>.Create(arg0, arg1, arg2, arg3);

        public static T CreateInstance<TArg0, TArg1, TArg2, TArg3, TArg4>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4)
            => FactoryImpl<T, TArg0, TArg1, TArg2, TArg3, TArg4>.Create(arg0, arg1, arg2, arg3, arg4);

        public static T CreateInstance<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5)
            => FactoryImpl<T, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>.Create(arg0, arg1, arg2, arg3, arg4, arg5);

        public static T CreateInstance<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6)
            => FactoryImpl<T, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>.Create(arg0, arg1, arg2, arg3, arg4, arg5, arg6);

        private static ConstructorInfo GetBestConstructor(Type type, Type[] paramTypes)
        {
            //  first look for an exact match
            var result = type.GetConstructor(paramTypes);
            if (result != null)
                return result;

            foreach (var ctor in type.GetConstructors())
            {
                var ctorParams = ctor.GetParameters();
                var lastParam = ctorParams.LastOrDefault();
                if (lastParam != null && lastParam.IsDefined(typeof(ParamArrayAttribute)))
                {
                    var newParams = ctorParams
                        .Select(p => p.ParameterType)
                        .ToArray();

                    for (int i = 0; i < ctorParams.Length - 1; i++)
                        newParams[i] = paramTypes[i];

                    result = type.GetConstructor(newParams);
                    if (result != null)
                        return result;
                }
            }

            throw new Exception("No constructor found");
        }

        private static TFunc Compile<TFunc>(params Type[] types)
        {
            var ctor = GetBestConstructor(typeof(T), types);
            var parameters = types
                .Select(Expression.Parameter)
                .ToArray();

            var args = ctor.GetParameters()
                .Select((p, i) => ConvertOrExpandParameter(p, i, parameters))
                .ToArray();

            var callNew = Expression.New(ctor, args);
            return Expression.Lambda<TFunc>(callNew, parameters).Compile();
        }

        private static Expression ConvertOrExpandParameter(ParameterInfo p, int index, ParameterExpression[] parameters)
        {
            if (p.IsDefined(typeof(ParamArrayAttribute)))
            {
                var arrayType = p.ParameterType.GetElementType();
                var expressions = new Expression[parameters.Length - index];
                for (int i = 0; i < expressions.Length; i++)
                    expressions[i] = Expression.Convert(parameters[index + i], arrayType);
                
                return Expression.NewArrayInit(arrayType, expressions);
            }

            return Expression.Convert(parameters[index], p.ParameterType);
        }

        #region Implementations
        private static class FactoryImpl<TResult>
        {
            public static readonly Func<TResult> Create = GenerateFactory();

            private static Func<TResult> GenerateFactory()
            {
                if (typeof(TResult).IsValueType)return () => default(TResult);
                return Expression.Lambda<Func<TResult>>(Expression.New(typeof(TResult).GetConstructor(Type.EmptyTypes))).Compile();
            }
        }

        private static class FactoryImpl<TResult, TArg0>
        {
            public static readonly Func<TArg0, TResult> Create = GenerateFactory();

            private static Func<TArg0, TResult> GenerateFactory()
            {
                var types = new[] { typeof(TArg0) };
                return Compile<Func<TArg0, TResult>>(types);
            }
        }

        private static class FactoryImpl<TResult, TArg0, TArg1>
        {
            public static readonly Func<TArg0, TArg1, TResult> Create = GenerateFactory();

            private static Func<TArg0, TArg1, TResult> GenerateFactory()
            {
                var types = new[] { typeof(TArg0), typeof(TArg1) };
                return Compile<Func<TArg0, TArg1, TResult>>(types);
            }
        }

        private static class FactoryImpl<TResult, TArg0, TArg1, TArg2>
        {
            public static readonly Func<TArg0, TArg1, TArg2, TResult> Create = GenerateFactory();

            private static Func<TArg0, TArg1, TArg2, TResult> GenerateFactory()
            {
                var types = new[] { typeof(TArg0), typeof(TArg1), typeof(TArg2) };
                return Compile<Func<TArg0, TArg1, TArg2, TResult>>(types);
            }
        }

        private static class FactoryImpl<TResult, TArg0, TArg1, TArg2, TArg3>
        {
            public static readonly Func<TArg0, TArg1, TArg2, TArg3, TResult> Create = GenerateFactory();

            private static Func<TArg0, TArg1, TArg2, TArg3, TResult> GenerateFactory()
            {
                var types = new[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3) };
                return Compile<Func<TArg0, TArg1, TArg2, TArg3, TResult>>(types);
            }
        }

        private static class FactoryImpl<TResult, TArg0, TArg1, TArg2, TArg3, TArg4>
        {
            public static readonly Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult> Create = GenerateFactory();

            private static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult> GenerateFactory()
            {
                var types = new[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4) };
                return Compile<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TResult>>(types);
            }
        }

        private static class FactoryImpl<TResult, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5>
        {
            public static readonly Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> Create = GenerateFactory();

            private static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> GenerateFactory()
            {
                var types = new[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5) };
                return Compile<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TResult>>(types);
            }
        }

        private static class FactoryImpl<TResult, TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6>
        {
            public static readonly Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> Create = GenerateFactory();

            private static Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult> GenerateFactory()
            {
                var types = new[] { typeof(TArg0), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4), typeof(TArg5), typeof(TArg6) };
                return Compile<Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TResult>>(types);
            }
        }
        #endregion
    }
}
