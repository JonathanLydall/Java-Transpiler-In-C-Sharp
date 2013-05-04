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
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Loops;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Statements;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class TryStatementParser : Parser, IParser
    {
        private TryStatement _tryStatement = new TryStatement();

        public override IAstNode ImplementationSpecificParse()
        {
            // See: http://docs.oracle.com/javase/specs/jls/se7/html/jls-14.html#jls-14.20
            
            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.Try);
            MoveToNextToken();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "{");
            _tryStatement.TryBody = ParseBody();

            _tryStatement.CatchStatements = GetCatchStatements();
            _tryStatement.FinallyBody = GetFinallyBody();
            
            return _tryStatement;
        }

        private IList<IAstNode> GetFinallyBody()
        {
            if (!Eof && CurrentInputElement.Data == Keywords.Finally)
            {
                MoveToNextToken();
                Debug.Assert(CurrentInputElement is SeperatorToken);
                Debug.Assert(CurrentInputElement.Data == "{");
                var returnValue = ParseBody();
                return returnValue.Count > 0 ? returnValue : null;
            }

            return null;
        }

        private IList<CatchStatement> GetCatchStatements()
        {
            // TODO: Implement modifiers, EG: catch (final Exception ex) {...

            var returnData = new List<CatchStatement>();
            
            while (!Eof && CurrentInputElement.Data == Keywords.Catch)
            {
                Debug.Assert(CurrentInputElement.Data == Keywords.Catch);
                MoveToNextToken();

                Debug.Assert(CurrentInputElement.Data == "(");
                MoveToNextToken();

                var catchStatement = new CatchStatement();
                while (CurrentInputElement.Data != ")")
                {
                    if (NextNonWhiteSpaceInputElement.Data == ")")
                    {
                        catchStatement.ExceptionName = CurrentInputElement;
                        MoveToNextToken();
                        continue;
                    }

                    if (CurrentInputElement.Data == "|")
                    {
                        MoveToNextToken();
                        continue;
                    }

                    catchStatement.ExceptionTypes.Add(CurrentInputElement);

                    MoveToNextToken();
                    continue;
                }
                Debug.Assert(CurrentInputElement is SeperatorToken);
                Debug.Assert(CurrentInputElement.Data == ")");
                MoveToNextToken();

                Debug.Assert(CurrentInputElement is SeperatorToken);
                Debug.Assert(CurrentInputElement.Data == "{");
                catchStatement.Body = ParseBody();

                returnData.Add(catchStatement);
            }

            return returnData;
        }
    }
}
