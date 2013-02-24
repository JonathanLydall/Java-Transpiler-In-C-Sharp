using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes
{
    public class WhiteSpaceInputElement : InputElement, IInputElement
    {
        public override InputElementTypeEnum InputElementType
        {
            get { return InputElementTypeEnum.WhiteSpace; }
        }
    }
}
