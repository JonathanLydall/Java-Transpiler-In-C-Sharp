using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Common
{
    public static class Keywords
    {
        public static string Abstract = "abstract";
        public static string Continue = "continue";
        public static string For = "for";
        public static string New = "new";
        public static string Switch = "switch";
        public static string Assert = "assert";
        public static string Default = "default";
        public static string If = "if";
        public static string Package = "package";
        public static string Synchronized = "synchronized";
        public static string Boolean = "boolean";
        public static string Do = "do";
        public static string Goto = "goto";
        public static string Private = "private";
        public static string This = "this";
        public static string Break = "break";
        public static string Double = "double";
        public static string Implements = "implements";
        public static string Protected = "protected";
        public static string Throw = "throw";
        public static string Byte = "byte";
        public static string Else = "else";
        public static string Import = "import";
        public static string Public = "public";
        public static string Throws = "throws";
        public static string Case = "case";
        public static string Enum = "enum";
        public static string Instanceof = "instanceof";
        public static string Return = "return";
        public static string Transient = "transient";
        public static string Catch = "catch";
        public static string Extends = "extends";
        public static string Int = "int";
        public static string Short = "short";
        public static string Try = "try";
        public static string Char = "char";
        public static string Final = "final";
        public static string Interface = "interface";
        public static string Static = "static";
        public static string Void = "void";
        public static string Class = "class";
        public static string Finally = "finally";
        public static string Long = "long";
        public static string Strictfp = "strictfp";
        public static string Volatile = "volatile";
        public static string Const = "const";
        public static string Float = "float";
        public static string Native = "native";
        public static string Super = "super";
        public static string While = "while";

        public static IList<string> MethodModifiers = new List<string>() { Abstract, Static, Final, Native, Strictfp, Synchronized, Protected };
    }
}
