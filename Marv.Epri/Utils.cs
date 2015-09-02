using System.Linq;
using Marv.Common;

namespace Marv.Epri
{
    public static class Utils
    {
        public static string FormRestArgs(object args)
        {
            var str = args.GetType().GetProperties().AllButLast().Aggregate("", (current, propertyInfo) => current + (propertyInfo.Name + "=" + propertyInfo.GetValue(args) + "&"));

            var lastPropertyInfo = args.GetType().GetProperties().Last();
            str += lastPropertyInfo.Name + "=" + lastPropertyInfo.GetValue(args);

            return str;
        }
    }
}