using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Assignments
{
    public class VariableAssignment : AstNode
    {
        public IList<IAstNode> AssignedValue = new List<IAstNode>();

        public IList<IInputElement> VariableName = new List<IInputElement>();

        public IList<IInputElement> Modifiers = new List<IInputElement>();

        public IInputElement AssignmentOperator = null;

        public override string DebugOut()
        {
            var variableName = VariableName.Count == 0
                ? string.Empty 
                : VariableName
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + y);

            var assignmentOperator = AssignmentOperator == null
                ? string.Empty
                : string.Format(" {0} ", AssignmentOperator.Data);

            var assignedValue = AssignmentOperator == null
                ? string.Empty
                : " = " + AssignedValue
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + " " + y);

            return string.Format("{0}{1}{2};", variableName, assignmentOperator, assignedValue);
        }
    }
}
