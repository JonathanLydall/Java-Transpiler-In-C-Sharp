using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class ValueStatement : AstNode
    {
        public IInputElement Content = null;

        public IInputElement CastTo = null;

        public override string DebugOut()
        {
            var castTo = CastTo != null
                ? string.Format("({0})", CastTo.Data)
                : string.Empty;

            var content = Content.Data;

            return string.Format("{0}{1}", castTo, content);
        }
    }
}
