using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.ControlStructures
{
    public class ForLoopInitializer
    {
        public IInputElement InitializedType = null;

        public IInputElement VariableName = null;

        public IList<IInputElement> AssignedValue = new List<IInputElement>();

        public string DebugOut()
        {
            var initializedType
                = InitializedType != null
                    ? string.Format("{0} ", InitializedType.Data)
                    : string.Empty;

            var assignedValue
                = AssignedValue
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + " " + y);

            return string.Format("{0}{1} = {2}", initializedType, VariableName.Data, assignedValue);
        }
    }
}
