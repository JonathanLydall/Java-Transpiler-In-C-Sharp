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
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Loops;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class DoWhileLoopParser : Parser, IParser
    {
        private DoWhileLoop _doWhileLoop = new DoWhileLoop();

        public override IAstNode ImplementationSpecificParse()
        {
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.Do);
            MoveToNextToken();

            _doWhileLoop.Body = GetBody();

            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.While);
            MoveToNextToken();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextToken();

            _doWhileLoop.Condition = GetInnerExpression(")");

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextToken();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextToken();

            return _doWhileLoop;
        }

        private IList<IAstNode> GetBody()
        {
            if (CurrentInputElement is SeperatorToken && CurrentInputElement.Data == "{")
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
    }
}
