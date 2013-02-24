using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class Operator : AstNode
    {
        public IInputElement Content = null;
        
        public override string DebugOut()
        {
            return string.Format(" {0} ", Content.Data);
        }
    }
}
