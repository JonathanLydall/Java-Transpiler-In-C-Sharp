﻿using Mordritch.Transpiler.Java.AstGenerator.Assignments;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
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
    public class VariableAssignmentParser : Parser, IParser
    {
        private VariableAssignment _variableAssignment = new VariableAssignment();

        public override IAstNode ImplementationSpecificParse()
        {
            Debug.Assert(!(CurrentInputElement is WhiteSpaceInputElement));

            if (CurrentInputElement is CommentInputElement && ((CommentInputElement)CurrentInputElement).IsMultilineComment)
            {
                _variableAssignment.PreComment = (CommentInputElement)CurrentInputElement;
                MoveToNextToken();
            }

            ProcessVariableName();

            if (JavaUtils.IsAssignmentOperator(CurrentInputElement.Data))
            {
                _variableAssignment.AssignmentOperator = CurrentInputElement;
                MoveToNextToken();

                while (CurrentInputElement.Data != ";")
                {
                    _variableAssignment.AssignedValue.Add(ParseExpression());
                }
            }

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == ";");
            MoveToNextToken();
            
            return _variableAssignment;
        }

        private void ProcessVariableName()
        {
            while (!JavaUtils.IsAssignmentOperator(CurrentInputElement.Data) && CurrentInputElement.Data != ";")
            {
                _variableAssignment.VariableName.Add(CurrentInputElement);
                MoveToNextToken();
            }
        }
    }
}
