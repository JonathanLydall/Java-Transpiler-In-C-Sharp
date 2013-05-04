using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Common
{
    public class JavaUtils
    {
        public static bool IsAssignmentOperator(string data)
        {
            var assignmentOperators = new string[]{ "=", "+=", "-=", "*=", "/=", "%=", "&=", "^=", "|=", "<<=", ">>=", ">>>=" };
            return assignmentOperators.Any(assignmentOperator => assignmentOperator == data);
        }
    }
}
