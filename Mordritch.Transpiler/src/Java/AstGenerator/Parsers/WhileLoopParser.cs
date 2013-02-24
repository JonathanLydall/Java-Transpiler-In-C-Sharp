using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures;
using System.Diagnostics;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class WhileLoopParser : Parser, IParser
    {
        private WhileLoop _whileLoop = new WhileLoop();

        public override IAstNode ImplementationSpecificParse()
        {
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.While);
            MoveToNextToken();

            _whileLoop.Condition = GetConditionalBracket();

            _whileLoop.Body = GetBody();

            return _whileLoop;
        }

        private IList<IAstNode> GetBody()
        {
            MoveToNextToken();
            if (CurrentInputElement is SeparatorToken && CurrentInputElement.Data == "{")
            {
                return ParseBody();
            }
            else
            {
                var body = new List<IAstNode>();
                body.Add(ParseSingleStatement());
                return body;
            }
        }

        private IList<IInputElement> GetConditionalBracket()
        {
            var contents = new List<IInputElement>();

            Debug.Assert(CurrentInputElement is SeparatorToken);
            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextInputElement();
            
            while (CurrentInputElement.Data != ")")
            {
                if (CurrentInputElement.Data == "(")
                {
                    contents.AddRange(GetConditionalBracket());
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
