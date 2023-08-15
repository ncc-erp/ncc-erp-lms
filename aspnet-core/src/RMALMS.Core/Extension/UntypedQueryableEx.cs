//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Text;

//namespace RMALMS.Extension
//{
//    public static class UntypedQueryableEx
//    {
//        public static IQueryable Select(this IQueryable query, Func<ParameterExpression, DynamicClassShape> dynamicClassShapeFact)
//        {
//            var qres = query.Select(e => dynamicClassShapeFact(e).CreateShapeExpr());
//            return qres;
//        }

//        public static IQueryable SelectMany(this IQueryable source, Func<Expression, Expression> collectionSelector, Func<Expression, Expression, DynamicClassShape> resultSelector)
//        {
//            var collectionSelectorArg = Expression.Parameter(source.ElementType);
//            var collectionSelectorExpr = collectionSelector(collectionSelectorArg);
//            var collectionSelectorLambda = Expression.Lambda(collectionSelectorExpr, collectionSelectorArg);
//            var collectionItemType = collectionSelectorLambda.ReturnType.GetEnumerableItemType();

//            var resultSourceArg = Expression.Parameter(source.ElementType);
//            var resultCollectionItemArg = Expression.Parameter(collectionItemType);
//            var resultSelectorShape = resultSelector(resultSourceArg, resultCollectionItemArg);
//            var resultLambda = Expression.Lambda(resultSelectorShape.CreateShapeExpr(), resultSourceArg, resultCollectionItemArg);

//            var selectManyExpr = Expression.Call(
//                typeof(Queryable), "SelectMany",
//                TypeHelpers.TypeArgs(source.ElementType, collectionItemType, resultLambda.ReturnType),
//                TypeHelpers.Args(source.Expression, collectionSelectorLambda, resultLambda)
//            );

//            var res = source.Provider.CreateQuery(selectManyExpr);
//            return res;
//        }

//        public static IQueryable Join(
//            this IQueryable outer,
//            IEnumerable inner,
//            Func<Expression, DynamicClassShape> outerKeySelector,
//            Func<Expression, DynamicClassShape> innerKeySelector,
//            Func<Expression, Expression, DynamicClassShape> resultSelector)
//        {
//            var innerElementType = inner.GetType().GetEnumerableItemType();

//            var outerArg = Expression.Parameter(outer.ElementType);
//            var outerKeySelectorShape = outerKeySelector(outerArg);
//            var outerKeyLambda = Expression.Lambda(outerKeySelectorShape.CreateShapeExpr(), outerArg);

//            var innerArg = Expression.Parameter(innerElementType);
//            var innerKeySelectorShape = innerKeySelector(innerArg);
//            var innerKeyLambda = Expression.Lambda(innerKeySelectorShape.CreateShapeExpr(), innerArg);

//            var resultArg1 = Expression.Parameter(outer.ElementType);
//            var resultArg2 = Expression.Parameter(innerElementType);
//            var resultSelectorShape = resultSelector(resultArg1, resultArg2);
//            var resultLambda = Expression.Lambda(resultSelectorShape.CreateShapeExpr(), resultArg1, resultArg2);


//            var innerAsQueryable = inner as IQueryable;
//            var innerExpression = innerAsQueryable == null ? inner.AsConstantExpression() : innerAsQueryable.Expression;

//            var joinExpr = Expression.Call(
//                typeof(Queryable), "Join",
//                TypeHelpers.TypeArgs(outer.ElementType, innerElementType, outerKeyLambda.ReturnType, resultLambda.ReturnType),
//                TypeHelpers.Args(outer.Expression, innerExpression, outerKeyLambda, innerKeyLambda, resultLambda)
//            );

//            var res = outer.Provider.CreateQuery(joinExpr);
//            return res;
//        }

