using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes
{
    public abstract class InputElement : IInputElement
    {
        public int Line { get; set; }

        public int Column { get; set; }

        public string Source { get; set; }

        public abstract InputElementTypeEnum InputElementType { get; }

        public string Data
        {
            get { return string.Empty; }
        }

        public string Position
        {
            get { return string.Format("({0}, {1} - {2})", Line + 1, Column + 1, Source); }
        }

        public string GetInputElementType()
        {
            return this.InputElementType.ToString();
        }
    }
}
