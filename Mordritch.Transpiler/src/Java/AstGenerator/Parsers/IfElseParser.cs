using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System.Diagnostics;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class IfElseStatementParser : Parser, IParser
    {
        private IfElseStatement _ifElseStatement = new IfElseStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            _ifElseStatement.PreComment = GetComment();
            
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.If);
            MoveToNextToken();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextToken();

            _ifElseStatement.Condition = GetInnerExpression(")");
            
            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextToken();

            _ifElseStatement.Body = GetWork();

            while (!Eof && CurrentInputElement.Data == Keywords.Else)
            {
                ProcessElseStatement();
            }

            return _ifElseStatement;
        }

        private IList<IAstNode> GetWork()
        {
            if (CurrentInputElement is SeperatorToken && CurrentInputElement.Data == "{")
            {
                return ParseBody();
            }
            else
            {
                return new List<IAstNode>{ ParseSingleStatement() };
            }
        }

        private void ProcessElseStatement()
        {
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.Else);
            MoveToNextToken();

            if (CurrentInputElement.Data == Keywords.If)
            {
                MoveToNextToken();
                var conditionalElse = new IfElseStatement();
                
                Debug.Assert(CurrentInputElement is SeperatorToken);
                Debug.Assert(CurrentInputElement.Data == "(");
                MoveToNextToken();

                conditionalElse.Condition = GetInnerExpression(")");

                Debug.Assert(CurrentInputElement is SeperatorToken);
                Debug.Assert(CurrentInputElement.Data == ")");
                MoveToNextToken();

                conditionalElse.Body = GetWork();
                _ifElseStatement.ConditionalElses.Add(conditionalElse);
            }
            else
            {
                _ifElseStatement.ElseBody = new List<IAstNode>();
                _ifElseStatement.ElseBody = GetWork();
            }
        }
    }
}
