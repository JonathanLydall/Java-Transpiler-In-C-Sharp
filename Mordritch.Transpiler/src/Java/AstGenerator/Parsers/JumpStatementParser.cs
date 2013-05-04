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
    public class JumpStatementParser : Parser, IParser
    {
        private JumpStatement _jumpStatement = new JumpStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            if (CurrentInputElement.Data != Keywords.Continue && CurrentInputElement.Data != Keywords.Break)
            {
                throw new Exception();
            }
            _jumpStatement.Statement = CurrentInputElement;
            MoveToNextInputElement();

            while (CurrentInputElement.Data != ";")
            {
                if (CurrentInputElement is IdentifierToken && _jumpStatement.JumpTo == null)
                {
                    _jumpStatement.JumpTo = (IdentifierToken)CurrentInputElement;
                    MoveToNextInputElement();
                    continue;
                }

                if (IsWhiteSpace)
                {
                    MoveToNextInputElement();
                    continue;
                }

                throw new Exception("Unexpected token.");
            }

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextInputElement();
            
            return _jumpStatement;
        }
    }
}
