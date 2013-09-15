using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Common
{
    public static class Primitives
    {
        public static string Boolean = "boolean";
        public static string Byte = "byte";
        public static string Char = "char";
        public static string Double = "double";
        public static string Float = "float";
        public static string Int = "int";
        public static string Long = "long";
        public static string Short = "short";
        public static string String = "String";
        public static string Object = "Object";

        public static string[] AsList = 
        {
            Boolean,
            Byte,
            Char,
            Double,
            Float,
            Int,
            Long,
            Short,
            String,
            Object
        };
    }
}
