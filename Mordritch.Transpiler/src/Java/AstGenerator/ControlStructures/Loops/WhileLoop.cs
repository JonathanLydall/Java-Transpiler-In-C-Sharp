using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.ControlStructures
{
    public class WhileLoop : AstNode
    {
        public IList<IInputElement> Condition = new List<IInputElement>();

        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            return string.Format("while ({0}) {{...", Condition.Select(x => x.Data).Aggregate((x, y) => x + " " + y));
        }
    }
}
