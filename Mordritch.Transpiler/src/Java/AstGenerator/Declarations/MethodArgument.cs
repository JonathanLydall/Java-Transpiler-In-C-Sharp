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

        public int ArrayDepth = 0;

        public override string DebugOut()
        {
            var modifier = Modifier == null ? string.Empty : Modifier.Data;
            var variableArity = !IsVariableArity ? string.Empty : "...";
            var name = Name.Data;
            var type = Type.Data;
            string arrayDepth = string.Empty;
            for (var i = 0; i < ArrayDepth; i++)
            {
                arrayDepth += "[]";
            }

            return string.Format("{0} {1}{2} {3}{4}", modifier, type, arrayDepth, variableArity, name);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            AddUsedTypeIfIdentifierToken(Type, returnList);

            return returnList;
        }
    }
}
