using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class AssertStatementParser : Parser, IParser
    {
        private AssertStatement _assertStatement = new AssertStatement();
        
        public override IAstNode ImplementationSpecificParse()
        {
            AssertKeyword(Keywords.Assert);
            MoveToNextInputElement();

            ProcessCondition();
            
            if (CurrentInputElement.Data == ":")
            {
                ProcessMessage();
            }
            
            AssertSeperator(";");
            MoveToNextInputElement();

            return _assertStatement;
        }

        private void ProcessMessage()
        {
            _assertStatement.Message = new List<IInputElement>();

            AssertTokenData(":");
            MoveToNextToken();

            while (CurrentInputElement.Data != ";")
            {
                _assertStatement.Message.Add(CurrentInputElement);
                MoveToNextInputElement();
            }
        }

        private void ProcessCondition()
        {
            while (CurrentInputElement.Data != ":" && CurrentInputElement.Data != ";")
            {
                _assertStatement.Condition.Add(CurrentInputElement);
                MoveToNextInputElement();
            }

            while (_assertStatement.Condition[0].InputElementType == InputElementTypeEnum.WhiteSpace)
            {
                _assertStatement.Condition.RemoveAt(0);
            }

            while (_assertStatement.Condition[_assertStatement.Condition.Count - 1].InputElementType == InputElementTypeEnum.WhiteSpace)
            {
                _assertStatement.Condition.RemoveAt(_assertStatement.Condition.Count - 1);
            }
        }
    }
}
