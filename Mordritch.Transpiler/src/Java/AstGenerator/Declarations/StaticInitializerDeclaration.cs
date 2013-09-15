using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Declarations
{
    public class StaticInitializerDeclaration : AstNode
    {
        public IList<IAstNode> Body = new List<IAstNode>();

        public IInputElement StaticKeywordToken;
        
        public override string DebugOut()
        {
            return "static {...";
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromAstNodes(Body)).ToList();

            return returnList;
        }
    }
}
