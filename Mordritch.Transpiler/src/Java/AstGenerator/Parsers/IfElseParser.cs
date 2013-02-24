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
    public class IfElseParser : Parser, IParser
    {
        private IfElseStatement _ifElseStatement = new IfElseStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            _ifElseStatement.PreComment = GetComment();
            
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.If);
            MoveToNextToken();

            ProcessCondition();

            return _ifElseStatement;
        }

        private void ProcessCondition()
        {
            _ifElseStatement.Condition = ProcessConditionalBracket();
            
            MoveToNextToken();
            if (CurrentInputElement is SeparatorToken && CurrentInputElement.Data == "{")
            {
                _ifElseStatement.Body = ParseBody();
            }
            else
            {
                _ifElseStatement.Body.Add(ParseSingleStatement());
            }

            while (CurrentInputElement.Data == Keywords.Else)
            {
                ProcessElseStatement();
            }
        }

        private void ProcessElseStatement()
        {
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.Else);
            MoveToNextToken();

            if (CurrentInputElement.Data == Keywords.If)
            {
                var conditionalElse = new IfElseStatement();
                
                MoveToNextToken();
                Debug.Assert(CurrentInputElement is SeparatorToken);
                Debug.Assert(CurrentInputElement.Data == "(");

                conditionalElse.Condition = ProcessConditionalBracket();
                MoveToNextToken();

                if (CurrentInputElement is SeparatorToken && CurrentInputElement.Data == "{")
                {
                    conditionalElse.Body = ParseBody();
                }
                else
                {
                    conditionalElse.Body.Add(ParseSingleStatement());
                }
            }
            else
            {
                if (CurrentInputElement is SeparatorToken && CurrentInputElement.Data == "{")
                {
                    _ifElseStatement.Body = ParseBody();
                }
                else
                {
                    _ifElseStatement.Body.Add(ParseSingleStatement());
                }
            }
        }

        private IList<IInputElement> ProcessConditionalBracket()
        {
            var contents = new List<IInputElement>();

            Debug.Assert(CurrentInputElement is SeparatorToken);
            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextInputElement();
            
            while (CurrentInputElement.Data != ")")
            {
                if (CurrentInputElement.Data == "(")
                {
                    contents.AddRange(ProcessConditionalBracket());
                    continue;
                }

                contents.Add(CurrentInputElement);
                MoveToNextInputElement();
                continue;
            }

            Debug.Assert(CurrentInputElement is SeparatorToken);
            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextInputElement();

            return contents;
        }
    }
}
