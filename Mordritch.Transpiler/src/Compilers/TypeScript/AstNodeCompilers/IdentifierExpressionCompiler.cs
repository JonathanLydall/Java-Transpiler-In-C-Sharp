using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
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

        private IAstNode _previousExpression;

        public IdentifierExpressionCompiler(ICompiler compiler, IdentifierExpression identifierExpression, IAstNode previousExpression)
        {
            _compiler = compiler;
            _identifierExpression = identifierExpression;
            _previousExpression = previousExpression;
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


            var scope = _compiler.GetScopeClarifier(_identifierExpression.Token.Data, _previousExpression);
            return scope + _identifierExpression.Token.Data;
        }
    }
}
