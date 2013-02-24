using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Declarations
{
    public class VariableDeclaration : AstNode
    {
        public bool HasInitialization = false;

        public bool IsArray = false;

        public IList<IInputElement> Modifiers = new List<IInputElement>();

        public IList<IInputElement> AssignedValue = new List<IInputElement>();

        public IInputElement VariableType;

        public IInputElement VariableName;

        public override string DebugOut()
        {
            var assignedValue = !HasInitialization ? string.Empty : " = " + AssignedValue.Select(x => x.Data).Aggregate((x, y) => x + y);
            var modifiers = Modifiers.Count == 0 ? string.Empty : Modifiers.Select(x => x.Data).Aggregate((x, y) => x + y) + " ";
            var array = IsArray ? "[]" : string.Empty;

            return string.Format("{0}{1} {2}{3}{4};", modifiers, VariableType.Data, VariableName.Data, array, assignedValue);
        }
    }
}
