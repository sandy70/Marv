using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace LibPipeline
{
    internal sealed class MetaObject<T> : DynamicMetaObject
    {
        internal MetaObject(Expression parameter, T value)
            : base(parameter, BindingRestrictions.Empty, value)
        {
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            // Method call in the containing class:
            string methodName = "GetMember";

            // One parameter
            Expression[] parameters = new Expression[]
            {
                Expression.Constant(binder.Name)
            };

            DynamicMetaObject getDictionaryEntry = new DynamicMetaObject(
                Expression.Call(
                    Expression.Convert(Expression, LimitType),
                    typeof(T).GetMethod(methodName),
                    parameters),
                BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            return getDictionaryEntry;
        }

        public override DynamicMetaObject BindInvokeMember(
                    InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            StringBuilder paramInfo = new StringBuilder();
            paramInfo.AppendFormat("Calling {0}(", binder.Name);
            foreach (var item in args)
                paramInfo.AppendFormat("{0}, ", item.Value);
            paramInfo.Append(")");

            Expression[] parameters = new Expression[]
            {
                Expression.Constant(paramInfo.ToString())
            };
            DynamicMetaObject methodInfo = new DynamicMetaObject(
                Expression.Call(
                Expression.Convert(Expression, LimitType),
                typeof(T).GetMethod("WriteMethodInfo"),
                parameters),
                BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            return methodInfo;
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder,
                    DynamicMetaObject value)
        {
            // Method to call in the containing class:
            string methodName = "SetMember";

            // setup the binding restrictions.
            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            // setup the parameters:
            Expression[] args = new Expression[2];
            // First parameter is the name of the property to Set
            args[0] = Expression.Constant(binder.Name);
            // Second parameter is the value
            args[1] = Expression.Convert(value.Expression, typeof(object));

            // Setup the 'this' reference
            Expression self = Expression.Convert(Expression, LimitType);

            // Setup the method call expression
            Expression methodCall = Expression.Call(self,
                    typeof(T).GetMethod(methodName),
                    args);

            // Create a meta object to invoke Set later:
            DynamicMetaObject setDictionaryEntry = new DynamicMetaObject(
                methodCall,
                restrictions);
            // return that dynamic object
            return setDictionaryEntry;
        }
    }
}