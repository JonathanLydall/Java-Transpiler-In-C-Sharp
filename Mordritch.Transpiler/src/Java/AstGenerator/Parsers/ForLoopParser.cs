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
    public class ForLoopParser : Parser, IParser
    {
        private ForLoop _forControlStructure = new ForLoop();

        public override IAstNode ImplementationSpecificParse()
        {
            _forControlStructure.PreComment = GetComment();

            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.For);
            MoveToNextToken();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextToken();

            _forControlStructure.Initializers = GetInitializers();
            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextToken();

            _forControlStructure.Condition = GetInnerExpression(";");
            
            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextToken();

            _forControlStructure.CounterExpressions = GetCounterExpressions();
            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextToken();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "{");
            _forControlStructure.Body = ParseBody();

            return _forControlStructure;
        }

        private IList<SimpleStatement> GetCounterExpressions()
        {
            var counterExpressions = new List<SimpleStatement>();

            while (CurrentInputElement.Data != ")")
            {
                var simpleStatement = new SimpleStatement();

                while (CurrentInputElement.Data != "," && CurrentInputElement.Data != ")")
                {
                    simpleStatement.Expressions.Add(ParseExpression());
                }

                if (CurrentInputElement.Data != ")")
                {
                    Debug.Assert(CurrentInputElement.Data == ",");
                    MoveToNextToken();
                }

                counterExpressions.Add(simpleStatement);
            }

            return counterExpressions;
        }

        private IList<ForLoopInitializer> GetInitializers()
        {
            var initializers = new List<ForLoopInitializer>();
            
            while (CurrentInputElement.Data != ";")
            {
                var initializer = new ForLoopInitializer();

                if (ForwardToken(1) is OperatorToken && CurrentInputElement is IdentifierToken)
                {
                    initializer.VariableName = CurrentInputElement;
                    MoveToNextToken();
                }
                else if (ForwardToken(2) is OperatorToken && ForwardToken(1) is IdentifierToken)
                {
                    initializer.InitializedType = CurrentInputElement;
                    MoveToNextToken();
                    initializer.VariableName = CurrentInputElement;
                    MoveToNextToken();
                }
                else
                {
                    throw new Exception("Expected pattern 'type variableName <operator>...' or 'variableName <operator>...'");
                }

                Debug.Assert(CurrentInputElement is OperatorToken);
                initializer.OperatorToken = CurrentInputElement as OperatorToken;
                MoveToNextToken();

                while (CurrentInputElement.Data != "," && CurrentInputElement.Data != ";")
                {
                    initializer.AssignedValue.Add(CurrentInputElement);
                    MoveToNextToken();
                }

                if (CurrentInputElement.Data == ",")
                {
                    MoveToNextToken();
                }

                initializers.Add(initializer);
            }

            return initializers;
        }
    }
}
