using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class MethodCallExpressionCompiler
    {
        private ICompiler _compiler;

        private MethodCallExpression _methodCallExpression;

        public MethodCallExpressionCompiler(ICompiler compiler, MethodCallExpression methodCallExpression)
        {
            _compiler = compiler;
            _methodCallExpression = methodCallExpression;
        }

        public string GetMethodCallExpressionString()
        {
            var parameters = _methodCallExpression.Parameters == null || _methodCallExpression.Parameters.Count == 0
                ? string.Empty
                : _methodCallExpression.Parameters
                    .Select(x => GetParameterString(x))
                    .Aggregate((x, y) => x + ", " + y);

            var methodIdentifier = _methodCallExpression.MethodIdentifier.Data;

            return string.Format("{0}({1})", methodIdentifier, parameters);
        }

        private string GetParameterString(IList<IAstNode> parameter)
        {
            var returnString = new StringBuilder();
            
            foreach (var expression in parameter)
            {
                returnString.Append(_compiler.GetExpressionString(expression));
            }

            return returnString.ToString();
        }

    }
}
