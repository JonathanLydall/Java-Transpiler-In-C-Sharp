using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes
{
    public abstract class TokenInputElement : InputElement, IInputElement, IToken
    {
        public override InputElementTypeEnum InputElementType
        {
            get { return InputElementTypeEnum.Token; }
        }

        public abstract TokenTypeEnum TokenType { get; }

        public new string Data { get; set; }

        public new string GetInputElementType()
        {
            return base.GetInputElementType() + " -> " + this.TokenType.ToString();
        }
    }
}
