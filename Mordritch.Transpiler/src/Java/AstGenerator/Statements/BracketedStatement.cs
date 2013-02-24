using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    class BracketedStatement : AstNode
    {
        public IList<IAstNode> Content = new List<IAstNode>();

        public IInputElement CastTo = null;
        
        public override string DebugOut()
        {
            var castTo = CastTo != null
                ? string.Format("({0})", CastTo.Data)
                : string.Empty;
            
            var content = Content.Count > 0
                ? Content
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + y)
                : string.Empty;

            return string.Format("{0}({1})", castTo, content);
        }
    }
}
