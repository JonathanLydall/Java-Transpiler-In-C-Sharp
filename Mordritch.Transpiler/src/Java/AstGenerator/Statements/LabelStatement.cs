using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class LabelStatement : AstNode
    {
        public IInputElement Name;

        public override string DebugOut()
        {
            return string.Format("{0}:", Name.Data);
        }
    }
}
