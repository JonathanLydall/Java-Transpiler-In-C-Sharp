using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
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

            AssertSeperator(";");
            MoveToNextToken();

            return _variableDeclaration;
        }

        private void ProcessInitialization()
        {
            AssertOperator("=");
            MoveToNextInputElement();

            _variableDeclaration.HasInitialization = true;

            if (IsWhiteSpace)
            {
                MoveToNextInputElement();
            }

            while (CurrentInputElement.Data != ";")
            {
                _variableDeclaration.AssignedValue.Add(CurrentInputElement);
                MoveToNextInputElement();
            }
        }

        private void ProcessVariableName()
        {
            AssertIdentifier();
            _variableDeclaration.VariableName = CurrentInputElement;
            MoveToNextInputElement();

            if (IsWhiteSpace)
            {
                MoveToNextInputElement();
            }
        }
        
        private void ProcessModifiers()
        {
            while ((!IsPrimitive(CurrentInputElement) && !IsIdentifier(CurrentInputElement)) || IsWhiteSpace || IsComment)
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
            if (!IsPrimitive(CurrentInputElement) && !IsIdentifier(CurrentInputElement))
            {
                throw new Exception(string.Format("Expected primitive or identifier, instead found '{0}'.", CurrentInputElement.Data));
            }

            _variableDeclaration.VariableType = CurrentInputElement;
            MoveToNextInputElement();

            if (CurrentInputElement.Data == "[")
            {
                _variableDeclaration.IsArray = true;
                MoveToNextInputElement();

                AssertSeperator("]");
                MoveToNextInputElement();
            }

            AssertWhiteSpace();
            MoveToNextInputElement();
        }
    }
}
