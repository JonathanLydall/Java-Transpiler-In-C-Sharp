using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class SimpleStatementParser : Parser, IParser
    {
        public override IAstNode ImplementationSpecificParse()
        {
            var simpleStatement = new SimpleStatement(); 
            
            while (CurrentInputElement.Data != ";")
            {
                simpleStatement.Expressions.Add(ParseExpression());
            }

            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextToken();

            return simpleStatement;
        }

        public static bool IsSimpleStatement(Parser parser)
        {
            return (parser.CurrentInputElement.Data == ")" && parser.NextNonWhiteSpaceInputElement.Data == ";") ||
            (parser.CurrentInputElement.Data == "." && parser.PreviousNonWhiteSpaceInputElement.Data == ")"); //Chained function calls, like: task().doWithTask();
        }
    }
}
