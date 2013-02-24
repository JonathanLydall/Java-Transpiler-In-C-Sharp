using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes
{
    public interface IToken : IInputElement
    {
        TokenTypeEnum TokenType { get; }

        new string Data { get; set; }
    }
}
