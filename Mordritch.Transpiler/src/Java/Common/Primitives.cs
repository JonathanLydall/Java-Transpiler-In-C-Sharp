using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Common
{
    public static class Primitives
    {
        public static string Byte = "byte";
        public static string Short = "short";
        public static string Int = "int";
        public static string Long = "long";
        public static string Float = "float";
        public static string Double = "double";
        public static string Boolean = "boolean";
        public static string Char = "char";

        public static string[] AsList = 
        {
            Byte,
            Short,
            Int,
            Long,
            Float,
            Double,
            Boolean,
            Char
        };
    }
}
