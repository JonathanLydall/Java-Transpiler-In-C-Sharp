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
    public class FunctionCallParser : Parser, IParser
    {
        public override IAstNode ImplementationSpecificParse()
        {
            return ParseFunctionCall();
        }

        private FunctionCallStatement ParseFunctionCall(IInputElement castTo = null)
        {
            var functionCallStatement = new FunctionCallStatement();
            functionCallStatement.Method = ParseMethod();
            functionCallStatement.Parameters = ParseParameters();

            if (castTo != null)
            {
                functionCallStatement.CastTo = castTo;
            }

            return functionCallStatement;
        }

        private IList<IInputElement> ParseMethod()
        {
            var method = new List<IInputElement>();

            if (IsWhiteSpace)
            {
                MoveToNextToken();
            }

            while (CurrentInputElement.Data != "(")
            {
                method.Add(CurrentInputElement);
                MoveToNextToken();
            }

            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextToken();

            return method;
        }

        private IList<IList<IAstNode>> ParseParameters()
        {
            var parameters = new List<IList<IAstNode>>();
            
            while (CurrentInputElement.Data != ")")
            {
                parameters.Add(ParseParameter());
            }

            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextToken();
            return parameters;
        }

        private IList<IAstNode> ParseParameter()
        {
            var parameter = new List<IAstNode>();
            IInputElement castTo = null;

            while (true)
            {
                if (IsOperator())
                {
                    parameter.Add(new Operator { Content = CurrentInputElement });
                    MoveToNextToken();
                    continue;
                }

                if (IsCast())
                {
                    Debug.Assert(CurrentInputElement.Data == "(");
                    MoveToNextToken();
                    
                    castTo = CurrentInputElement;
                    MoveToNextToken();

                    Debug.Assert(CurrentInputElement.Data == ")");
                    MoveToNextToken();
                    continue;
                }

                if (IsBracketedStatement())
                {
                    Debug.Assert(BufferSize == 0);
                    Debug.Assert(CurrentInputElement.Data == "(");
                    MoveToNextToken();

                    parameter.Add(GetBracketedStatement(castTo));
                    castTo = null;
                    continue;
                }

                if (IsNestedFunctionCall())
                {
                    ResetBuffer();
                    parameter.Add(ParseFunctionCall(castTo));
                    castTo = null;
                    continue;
                }

                if (IsValueStatement())
                {
                    var content = new ValueStatement { Content = CurrentInputElement, CastTo = castTo };
                    castTo = null;

                    parameter.Add(content);

                    MoveToNextToken();
                    continue;
                }

                if (CurrentInputElement.Data == "," || CurrentInputElement.Data == ")")
                {
                    break;
                }

                BufferSize++;
                MoveToNextToken();
            }

            if (CurrentInputElement.Data == ",")
            {
                MoveToNextToken();
            }

            return parameter;
        }

        private IAstNode GetBracketedStatement(IInputElement castTo)
        {
            var bracketedStatement = new BracketedStatement();
            if (castTo != null)
            {
                bracketedStatement.CastTo = castTo;
            }

            bracketedStatement.Content = ParseParameter();
            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextToken();

            return bracketedStatement;
        }


        private bool IsOperator()
        {
            return CurrentInputElement is OperatorToken;
        }

        private bool IsNestedFunctionCall()
        {
            return CurrentInputElement.Data == "(" && PreviousNonWhiteSpaceInputElement is IdentifierToken;
        }

        private bool IsCast()
        {
            return CurrentInputElement.Data == "(" && !(PreviousNonWhiteSpaceInputElement is IdentifierToken) && ForwardToken(2).Data == ")";
        }

        private bool IsValueStatement()
        {
            return !IsOperator() && !IsNestedFunctionCall() && !IsCast() && !IsBracketedStatement() && CurrentInputElement.Data != "," && CurrentInputElement.Data != ")" && ForwardToken(1).Data != "." && ForwardToken(1).Data != "(";
        }

        private bool IsBracketedStatement()
        {
            return CurrentInputElement.Data == "(" && !(PreviousNonWhiteSpaceInputElement is IdentifierToken) && ForwardToken(2).Data != ")";
        }
    }
}
