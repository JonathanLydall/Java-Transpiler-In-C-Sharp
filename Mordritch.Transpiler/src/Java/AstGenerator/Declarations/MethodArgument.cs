using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Declarations
{
    public class MethodArgument : AstNode
    {
        public KeywordToken Modifier = null;

        public TokenInputElement Type = null;

        public TokenInputElement Name = null;

        public bool IsVariableArity = false;

        public override string DebugOut()
        {
            var modifier = Modifier == null ? string.Empty : Modifier.Data;
            var variableArity = !IsVariableArity ? string.Empty : "...";
            
            return string.Format("{0} {1} {3}{2}", modifier, Type.Data, Name.Data, variableArity);
        }
    }
}
