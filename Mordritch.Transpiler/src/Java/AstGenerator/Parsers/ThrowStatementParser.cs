using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class ThrowStatementParser : Parser, IParser
    {
        private ThrowStatement _throwStatement = new ThrowStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            AssertKeyword(Keywords.Throw);
            MoveToNextInputElement();

            AssertWhiteSpace();
            MoveToNextInputElement();

            while (CurrentInputElement.Data != ";")
            {
                _throwStatement.ExceptionInstance.Add(CurrentInputElement);
                MoveToNextInputElement();
            }
            
            AssertSeperator(";");
            MoveToNextInputElement();
            
            return _throwStatement;
        }

    }
}
