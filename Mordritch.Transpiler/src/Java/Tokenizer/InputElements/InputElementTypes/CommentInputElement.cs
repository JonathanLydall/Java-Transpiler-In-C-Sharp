using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes
{
    public class CommentInputElement : InputElement, IInputElement
    {
        public override InputElementTypeEnum InputElementType
        {
            get { return InputElementTypeEnum.Comment; }
        }

        public bool IsMultilineComment { get; set; }

        public new string Data { get; set; }
    }
}
