using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements
{
    public static class InputElementClassifier
    {
        public static string[] Seperators =
        {
            "(",
            ")",
            "{",
            "}",
            "[",
            "]",
            ";",
            ",",
            "."
        };

        public static string[] Operators =
        {
            "=",
            ">",
            "<",
            "!",
            "~",
            "?",
            ":",
            "==",
            "<=",
            ">=",
            "!=",
            "&&",
            "||",
            "++",
            "--",
            "+",
            "-",
            "*",
            "/",
            "&",
            "|",
            "^",
            "%",
            "<<",
            ">>",
            ">>>",
            "+=",
            "-=",
            "*=",
            "/=",
            "&=",
            "|=",
            "^=",
            "%=",
            "<<=",
            ">>=",
            ">>>="
        };

        public static string[] Keywords = 
        {
            "abstract",
            "continue",
            "for",
            "new",
            "switch",
            "assert",
            "default",
            "if",
            "package",
            "synchronized",
            "boolean",
            "do",
            "goto",
            "private",
            "this",
            "break",
            "double",
            "implements",
            "protected",
            "throw",
            "byte",
            "else",
            "import",
            "public",
            "throws",
            "case",
            "enum",
            "instanceof",
            "return",
            "transient",
            "catch",
            "extends",
            "int",
            "short",
            "try",
            "char",
            "final",
            "interface",
            "static",
            "void",
            "class",
            "finally",
            "long",
            "strictfp",
            "volatile",
            "const",
            "float",
            "native",
            "super",
            "while",
            "const", // Reserved, but not currently used.
            "goto", // Reserved, but not currently used.
        };

        public static string NullLiteral =
            "null";

        public static string[] BooleanLiterals =
        {
            "true",
            "false"
        };

        public static string[] StaticLiterals =
        {
            NullLiteral,
            BooleanLiterals[0],
            BooleanLiterals[1]
        };
        
        public static bool IsJavaIdentifierStart(char ch)
        {
            return
                char.IsLetter(ch) ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.LetterNumber ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.CurrencySymbol ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.ConnectorPunctuation;
        }

        public static bool IsJavaIdentifierPart(char ch)
        {
            return
                char.IsLetter(ch) ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.CurrencySymbol ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.ConnectorPunctuation ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.DecimalDigitNumber ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.LetterNumber ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.SpacingCombiningMark ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark ||
                IsIdentifierIgnorable(ch);
        }

        public static bool IsIdentifierIgnorable(char ch)
        {
            var codePoint = Convert.ToInt32(ch);

            return
                (codePoint >= 0x0000 && codePoint <= 0x0008) ||
                (codePoint >= 0x000E && codePoint <= 0x001B) ||
                (codePoint >= 0x007F && codePoint <= 0x009F) ||
                char.GetUnicodeCategory(ch) == UnicodeCategory.Format;
        }

        public static bool IsJavaCharacterLiteralStart(char ch)
        {
            return ch == Convert.ToChar("'");
        }

        public static bool IsJavaStringLiteralStart(char ch)
        {
            return ch == Convert.ToChar("\"");
        }

        public static bool IsStartForWhiteSpace(char ch)
        {
            return char.IsWhiteSpace(ch);
        }

        public static bool IsCharacterLiteralStart(char ch)
        {
            return ch == Convert.ToChar("'");
        }

        public static bool IsStringLiteralStart(char ch)
        {
            return ch == Convert.ToChar("\"");
        }

        public static bool IsOperatorTokenStart(char ch)
        {
            return Operators.Any(o => o.Length == 1 && o == new String(ch, 1));
        }

        public static bool IsSeperatorTokenStart(char ch)
        {
            return Seperators.Any(s => s == new String(ch, 1));
        }

        public static bool IsStartForToken(char ch)
        {
            return
                IsJavaIdentifierStart(ch) ||
                IsJavaIdentifierPart(ch) ||
                IsStringLiteralStart(ch) ||
                IsCharacterLiteralStart(ch) ||
                IsSeperatorTokenStart(ch) ||
                IsOperatorTokenStart(ch);
        }

        public static bool IsStartForComment(char ch, char ch1)
        {
            // TODO: This probably isn't entirely to spec, see http://docs.oracle.com/javase/specs/jls/se7/html/jls-3.html#jls-3.7
            return
                ch == Convert.ToChar("/") && (
                    ch1 == Convert.ToChar("/") ||
                    ch1 == Convert.ToChar("*"));
        }
    }
}
