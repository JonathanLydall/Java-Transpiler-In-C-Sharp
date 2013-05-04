using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class LabelStatementParser : Parser, IParser
    {
        private LabelStatement _labelStatement = new LabelStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            _labelStatement.Name = PreviousNonWhiteSpaceInputElement;
            MoveToNextInputElement();
            return _labelStatement;
        }

        public static bool IsLabel(IInputElement currentInputElement, int bufferSize, InputElementDataSource dataSource)
        {
            if (currentInputElement.Data != ":")
            {
                return false;
            }
            
            var buffer = new List<IInputElement>();
            for (var i = 0; i < bufferSize; i++)
            {
                if (dataSource.ForwardInputElement(i * -1).Data == "?")
                {
                    return false;
                }
            }
            
            return true;
        }
    }
}
