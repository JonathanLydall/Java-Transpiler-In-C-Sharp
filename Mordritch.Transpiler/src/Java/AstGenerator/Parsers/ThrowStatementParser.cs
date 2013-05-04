using Mordritch.Transpiler.Java.AstGenerator.Declarations;
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
    public class ThrowStatementParser : Parser, IParser
    {
        private ThrowStatement _throwStatement = new ThrowStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            //TODO: This is not a full implementation and probably won't work for all circumstances.

            Debug.Assert(CurrentInputElement.Data == Keywords.Throw);
            MoveToNextInputElement();

            Debug.Assert(CurrentInputElement is WhiteSpaceInputElement);
            MoveToNextInputElement();

            while (CurrentInputElement.Data != ";")
            {
                _throwStatement.ExceptionInstance.Add(CurrentInputElement);
                MoveToNextInputElement();
            }

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextInputElement();
            
            return _throwStatement;
        }

    }
}
