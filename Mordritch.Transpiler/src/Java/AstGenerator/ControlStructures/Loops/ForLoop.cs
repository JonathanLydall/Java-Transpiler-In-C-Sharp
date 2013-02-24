using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.ControlStructures
{
    public class ForLoop : AstNode
    {
        public IList<ForLoopInitializer> Initializers = new List<ForLoopInitializer>();

        public IList<IInputElement> Condition = new List<IInputElement>();

        public IList<IList<IInputElement>> CounterExpressions = new List<IList<IInputElement>>();

        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            var condition = Condition.Select(x => x.Data).Aggregate((x, y) => x + " " + y);
            
            var initializers = Initializers.Count > 0 
                ? Initializers
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + ", " + y)
                : string.Empty;

            var counterExpressions = CounterExpressions.Count > 0
                ? CounterExpressions
                    .Select(xx => xx
                        .Select(x => x.Data)
                        .Aggregate((x, y) => x + " " + y))
                    .Aggregate((x, y) => x + ", " + y)
                : string.Empty;
            
            return string.Format("for ({0};{1};{2}) {{...", initializers, condition, counterExpressions);
        }
    }
}
