using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class ReturnStatement : AstNode
    {
        public IList<IInputElement> ReturnValue = new List<IInputElement>();

        public override string DebugOut()
        {
            var returnValue =
                ReturnValue
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + y);

            return string.Format("return {0};", returnValue);
        }
    }
}
