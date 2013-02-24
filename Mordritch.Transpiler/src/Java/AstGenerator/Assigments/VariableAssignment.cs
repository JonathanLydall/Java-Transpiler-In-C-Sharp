using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Assignments
{
    public class VariableAssignment : AstNode
    {
        public IList<IInputElement> AssignedValue = new List<IInputElement>();

        public IList<IInputElement> VariableName = new List<IInputElement>();

        public override string DebugOut()
        {
            var variableName = VariableName.Count == 0 
                ? string.Empty 
                : VariableName
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + y);
            var assignedValue = AssignedValue.Count == 0 
                ? string.Empty 
                : string.Format(
                    " = {0}", 
                    AssignedValue
                        .Select(x => x.Data)
                        .Aggregate((x, y) => x + y));

            return string.Format("{0}{1};", variableName, assignedValue);
        }
    }
}
