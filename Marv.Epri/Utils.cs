using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marv;
using Marv.Common;

namespace Marv.Epri
{
    public static class Utils
    {
        public static string FormRestArgs(object args)
        {
            var str = "";

            foreach (var propertyInfo in args.GetType().GetProperties().AllButLast())
            {
                str += propertyInfo.Name + "=" + propertyInfo.GetValue(args) + "&";
            }

            var lastPropertyInfo = args.GetType().GetProperties().Last();
            str += lastPropertyInfo.Name + "=" + lastPropertyInfo.GetValue(args);

            return str;
        }
    }
}
