using System;
using System.Collections.Generic;
using System.Text;

namespace IoC.Configuration.Tests.ClassMember.Services
{
    public class StaticMethodsWithParameters
    {
        public static string GetString(int intParam, string strParam)
        {
            return $"Static: {intParam}, {strParam}";
        }

        public static int GetInt(int param1)
        {
            return param1 + 1;
        }
    }
}
