using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes
{
    public class IdentifierToken : TokenInputElement, IToken
    {
        public override TokenTypeEnum TokenType
        {
            get { return TokenTypeEnum.Identifier; }
        }
    }
}
