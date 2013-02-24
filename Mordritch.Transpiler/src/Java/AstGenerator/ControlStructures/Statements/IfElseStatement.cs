using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class IfElseStatement : AstNode
    {
        public IList<IInputElement> Condition = new List<IInputElement>();

        public IList<IAstNode> Body = new List<IAstNode>();

        public IList<IfElseStatement> ConditionalElses = new List<IfElseStatement>();

        public IList<IAstNode> ElseBody = new List<IAstNode>();

        public override string DebugOut()
        {
            return string.Format("if ({0}) {{...", Condition.Select(x => x.Data).Aggregate((x, y) => x + " " + y));
        }
    }
}
