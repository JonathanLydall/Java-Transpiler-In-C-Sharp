using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public interface IParser
    {
        IAstNode Parse(InputElementDataSource inputElementDataSource);

        bool IsWhiteSpace { get; }

        void MoveToNextInputElement();
    }
}
