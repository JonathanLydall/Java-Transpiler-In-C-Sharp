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
using Mordritch.Transpiler.Java.AstGenerator.Expressions;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class ClassInstantiationExpressionParser : Parser, IParser
    {
        private ClassInstantiationExpression _classInstantiationExpression = new ClassInstantiationExpression();

        public override IAstNode ImplementationSpecificParse()
        {
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.New);
            MoveToNextToken();

            _classInstantiationExpression.ClassName = CurrentInputElement;
            MoveToNextToken();

            while (CurrentInputElement.Data == "[")
            {
                Debug.Assert(CurrentInputElement.Data == "[");
                MoveToNextToken();

                _classInstantiationExpression.ArraySizes.Add(GetInnerExpression("]"));

                Debug.Assert(CurrentInputElement.Data == "]");
                MoveToNextToken();
            }

            if (CurrentInputElement.Data == "{")
            {
                _classInstantiationExpression.ArrayContents = GetArrayInitialization();
            }
            else if (CurrentInputElement.Data == "(")
            {
                Debug.Assert(CurrentInputElement.Data == "(");
                MoveToNextToken();

                while (CurrentInputElement.Data != ")")
                {
                    _classInstantiationExpression.InitializationData.Add(ParseExpressionParameter());
                }

                Debug.Assert(CurrentInputElement.Data == ")");
                MoveToNextToken();
            }
            else if (CurrentInputElement.Data != ";" && CurrentInputElement.Data != ")")
            {
                throw new Exception("Unexpected pattern.");
            }

            return _classInstantiationExpression;
        }

        private ArrayInitializationExpression GetArrayInitialization()
        {
            var arrayInitializationExpression = new ArrayInitializationExpression();

            Debug.Assert(CurrentInputElement.Data == "{");
            MoveToNextToken();

            while (CurrentInputElement.Data != "}")
            {
                if (CurrentInputElement.Data == "{")
                {
                    arrayInitializationExpression.Contents.Add(GetArrayInitialization());
                }
                else
                {
                    arrayInitializationExpression.Contents.Add(new ParameterExpression { AstNodes = ParseExpressionParameter() });
                }

                if (CurrentInputElement.Data == ",")
                {
                    MoveToNextToken();
                }
            }

            Debug.Assert(CurrentInputElement.Data == "}");
            MoveToNextToken();

            return arrayInitializationExpression;
        }
    }
}
