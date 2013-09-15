using Mordritch.Transpiler.Java.AstGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Declarations
{
    public class ClassInitializerDeclaration : AstNode
    {
        public bool IsStatic = false;

        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            return string.Empty;
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromAstNodes(Body)).ToList();

            return returnList;
        }
    }
}
