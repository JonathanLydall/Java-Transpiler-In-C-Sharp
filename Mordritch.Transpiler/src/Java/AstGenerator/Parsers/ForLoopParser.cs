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

            Debug.Assert(CurrentInputElement is SeparatorToken);
            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextToken();

            _forControlStructure.Initializers = GetInitializers();
            Debug.Assert(CurrentInputElement is SeparatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextToken();

            _forControlStructure.Condition = GetCondition();
            Debug.Assert(CurrentInputElement is SeparatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextToken();

            _forControlStructure.CounterExpressions = GetCounterExpressions();
            Debug.Assert(CurrentInputElement is SeparatorToken);
            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextToken();

            Debug.Assert(CurrentInputElement is SeparatorToken);
            Debug.Assert(CurrentInputElement.Data == "{");
            _forControlStructure.Body = ParseBody();

            return _forControlStructure;
        }

        private IList<IList<IInputElement>> GetCounterExpressions()
        {
            var counterExpressions = new List<IList<IInputElement>>();
            
            while (CurrentInputElement.Data != ")")
            {
                var counterExpression = new List<IInputElement>();

                while (CurrentInputElement.Data != "," && CurrentInputElement.Data != ")")
                {
                    counterExpression.Add(CurrentInputElement);
                    MoveToNextInputElement();
                }

                if (CurrentInputElement.Data == ",")
                {
                    MoveToNextToken();
                }

                counterExpressions.Add(counterExpression);
            }

            return counterExpressions;
        }

        private IList<IInputElement> GetCondition()
        {
            var condition = new List<IInputElement>();
            
            while (CurrentInputElement.Data != ";")
            {
                condition.Add(CurrentInputElement);
                MoveToNextInputElement();
            }

            return condition;
        }

        private IList<ForLoopInitializer> GetInitializers()
        {
            var initializers = new List<ForLoopInitializer>();
            
            while (CurrentInputElement.Data != ";")
            {
                var initializer = new ForLoopInitializer();
                
                if (ForwardToken(1).Data == "=" && CurrentInputElement is IdentifierToken)
                {
                    initializer.VariableName = CurrentInputElement;
                    MoveToNextToken();
                }
                else if (ForwardToken(2).Data == "=" && ForwardToken(1) is IdentifierToken)
                {
                    initializer.InitializedType = CurrentInputElement;
                    MoveToNextToken();
                    initializer.VariableName = CurrentInputElement;
                    MoveToNextToken();
                }
                else
                {
                    throw new Exception("Expected pattern 'type variableName =...' or 'variableName =...'");
                }

                Debug.Assert(CurrentInputElement is OperatorToken);
                Debug.Assert(CurrentInputElement.Data == "=");
                MoveToNextToken();

                while (CurrentInputElement.Data != "," && CurrentInputElement.Data != ";")
                {
                    initializer.AssignedValue.Add(CurrentInputElement);
                    MoveToNextInputElement();
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
