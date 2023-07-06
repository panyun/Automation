using MongoDB.Bson.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot
{
    public static class Helper
    {
        public static T GetValue<T>(this object obj, string propertyName)
        {
            try
            {
                var t= (T)obj.GetType().GetProperty(propertyName).GetValue(obj, null);
                return t;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
