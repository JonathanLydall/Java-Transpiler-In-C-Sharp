using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class VariableDeclarationParser : Parser, IParser
    {
        private VariableDeclaration _variableDeclaration = new VariableDeclaration();

        public override IAstNode ImplementationSpecificParse()
        {
            ProcessModifiers();
            ProcessVariableType();
            ProcessVariableName();

            if (CurrentInputElement.Data == "=")
            {
                ProcessInitialization();
            }

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextToken();

            return _variableDeclaration;
        }

        private void ProcessModifiers()
        {
            while ((!IsPrimitive(CurrentInputElement) && !(CurrentInputElement is IdentifierToken)) || IsWhiteSpace || IsComment)
            {
                if (IsWhiteSpace)
                {
                    MoveToNextInputElement();
                    continue;
                }

                if (IsComment)
                {
                    MoveToNextInputElement();
                    continue;
                }

                _variableDeclaration.Modifiers.Add(CurrentInputElement);
                MoveToNextInputElement();
            }
        }

        private void ProcessVariableType()
        {
            if (!IsPrimitive(CurrentInputElement) && !(CurrentInputElement is IdentifierToken))
            {
                throw new Exception(string.Format("Expected primitive or identifier, instead found '{0}'.", CurrentInputElement.Data));
            }

            _variableDeclaration.VariableType = CurrentInputElement;
            MoveToNextInputElement();

            while (CurrentInputElement.Data == "[")
            {
                _variableDeclaration.ArrayCount++;
                MoveToNextInputElement();

                Debug.Assert(CurrentInputElement is SeperatorToken);
                Debug.Assert(CurrentInputElement.Data == "]");
                MoveToNextInputElement();
            }

            Debug.Assert(CurrentInputElement is WhiteSpaceInputElement);
            MoveToNextInputElement();
        }

        private void ProcessVariableName()
        {
            Debug.Assert(CurrentInputElement is IdentifierToken);
            _variableDeclaration.VariableName = CurrentInputElement;
            MoveToNextInputElement();

            if (IsWhiteSpace)
            {
                MoveToNextInputElement();
            }
        }

        private void ProcessInitialization()
        {
            Debug.Assert(CurrentInputElement is OperatorToken);
            Debug.Assert(CurrentInputElement.Data == "=");
            MoveToNextToken();

            _variableDeclaration.HasInitialization = true;

            while (CurrentInputElement.Data != ";")
            {
                _variableDeclaration.AssignedValue.Add(ParseExpression());
            }
        }
    }
}
