using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using Mordritch.Transpiler.src.Compilers;
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
        private IList<InnerExpressionProcessingListItem> _list;

        public IdentifierExpressionCompiler(ICompiler compiler, IdentifierExpression identifierExpression, IList<InnerExpressionProcessingListItem> list)
        {
            _list = list;
            _compiler = compiler;
            _identifierExpression = identifierExpression;
        }

        public string GetIdentifierExpressionString()
        {
            if (_identifierExpression.Token.Data == Keywords.Instanceof && KnownInterfaces.IsKnown(NextItemAsIdentifier().Token.Data))
            {
                NextItem().Processed = true;
                return string.Format(".instanceOf(\"{0}\")", NextItemAsIdentifier().Token.Data);
            }

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

            var scope = _compiler.GetScopeClarifier(_identifierExpression.Token.Data, PreviousItemAsExpression());
            return scope + _identifierExpression.Token.Data;
        }

        private IdentifierExpression NextItemAsIdentifier()
        {
            return NextItem().AstNode as IdentifierExpression;
        }

        private InnerExpressionProcessingListItem NextItem()
        {
            var itemIndex = _list == null ? 0 : _list.IndexOf(_list.First(x => x.AstNode == _identifierExpression));

            return itemIndex > 0 ? _list[itemIndex + 1] : null;
        }

        private IAstNode PreviousItemAsExpression()
        {
            var itemIndex = _list == null ? 0 : _list.IndexOf(_list.First(x => x.AstNode == _identifierExpression));
            
            return itemIndex > 0 ? _list[itemIndex - 1].AstNode : null;
        }
    }
}
