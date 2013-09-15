using Mordritch.Transpiler.Java.AstGenerator.Statements;
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

        public IList<IAstNode> Condition = new List<IAstNode>();

        public IList<SimpleStatement> CounterExpressions = new List<SimpleStatement>();

        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            var condition =
                Condition
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + y);
            
            var initializers = Initializers.Count > 0 
                ? Initializers
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + ", " + y)
                : string.Empty;

            var counterExpressions = CounterExpressions.Count > 0
                ? CounterExpressions
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + ", " + y)
                : string.Empty;
            
            return string.Format("for ({0};{1};{2}) {{...", initializers, condition, counterExpressions);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            foreach (var forLoopInitializer in Initializers)
            {
                returnList = returnList.Union(forLoopInitializer.GetUsedTypes()).ToList();
            }

            returnList = returnList.Union(GetUsedTypesFromAstNodes(Condition)).ToList();

            foreach (var simpleStatement in CounterExpressions)
            {
                returnList = returnList.Union(simpleStatement.GetUsedTypes()).ToList();
            }

            returnList = returnList.Union(GetUsedTypesFromAstNodes(Body)).ToList();

            return returnList;
        }
    }
}