//        public static IQueryable GroupJoin(
//            this IQueryable outer,
//            IEnumerable inner,
//            Func<Expression, DynamicClassShape> outerKeySelector,
//            Func<Expression, DynamicClassShape> innerKeySelector,
//            Func<Expression, Expression, DynamicClassShape> resultSelector)
//        {
//            var innerElementType = inner.GetType().GetEnumerableItemType();

//            var outerArg = Expression.Parameter(outer.ElementType);
//            var outerKeySelectorShape = outerKeySelector(outerArg);
//            var outerKeyLambda = Expression.Lambda(outerKeySelectorShape.CreateShapeExpr(), outerArg);

//            var innerArg = Expression.Parameter(innerElementType);
//            var innerKeySelectorShape = innerKeySelector(innerArg);
//            var innerKeyLambda = Expression.Lambda(innerKeySelectorShape.CreateShapeExpr(), innerArg);

//            var resultArg1 = Expression.Parameter(outer.ElementType);
//            var resultArg2 = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(innerElementType));
//            var resultSelectorShape = resultSelector(resultArg1, resultArg2);
//            var resultLambda = Expression.Lambda(resultSelectorShape.CreateShapeExpr(), resultArg1, resultArg2);

//            var innerAsQueryable = inner as IQueryable;
//            var innerExpression = innerAsQueryable == null ? inner.AsConstantExpression() : innerAsQueryable.Expression;

//            var joinExpr = Expression.Call(
//                typeof(Queryable), "GroupJoin",
//                TypeHelpers.TypeArgs(outer.ElementType, innerElementType, outerKeyLambda.ReturnType, resultLambda.ReturnType),
//                TypeHelpers.Args(outer.Expression, innerExpression, outerKeyLambda, innerKeyLambda, resultLambda)
//            );

//            var res = outer.Provider.CreateQuery(joinExpr);
//            return res;
//        }

//        static IOrderedQueryable OrderByExtended(this IQueryable source, Func<Expression, Expression> keySelector, bool first, bool desc)
//        {
//            var sourceArg = Expression.Parameter(source.ElementType);
//            var keySelectorBody = keySelector(sourceArg);
//            var keyLambda = Expression.Lambda(keySelectorBody, sourceArg);

//            var mname = first ? "OrderBy" : "ThenBy";
//            if (desc)
//                mname += "Descending";

//            var orderByExpr = Expression.Call(
//                typeof(Queryable),
//                mname,
//                TypeHelpers.TypeArgs(source.ElementType, keyLambda.ReturnType),
//                TypeHelpers.Args(source.Expression, keyLambda)
//            );

//            var res = source.Provider.CreateQuery(orderByExpr);
//            return (IOrderedQueryable)res;
//        }

//        public static IOrderedQueryable OrderBy(this IQueryable source, Func<Expression, Expression> keySelector)
//        {
//            return source.OrderByExtended(keySelector, first: true, desc: false);
//        }

//        public static IOrderedQueryable OrderByDescending(this IQueryable source, Func<Expression, Expression> keySelector)
//        {
//            return source.OrderByExtended(keySelector, first: true, desc: true);
//        }

//        public static IOrderedQueryable ThenBy(this IOrderedQueryable source, Func<Expression, Expression> keySelector)
//        {
//            return source.OrderByExtended(keySelector, first: false, desc: false);
//        }

//        public static IOrderedQueryable ThenByDescending(this IOrderedQueryable source, Func<Expression, Expression> keySelector)
//        {
//            return source.OrderByExtended(keySelector, first: false, desc: true);
//        }

//        public static ICollection ToListUntyped(this IQueryable query)
//        {
//            var list = (IList)typeof(List<>).MakeGenericType(query.ElementType).GetConstructor(Type.EmptyTypes).Invoke(null);
//            foreach (var item in query)
//                list.Add(item);

//            return list;
//        }
//    }

//    public class LinqExpressionException : Exception
//    {
//        public LinqExpressionException(string message, Exception inner = null) : base(message, inner)
//        {
//        }
//    }

