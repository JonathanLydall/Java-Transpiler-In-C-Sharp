using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Statements
{
    public class CatchStatement : AstNode
    {
        public IList<IInputElement> ExceptionTypes = new List<IInputElement>();

        public IInputElement ExceptionName;

        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            var exceptionTypes = ExceptionTypes.Count == 0
                ? string.Empty
                : ExceptionTypes
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + " | " + y);

            var exceptionName = ExceptionName.Data;
            
            return string.Format("catch ({0} {1}) {{...", exceptionTypes, exceptionName);
        }
    }
}
