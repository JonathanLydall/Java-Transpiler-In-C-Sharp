using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.ControlStructures
{
    public class ForLoopInitializer
    {
        public IInputElement InitializedType = null;

        public OperatorToken OperatorToken = null;

        public IInputElement VariableName = null;

        public IList<IAstNode> AssignedValue = new List<IAstNode>();

        public string DebugOut()
        {
            var initializedType
                = InitializedType != null
                    ? string.Format("{0} ", InitializedType.Data)
                    : string.Empty;

            var assignedValue
                = AssignedValue
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + " " + y);

            return string.Format("{0}{1} = {2}", initializedType, VariableName.Data, assignedValue);
        }
    }
}
