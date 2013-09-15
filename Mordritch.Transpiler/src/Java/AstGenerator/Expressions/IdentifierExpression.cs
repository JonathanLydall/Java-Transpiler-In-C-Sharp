using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Expressions
{
    public class IdentifierExpression : AstNode
    {
        public IInputElement Token;
        
        public IdentifierExpression(IInputElement inputElement)
        {
            Token = inputElement;
        }

        public override string DebugOut()
        {
            if (Token.Data == Keywords.New)
            {
                return string.Format("{0} ", Keywords.New);
            }

            if (
                Token.Data == "+" ||
                Token.Data == "-" ||
                Token.Data == "*" ||
                Token.Data == "/")
            {
                return string.Format(" {0} ", Token.Data);
            }

            return Token.Data;
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            AddUsedTypeIfIdentifierToken(Token, returnList);

            return returnList;
        }
    }
}
