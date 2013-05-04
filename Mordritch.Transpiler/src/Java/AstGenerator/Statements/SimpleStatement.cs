using Mordritch.Transpiler.Java.AstGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    /// <summary>
    /// Intended to store statements which are simple, like a method call (although the method call could have
    /// casting, and nested method calls).
    /// </summary>
    public class SimpleStatement : AstNode
    {
        public IList<IAstNode> Expressions = new List<IAstNode>();

        public override string DebugOut()
        {
            var expressions =
                Expressions
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + y);

            return string.Format("{0};", expressions);
        }
    }
}
