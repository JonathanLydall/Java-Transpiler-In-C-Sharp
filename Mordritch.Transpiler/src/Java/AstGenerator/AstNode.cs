using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator
{
    public abstract class AstNode : IAstNode
    {
        public CommentInputElement PreComment { get; set; }

        public CommentInputElement PostComment { get; set; }

        public abstract string DebugOut();
    }
}
