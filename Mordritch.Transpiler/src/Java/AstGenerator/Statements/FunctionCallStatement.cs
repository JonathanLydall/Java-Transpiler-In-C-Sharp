using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class FunctionCallStatement : AstNode
    {
        public IList<IInputElement> Method = new List<IInputElement>();

        public IList<IList<IAstNode>> Parameters = new List<IList<IAstNode>>();

        public IInputElement CastTo = null;

        public bool IsStandaloneStatement = false;

        public override string DebugOut()
        {
            return DebugOut(false);
        }

        public string DebugOut(bool forNested = false)
        {
            var castTo = CastTo != null
                ? string.Format("({0})", CastTo.Data)
                : string.Empty;
            
            var method = 
                Method
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + y);
            
            var parameters = Parameters.Count == 0 
                ? string.Empty 
                : Parameters
                    .Select(xx => xx
                        .Select(x => x.DebugOut())
                        .Aggregate((x, y) => x + y))
                    .Aggregate((x, y) => x + ", " + y);

            return string.Format("{0}{1}({2}){3}", castTo, method, parameters, forNested ? string.Empty : ";");
        }
    }
}
