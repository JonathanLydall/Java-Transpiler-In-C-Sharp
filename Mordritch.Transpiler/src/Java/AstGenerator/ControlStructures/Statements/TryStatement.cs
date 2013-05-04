﻿using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Statements
{
    public class TryStatement : AstNode
    {
        public IList<IAstNode> TryBody = new List<IAstNode>();

        public IList<CatchStatement> CatchStatements = new List<CatchStatement>();

        public IList<IAstNode> FinallyBody = null;

        public override string DebugOut()
        {
            return "try {...";
        }
    }
}
