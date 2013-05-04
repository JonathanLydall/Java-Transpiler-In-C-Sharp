using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.ControlStructures
{
    public class WhileLoop : AstNode
    {
        public IList<IAstNode> Condition = new List<IAstNode>();

        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            var condition =
                Condition
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + y);
            

            return string.Format("while ({0}) {{...", condition);
        }
    }
}