//    internal static class TypeHelpers
//    {
//        internal static Expression CreateShapeExpr(this DynamicClassShape shape)
//        {
//            if (shape.Fields.Take(2).Count() == 1 && shape.Fields.First().Name.IsEmpty())
//                return shape.Fields.First().Value;

//            if (shape.ShapeType != null)
//            {
//                var getMember = FuncEx.Create((string name) =>
//                {
//                    //have to include NonPublic since internal properties can also be used in linq expressions
//                    const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
//                    var members = shape.ShapeType.GetMember(name, bindingFlags);
//                    if (!members.Any())
//                        throw new LinqExpressionException(string.Format("Member {0}.{1} not found.", shape.ShapeType.Name, name));

//                    if (members.Length > 1)
//                        throw new LinqExpressionException(string.Format("Found multiple members {0}.{1}.", shape.ShapeType.Name, name));

//                    return members.Single();
//                });

//                var bindings = shape.Fields.Select(field => Expression.Bind(getMember(field.Name), field.Value)).ToArray();
//                var createAndBindExpr = Expression.MemberInit(Expression.New(shape.ShapeType), bindings);
//                return createAndBindExpr;
//            }

//            var selector = string.Format("new({0})", shape.Fields.Select((f, i) => string.Format("@{0} as {1}", i, f.Name)).Concatenate(", "));
//            var lambda = System.Linq.Dynamic.DynamicExpression.ParseLambda(new ParameterExpression[] { }, null, selector, shape.Fields.Select(f => (object)f.Value).ToArray());
//            var res = lambda.Call();
//            return res;
//        }
//        internal static Type[] TypeArgs(params Type[] args) { return args; }
//        internal static Expression[] Args(params Expression[] args) { return args.Select(arg => arg is LambdaExpression ? arg.Quote() : arg).ToArray(); }
//    }

//    public static class EnumerableUntypedExtensions
//    {
//        public static Expression Select(this Expression query, Func<Expression, DynamicClassShape> expr)
//        {
//            var qres = query.Select(e => expr(e).CreateShapeExpr());
//            return qres;
//        }
//        public static Expression NewInstance(this DynamicClassShape shape)
//        {
//            return shape.CreateShapeExpr();
//        }
//        public static NewArrayExpression NewArray(this IEnumerable<Expression> items)
//        {
//            return Expression.NewArrayInit(items.First().Type, items);
//        }
//        static MethodCallExpression OrderByExtended(this Expression source, Func<Expression, Expression> keySelector, bool first, bool desc)
//        {
//            var res = source.OrderBy(p => keySelector(p), firstKey: first, descending: desc);
//            return res;
//        }
//        public static MethodCallExpression OrderBy(this Expression source, Func<Expression, Expression> keySelector)
//        {
//            return source.OrderByExtended(keySelector, first: true, desc: false);
//        }
//        public static MethodCallExpression OrderByDescending(this Expression source, Func<Expression, Expression> keySelector)
//        {
//            return source.OrderByExtended(keySelector, first: true, desc: true);
//        }
//        public static MethodCallExpression ThenBy(this Expression source, Func<Expression, Expression> keySelector)
//        {
//            return source.OrderByExtended(keySelector, first: false, desc: false);
//        }
//        public static MethodCallExpression ThenByDescending(this Expression source, Func<Expression, Expression> keySelector)
//        {
//            return source.OrderByExtended(keySelector, first: false, desc: true);
//        }

//        public static MethodCallExpression GroupJoin(
//            this Expression outer,
//            Expression inner,
//            Func<Expression, DynamicClassShape> outerKeySelector,
//            Func<Expression, DynamicClassShape> innerKeySelector,
//            Func<Expression, Expression, DynamicClassShape> resultSelector)
//        {
//            var outerElementType = outer.Type.GetEnumerableItemType();
//            var outerArg = Expression.Parameter(outerElementType);
//            var outerKeySelectorShape = outerKeySelector(outerArg);
//            var outerKeyLambda = Expression.Lambda(outerKeySelectorShape.CreateShapeExpr(), outerArg);

