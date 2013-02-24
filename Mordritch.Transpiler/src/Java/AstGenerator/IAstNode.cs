using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator
{
    public interface IAstNode
    {
        string DebugOut();

        CommentInputElement PreComment { get; set; }

        CommentInputElement PostComment { get; set; }
    }
}
