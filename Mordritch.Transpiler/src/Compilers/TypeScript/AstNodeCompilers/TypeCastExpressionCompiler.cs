using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class TypeCastExpressionCompiler
    {
        private ICompiler _compiler;

        private TypeCastExpression _typeCastExpression;

        public TypeCastExpressionCompiler(ICompiler compiler, TypeCastExpression typeCastExpression)
        {
            _compiler = compiler;
            _typeCastExpression = typeCastExpression;
        }

        public string GetTypeCastExpressionString()
        {
            var innerExpressions =
                _typeCastExpression.InnerExpressions
                    .Select(x => _compiler.GetExpressionString(x))
                    .Aggregate((x, y) => x + y);

            var castTarget = _compiler.GetTypeString(_typeCastExpression.CastTarget, "GetTypeCastExpressionString");

            return string.Format("<{0}>{1}", castTarget, innerExpressions);
        }
    }
}
