using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class ReturnStatement : AstNode
    {
        public IList<IAstNode> ReturnValue = new List<IAstNode>();

        public override string DebugOut()
        {
            if (ReturnValue.Count == 0)
            {
                return "return;";
            }
            
            var returnValue =
                ReturnValue
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + y);

            return string.Format("return {0};", returnValue);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromAstNodes(ReturnValue)).ToList();

            return returnList;
        }
    }
}
