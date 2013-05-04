using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    class IdentifierExpressionCompiler
    {
        private ICompiler _compiler;

        private IdentifierExpression _identifierExpression;

        public IdentifierExpressionCompiler(ICompiler compiler, IdentifierExpression identifierExpression)
        {
            _compiler = compiler;
            _identifierExpression = identifierExpression;
        }

        public string GetIdentifierExpressionString()
        {
            if (_identifierExpression.Token.Data == Keywords.New)
            {
                return string.Format("{0} ", Keywords.New);
            }

            if (
                _identifierExpression.Token.Data == "+" ||
                _identifierExpression.Token.Data == "-" ||
                _identifierExpression.Token.Data == "*" ||
                _identifierExpression.Token.Data == "/")
            {
                return string.Format(" {0} ", _identifierExpression.Token.Data);
            }

            if (_identifierExpression.Token is FloatingPointLiteral)
            {
                var returnString = _identifierExpression.Token.Data;
                return returnString.Substring(0, returnString.Length - 1);
            }

            return _identifierExpression.Token.Data;
        }
    }
}
