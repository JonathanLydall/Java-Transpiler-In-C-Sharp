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
    public class MethodDeclarationParser : Parser, IParser
    {
        private MethodDeclaration _methodDeclaration = new MethodDeclaration();

        public override IAstNode ImplementationSpecificParse()
        {
            GetStart();
            GetArgument();

            if (CurrentInputElement.Data == Keywords.Throws)
            {
                GetThrows();
            }

            if (_methodDeclaration.Modifiers.Any(x => x.Data == Keywords.Abstract) && CurrentInputElement.Data == ";")
            {
                MoveToNextToken();
            }
            else
            {
                _methodDeclaration.Body = ParseBody();
            }

            return _methodDeclaration;
        }

        public static bool IsMatch(Parser parser)
        {
            if (parser.CurrentInputElement.Data == ")" && parser.NextNonWhiteSpaceInputElement.Data == "{")
            {
                return true;
            }

            // Catches pattern like: public void checkSessionLock() throws MinecraftException
            if (
                parser.CurrentInputElement.Data == ")" &&
                parser.ForwardToken(1).Data == Keywords.Throws &&
                parser.ForwardToken(2) is TokenInputElement &&
                parser.ForwardToken(3).Data == "{")
            {
                return true;
            }


            var buffer = parser.GetBufferElements();

            /*
             * TODO: A more correct way to solve this problem would be for the parser to be aware of its current context,
             * like if it's in the context of parsing a class, or being inside of a method. However, this is something
             * I only thought of in hindsight, so hopefully for now I can get away with doing it this way.
             * 
             * By the way, without catching the abstract keyword, it was being mis-recognized as a simple statement.
             */
            if (
                ((parser.CurrentInputElement.Data == ")" && parser.NextNonWhiteSpaceInputElement.Data == ";") ||
                (parser.CurrentInputElement.Data == "." && parser.PreviousNonWhiteSpaceInputElement.Data == ")")) &&
                buffer.Any(x => x.Data == Keywords.Abstract))
            {
                return true;
            }


            return false;
        }

        private void GetThrows()
        {
            Debug.Assert(CurrentInputElement.Data == Keywords.Throws);
            MoveToNextToken();
            _methodDeclaration.ThrowsType = (TokenInputElement)CurrentInputElement;
            MoveToNextToken();
        }

        private void GetArgument()
        {
            var arguments = new List<IList<IInputElement>>();
            var currentArgument = new List<IInputElement>();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "(");
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
            while (
                IsMethodModifier(CurrentInputElement) ||
                CurrentInputElement.Data == Keywords.Public ||
                CurrentInputElement.Data == Keywords.Private)
            {
                _methodDeclaration.Modifiers.Add((TokenInputElement)CurrentInputElement);
                MoveToNextToken();
            }

            // Is it a constructor? EG: public <Class Name>()
            if (
                CurrentInputElement is TokenInputElement &&
                ForwardToken(1).Data == "(")
            {
                _methodDeclaration.Name = (TokenInputElement)CurrentInputElement;
                MoveToNextToken();
                return;
            }

            Debug.Assert(CurrentInputElement is TokenInputElement);
            _methodDeclaration.ReturnType = (TokenInputElement)CurrentInputElement;
            MoveToNextToken();

            while (CurrentInputElement.Data == "[")
            {
                Debug.Assert(CurrentInputElement.Data == "[");
                MoveToNextToken();
                
                Debug.Assert(CurrentInputElement.Data == "]");
                MoveToNextToken();
                
                _methodDeclaration.ReturnArrayDepth++;
            }

            Debug.Assert(CurrentInputElement is TokenInputElement);
            _methodDeclaration.Name = (TokenInputElement)CurrentInputElement;
            MoveToNextToken();
            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "(");
        }
    }
}
