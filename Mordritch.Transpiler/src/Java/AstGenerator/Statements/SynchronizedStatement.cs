﻿using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class SynchronizedStatement : AstNode
    {
        public IList<IInputElement> LockObject = new List<IInputElement>();

        public IList<IInputElement> Modifiers = new List<IInputElement>();

        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            var syncedStatement = LockObject.Select(x => x.Data).Aggregate((x, y) => x + y);
            return string.Format("synchronized {0} {{...", syncedStatement);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromAstNodes(Body)).ToList();

            return returnList;
        }
    }
}
