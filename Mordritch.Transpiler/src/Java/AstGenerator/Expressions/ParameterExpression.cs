using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Expressions
{
    public class ParameterExpression : AstNode
    {
        public IList<IAstNode> AstNodes;
        
        public override string DebugOut()
        {
            var nodes = AstNodes
                .Select(x => x.DebugOut())
                .Aggregate((x, y) => x + " " + y);

            return nodes;
        }
    }
}
