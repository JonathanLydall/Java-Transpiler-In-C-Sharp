using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures;
using System.Diagnostics;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class SwitchStatementParser : Parser, IParser
    {
        private SwitchStatement _switchStatement = new SwitchStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.Switch);
            MoveToNextToken();

            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextToken();
            _switchStatement.ControlStatement = GetInnerExpression(")");

            _switchStatement.Body = GetBody();

            return _switchStatement;
        }

        public override IAstNode ImplementationSpecificParseSingleStatement()
        {
            if (CurrentInputElement is KeywordToken && CurrentInputElement.Data == Keywords.Case)
            {
                return GetCaseStatement();
            }

            if (CurrentInputElement is KeywordToken && CurrentInputElement.Data == Keywords.Default)
            {
                return GetSwitchDefaultStatement();
            }

            return null;
        }

        private IAstNode GetSwitchDefaultStatement()
        {
            ParserHelper.PreMessage<SwitchStatement>(DataSource);

            var switchDefaultStatement = new SwitchDefaultStatement();

            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.Default);
            MoveToNextToken();

            Debug.Assert(CurrentInputElement is OperatorToken);
            Debug.Assert(CurrentInputElement.Data == ":");
            MoveToNextToken();

            ParserHelper.PostMessage<SwitchStatement>(switchDefaultStatement);

            return switchDefaultStatement;
        }

        private IAstNode GetCaseStatement()
        {
            ParserHelper.PreMessage<SwitchStatement>(DataSource);

            var caseStatement = new CaseStatement();
            var caseValue = caseStatement.CaseValue;
            
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.Case);
            MoveToNextToken();

            while (CurrentInputElement.Data != ":")
            {
                caseValue.Add(CurrentInputElement);
                MoveToNextToken();
            }

            Debug.Assert(CurrentInputElement is OperatorToken);
            Debug.Assert(CurrentInputElement.Data == ":");
            MoveToNextToken();

            ParserHelper.PostMessage<SwitchStatement>(caseStatement);

            return caseStatement;
        }

        private IList<IAstNode> GetBody()
        {
            MoveToNextToken();
            if (CurrentInputElement is SeperatorToken && CurrentInputElement.Data == "{")
            {
                return ParseBody();
            }
            else
            {
                var body = new List<IAstNode>();
                body.Add(ParseSingleStatement());
                return body;
            }
        }
    }
}
