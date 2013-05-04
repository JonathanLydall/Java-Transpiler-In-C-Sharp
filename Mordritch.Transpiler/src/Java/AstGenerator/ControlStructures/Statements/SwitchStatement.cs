using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.ControlStructures
{
    public class SwitchStatement : AstNode
    {
        public IList<IAstNode> ControlStatement = new List<IAstNode>();

        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            var controlStatement =
                ControlStatement
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + y);

            return string.Format("switch ({0}) {{...", controlStatement);
        }
    }
}
