using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class AssertStatement : AstNode
    {
        public IList<IInputElement> Message = null;

        public IList<IInputElement> Condition = new List<IInputElement>();

        public override string DebugOut()
        {
            throw new NotImplementedException();
        }
    }
}
