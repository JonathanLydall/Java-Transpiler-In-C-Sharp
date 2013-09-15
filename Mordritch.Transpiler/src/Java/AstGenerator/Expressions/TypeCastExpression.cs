using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Expressions
{
    public class TypeCastExpression : AstNode
    {
        public IInputElement CastTarget;
        
        public IList<IAstNode> InnerExpressions = new List<IAstNode>();

        public override string DebugOut()
        {
            var innerExpressions = InnerExpressions.Count > 0
                ? InnerExpressions
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + y)
                : string.Empty;

            var castTarget = CastTarget.Data;

            return string.Format("({0}){1}", castTarget, innerExpressions);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromAstNodes(InnerExpressions)).ToList();
            AddUsedTypeIfIdentifierToken(CastTarget, returnList);

            return returnList;
        }
    }
}
