using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class ThrowStatement : AstNode
    {
        public IList<IInputElement> ExceptionInstance = new List<IInputElement>();

        public override string DebugOut()
        {
            return ExceptionInstance.Select(x => x.Data).Aggregate((x, y) => x + " " + y) + ";";
        }
    }
}
