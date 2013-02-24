using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class JumpStatement : AstNode
    {
        public IInputElement Statement = null;

        public IdentifierToken JumpTo = null;

        public override string DebugOut()
        {
            throw new NotImplementedException();
        }
    }
}
