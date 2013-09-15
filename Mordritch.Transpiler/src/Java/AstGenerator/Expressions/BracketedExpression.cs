using Mordritch.Transpiler.Java.AstGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Expressions
{
    public class BracketedExpression : AstNode
    {
        public IList<IAstNode> InnerExpressions = new List<IAstNode>();

        public override string DebugOut()
        {
            var innerExpressions = InnerExpressions.Count > 0
                ? InnerExpressions
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + y)
                : string.Empty;

            return string.Format("({0})", innerExpressions);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromAstNodes(InnerExpressions)).ToList();

            return returnList;
        }
    }
}