//            var innerElementType = inner.Type.GetEnumerableItemType();
//            var innerArg = Expression.Parameter(innerElementType);
//            var innerKeySelectorShape = innerKeySelector(innerArg);
//            var innerKeyLambda = Expression.Lambda(innerKeySelectorShape.CreateShapeExpr(), innerArg);

//            var resultArg1 = Expression.Parameter(outerElementType);
//            var resultArg2 = Expression.Parameter(typeof(IEnumerable<>).MakeGenericType(innerElementType));
//            var resultSelectorShape = resultSelector(resultArg1, resultArg2);
//            var resultLambda = Expression.Lambda(resultSelectorShape.CreateShapeExpr(), resultArg1, resultArg2);


//            var innerAsQueryable = inner as IQueryable;
//            var innerExpression = innerAsQueryable == null ? inner.AsConstantExpression() : innerAsQueryable.Expression;

//            var joinExpr = Expression.Call(
//                typeof(Enumerable), "GroupJoin",
//                TypeHelpers.TypeArgs(outerElementType, innerElementType, outerKeyLambda.ReturnType, resultLambda.ReturnType),
//                outer, innerExpression, outerKeyLambda, innerKeyLambda, resultLambda
//            );

//            return joinExpr;
//        }
//        public static MethodCallExpression GroupBy(
//            this Expression items,
//            Func<Expression, DynamicClassShape> keySelector,
//            Func<Expression, DynamicClassShape> elementSelector)
//        {
//            var elementType = items.Type.GetEnumerableItemType();

//            var keySelectorArg = Expression.Parameter(elementType);
//            var keySelectorShape = keySelector(keySelectorArg);
//            var keyLambdaBody = keySelectorShape.CreateShapeExpr();
//            var keyLambda = Expression.Lambda(keyLambdaBody, keySelectorArg);

//            var elementSelectorArg = Expression.Parameter(elementType);
//            var elementSelectorShape = elementSelector(elementSelectorArg);
//            var elementSelectorLambdaBody = elementSelectorShape.CreateShapeExpr();
//            var elementSelectorLambda = Expression.Lambda(elementSelectorLambdaBody, elementSelectorArg);

//            var expr = Expression.Call(
//                typeof(Enumerable), "GroupBy",
//                TypeHelpers.TypeArgs(elementType, keyLambda.ReturnType, elementSelectorLambda.ReturnType),
//                items, keyLambda, elementSelectorLambda
//            );

//            return expr;
//        }

//        public static MethodCallExpression Count(this Expression items)
//        {
//            var elementType = items.Type.GetEnumerableItemType();
//            var expr = Expression.Call(
//                typeof(Enumerable), "Count",
//                TypeHelpers.TypeArgs(elementType),
//                items
//            );
//            return expr;
//        }
//    }

//    public static class DynamicClassExtensions
//    {
//        public static DynamicClassField AsField(this Expression value, string name) { return new DynamicClassField { Name = name, Value = value }; }
//        public static DynamicClassShape AsShape(this Expression value) { return DynamicClassShape.Create(value.AsField("")); }
//    }

//    [DebuggerDisplay("{Name} = {Value}")]
//    public class DynamicClassField
//    {
//        public string Name { get; set; }
//        public Expression Value { get; set; }
//    }

//    public class DynamicClassShape
//    {
//        public static DynamicClassShape Create(params DynamicClassField[] fields) { return Create((Type)null, fields); }
//        public static DynamicClassShape Create(Type classType, params DynamicClassField[] fields) { return new DynamicClassShape { Fields = fields, ShapeType = classType }; }
//        public IEnumerable<DynamicClassField> Fields { get; set; }
//        public Type ShapeType;

//        public static DynamicClassShape ChangeType(ParameterExpression e, Type shapeType)
//        {
//            return new DynamicClassShape { Fields = e.Type.GetProperties().Select(p => e.Property(p).AsField(p.Name)), ShapeType = shapeType };
//        }
//    }
//}
