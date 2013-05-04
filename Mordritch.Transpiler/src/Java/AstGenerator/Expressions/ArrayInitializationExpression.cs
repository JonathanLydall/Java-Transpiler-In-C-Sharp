using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Expressions
{
    public class ArrayInitializationExpression : AstNode
    {
        public IList<IAstNode> Contents = new List<IAstNode>();

        public override string DebugOut()
        {
            var arrayContents = Contents.Count == 0
                ? string.Empty
                : Contents
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + ", " + y);
            
            return string.Format("{{ {0} }}", arrayContents);
        }
    }
}
