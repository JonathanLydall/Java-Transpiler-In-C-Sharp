using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Expressions
{
    public class UnaryExpression : AstNode
    {
        public IList<IAstNode> Condition = new List<IAstNode>();

        public IList<IAstNode> ValueIfTrue = new List<IAstNode>();

        public IList<IAstNode> ValueIfFalse = new List<IAstNode>();

        public override string DebugOut()
        {
            var condition = Condition
                .Select(x => x.DebugOut())
                .Aggregate((x, y) => x + y);

            var valueIfTrue = ValueIfTrue
                .Select(x => x.DebugOut())
                .Aggregate((x, y) => x + y);

            var valueIfFalse = ValueIfFalse
                .Select(x => x.DebugOut())
                .Aggregate((x, y) => x + y);

            return string.Format("{0} ? {1} : {2}", condition, valueIfTrue, valueIfFalse);
        }

    }
}
