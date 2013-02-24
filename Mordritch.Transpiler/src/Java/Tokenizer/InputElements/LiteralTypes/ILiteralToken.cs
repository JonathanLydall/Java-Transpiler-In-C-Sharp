using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes
{
    public interface ILiteralToken : IToken
    {
        LiteralTypeEnum LiteralType { get; }
    }
}
