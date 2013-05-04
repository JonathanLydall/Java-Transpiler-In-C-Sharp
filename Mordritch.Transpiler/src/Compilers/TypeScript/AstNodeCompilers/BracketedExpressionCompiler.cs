using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class BracketedExpressionCompiler
    {
        private ICompiler _compiler;

        private BracketedExpression _bracketedExpression;

        public BracketedExpressionCompiler(ICompiler compiler, BracketedExpression bracketedExpression)
        {
            _compiler = compiler;
            _bracketedExpression = bracketedExpression;
        }

        public string GetBracketedExpressionString()
        {
            var innerExpressions =
                _bracketedExpression.InnerExpressions
                    .Select(x => _compiler.GetExpressionString(x))
                    .Aggregate((x, y) => x + " " + y); // TODO: The formatting probably won't look ideal, there may land up being ugly looking spaces

            return string.Format("({0})", innerExpressions);
        }
    }
}
