﻿using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Loops
{
    public class DoWhileLoop : AstNode
    {
        public IList<IAstNode> Condition = new List<IAstNode>();

        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            var condition = 
                Condition
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + y);

            return string.Format("do {{ ... }} while ({0});", condition);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromAstNodes(Body)).ToList();
            returnList = returnList.Union(GetUsedTypesFromAstNodes(Condition)).ToList();

            return returnList;
        }
    }
}
