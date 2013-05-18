﻿using Mordritch.Transpiler.Java.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript
{
    public class PrimitiveMapper
    {
        private static string Number = "number";

        private static string String = "string";

        private static string Bool = "bool";

        private static string Void = "void";

        private static IDictionary<string, string> _map = new Dictionary<string, string>
        {
            { Primitives.Boolean, Bool },
            { Primitives.Byte, Number },
            { Primitives.Char, Number },
            { Primitives.Double, Number },
            { Primitives.Float, Number },
            { Primitives.Int, Number },
            { Primitives.Long, Number },
            { Primitives.Short, Number },
            { "String", String },
            { Void, Void }
        };

        private static IDictionary<string, string> _isTypeOfMap = new Dictionary<string, string>
        {
            { Primitives.Boolean, "boolean" },
            { Primitives.Byte, "number" },
            { Primitives.Char, "number" },
            { Primitives.Double, "number" },
            { Primitives.Float, "number" },
            { Primitives.Int, "number" },
            { Primitives.Long, "number" },
            { Primitives.Short, "number" },
            { "String", "string" }
        };
        
        public static bool IsPrimitive(string type)
        {
            return _map.ContainsKey(type);
        }

        public static string Map(string type)
        {
            if (!IsPrimitive(type))
            {
                throw new Exception(string.Format("{0} is not a primitive type.", type));
            }

            return _map[type];
        }

        public static string IsTypeOfMap(string type)
        {
            if (IsPrimitive(type) && type != Void)
            {
                return _isTypeOfMap[type];
            }

            return null;
        }
    }
}
