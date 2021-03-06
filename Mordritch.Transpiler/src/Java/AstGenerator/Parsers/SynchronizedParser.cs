﻿using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    class SynchronizedStatementParser : Parser, IParser
    {
        private SynchronizedStatement _synchronizedStatement = new SynchronizedStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            while (CurrentInputElement.Data == Keywords.Synchronized)
            {
                _synchronizedStatement.Modifiers.Add(CurrentInputElement);
                MoveToNextToken();
            }
            
            MoveToNextInputElement();

            ProcessLockObject();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "{");
            _synchronizedStatement.Body = ParseBody();            

            return _synchronizedStatement;
        }

        private void ProcessLockObject()
        {
            while (CurrentInputElement.Data != "{")
            {
                _synchronizedStatement.LockObject.Add(CurrentInputElement);
                MoveToNextInputElement();
            }

            while (_synchronizedStatement.LockObject[0].InputElementType == InputElementTypeEnum.WhiteSpace)
            {
                _synchronizedStatement.LockObject.RemoveAt(0);
            }

            while (_synchronizedStatement.LockObject[_synchronizedStatement.LockObject.Count - 1].InputElementType == InputElementTypeEnum.WhiteSpace)
            {
                _synchronizedStatement.LockObject.RemoveAt(_synchronizedStatement.LockObject.Count - 1);
            }
        }
    }
}
