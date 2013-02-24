using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class MethodDeclarationParser : Parser, IParser
    {
        private MethodDeclaration _methodDeclaration = new MethodDeclaration();

        public override IAstNode ImplementationSpecificParse()
        {
            GetStart();
            GetArgument();
            _methodDeclaration.Body = ParseBody();

            return _methodDeclaration;
        }

        private void GetArgument()
        {
            var arguments = new List<IList<IInputElement>>();
            var currentArgument = new List<IInputElement>();

            AssertSeperator("(");
            MoveToNextToken();

            //no arguments
            if (CurrentInputElement.Data == ")")
            {
                MoveToNextToken();
                return;
            }

            while (true)
            {
                if (CurrentInputElement.Data == ",")
                {
                    arguments.Add(currentArgument);
                    currentArgument = new List<IInputElement>();
                    MoveToNextToken();
                    continue;
                }

                if (CurrentInputElement.Data == ")")
                {
                    arguments.Add(currentArgument);
                    currentArgument = new List<IInputElement>();
                    MoveToNextToken();
                    break;
                }

                currentArgument.Add(CurrentInputElement);
                MoveToNextToken();
            }

            foreach (var argument in arguments)
            {
                var methodArgument = new MethodArgument();
                for (var i = 0; i < argument.Count; i++)
                {
                    if (Keywords.MethodModifiers.Any(m => m == argument[i].Data))
                    {
                        methodArgument.Modifier = (KeywordToken)argument[i];
                        continue;
                    }

                    if (argument.Count >= i + 2 && argument[i].Data == "." && argument[i + 1].Data == "." && argument[i + 2].Data == ".")
                    {
                        methodArgument.IsVariableArity = true;
                        i += 2;
                        continue;
                    }

                    if (methodArgument.Type == null)
                    {
                        methodArgument.Type = (TokenInputElement)argument[i];
                        continue;
                    }

                    if (methodArgument.Name == null)
                    {
                        methodArgument.Name = (TokenInputElement)argument[i];
                        continue;
                    }

                    throw new Exception("Enexpected token.");
                }
                _methodDeclaration.Arguments.Add(methodArgument);
            }
        }

        private void GetStart()
        {
            //Is a class constructor, EG: protected ClassName(int par1)
            if (IsMethodModifier(CurrentInputElement) && IsWhiteSpaceElement(ForwardInputElement(1)) && IsIdentifier(ForwardInputElement(2)) && IsSeperator(ForwardInputElement(3)) && ForwardInputElement(3).Data == "(")
            {
                _methodDeclaration.Modifiers.Add((TokenInputElement)CurrentInputElement);
                MoveToNextToken();
                _methodDeclaration.Name = (TokenInputElement)CurrentInputElement;
                MoveToNextInputElement();
                AssertSeperator("(");
                return;
            }

            while (!(IsSeperator(ForwardInputElement(3)) && ForwardInputElement(3).Data == "("))
            {
                _methodDeclaration.Modifiers.Add((TokenInputElement)CurrentInputElement);
                MoveToNextToken();
            }

            _methodDeclaration.ReturnType = (TokenInputElement)CurrentInputElement;
            MoveToNextToken();
            _methodDeclaration.Name = (TokenInputElement)CurrentInputElement;
            MoveToNextToken();
            AssertSeperator("(");
        }
    }
}
