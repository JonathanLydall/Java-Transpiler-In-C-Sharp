using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes
{
    public interface IInputElement
    {
        InputElementTypeEnum InputElementType { get; }

        int Line { get; set; }

        int Column { get; set; }

        string Source { get; set; }

        string Position { get; }

        string GetInputElementType();

        string Data { get; }
    }
}
