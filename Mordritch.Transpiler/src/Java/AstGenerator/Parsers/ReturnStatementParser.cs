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
    public class ReturnStatementParser : Parser, IParser
    {
        private ReturnStatement _returnStatement = new ReturnStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            _returnStatement.PreComment = GetComment();

            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.Return);
            MoveToNextToken();

            while (CurrentInputElement.Data != ";")
            {
                _returnStatement.ReturnValue.Add(ParseExpression());
            }

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextToken();
            
            return _returnStatement;
        }

    }
}
