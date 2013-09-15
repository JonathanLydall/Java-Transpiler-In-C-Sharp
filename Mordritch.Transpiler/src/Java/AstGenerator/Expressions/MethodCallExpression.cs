using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Expressions
{
    public class MethodCallExpression : AstNode
    {
        public IInputElement MethodIdentifier;

        public IList<IList<IAstNode>> Parameters = new List<IList<IAstNode>>();

        public override string DebugOut()
        {
            return DebugOut(false);
        }

        public string DebugOut(bool forNested = false)
        {
            var parameters = Parameters.Count == 0
                ? string.Empty
                : Parameters
                    .Select(xx => xx
                        .Select(x => x.DebugOut())
                        .Aggregate((x, y) => x + y))
                    .Aggregate((x, y) => x + ", " + y);

            return string.Format("{0}({1})", MethodIdentifier.Data, parameters);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

#warning This is will return locally defined variables too.
            
            foreach (var parameter in Parameters)
            {
                returnList = returnList.Union(GetUsedTypesFromAstNodes(parameter)).ToList();
            }

            return returnList;
        }
    }
}
