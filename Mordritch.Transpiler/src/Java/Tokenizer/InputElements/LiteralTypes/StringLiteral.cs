using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes
{
    public class StringLiteral : LiteralToken, ILiteralToken
    {
        public override LiteralTypeEnum LiteralType
        {
            get { return LiteralTypeEnum.StringLiteral; }
        }
    }
}
