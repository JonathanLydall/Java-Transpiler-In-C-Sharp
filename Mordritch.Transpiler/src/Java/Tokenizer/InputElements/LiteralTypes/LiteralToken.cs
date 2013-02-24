using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes
{
    public abstract class LiteralToken : TokenInputElement, IToken, ILiteralToken
    {
        public override TokenTypeEnum TokenType
        {
            get { return TokenTypeEnum.Literal; }
        }

        abstract public LiteralTypeEnum LiteralType { get; }

        public new string GetInputElementType()
        {
            return base.GetInputElementType() + " -> " + this.LiteralType.ToString();
        }
    }
}
