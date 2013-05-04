using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Declarations
{
    public class MethodDeclaration : AstNode
    {
        public IList<MethodArgument> Arguments = new List<MethodArgument>();

        public TokenInputElement ReturnType = null;

        public IList<IAstNode> Body = null;

        public TokenInputElement ThrowsType = null;

        public TokenInputElement Name = null;

        public IList<TokenInputElement> Modifiers = new List<TokenInputElement>();

        public int ReturnArrayDepth = 0;

        public override string DebugOut()
        {
            var modifiers = Modifiers.Count == 0 ? string.Empty : Modifiers.Select(x => x.Data).Aggregate((x, y) => x + " " + y);
            var methodArguments = Arguments.Count == 0 ? string.Empty : Arguments.Select(x => x.DebugOut()).Aggregate((x, y) => x + ", " + y);
            var returnType = ReturnType == null ? string.Empty : ReturnType.Data;
            
            return string.Format("{0} {1} {2}({3})", modifiers, returnType, Name.Data, methodArguments);
        }
    }
}
