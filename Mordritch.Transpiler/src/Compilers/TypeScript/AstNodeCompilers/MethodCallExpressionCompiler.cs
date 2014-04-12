using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using Mordritch.Transpiler.Java.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class MethodCallExpressionCompiler
    {
        private ICompiler _compiler;

        private MethodCallExpression _methodCallExpression;

        private IAstNode _previousExpression;

        public MethodCallExpressionCompiler(ICompiler compiler, MethodCallExpression methodCallExpression, IAstNode previousExpression)
        {
            _compiler = compiler;
            _methodCallExpression = methodCallExpression;
            _previousExpression = previousExpression;
        }

        public string GetMethodCallExpressionString()
        {
            var parameters = _methodCallExpression.Parameters == null || _methodCallExpression.Parameters.Count == 0
                ? string.Empty
                : _methodCallExpression.Parameters
                    .Select(x => GetParameterString(x))
                    .Aggregate((x, y) => x + ", " + y);

            var methodIdentifier = _methodCallExpression.MethodIdentifier.Data;

            if (methodIdentifier == Keywords.This)
            {
                methodIdentifier = string.Format("{0}.{1}", Keywords.This, ConstructorCompiler.CONSTRUCTOR_DISPATCHER_FUNCTION_NAME);
            }

            if (methodIdentifier == Keywords.Super)
            {
                methodIdentifier = string.Format("{0}.{1}", Keywords.Super, ConstructorCompiler.CONSTRUCTOR_DISPATCHER_FUNCTION_NAME);
            }

            var scopeClarifier = _compiler.GetScopeClarifier(methodIdentifier, _previousExpression);

            return string.Format("{0}{1}({2})", scopeClarifier, methodIdentifier, parameters);
        }

        private string GetParameterString(IList<IAstNode> parameter)
        {
            var returnString = new StringBuilder();

            IAstNode previousExpression = null;
            foreach (var expression in parameter)
            {
                returnString.Append(_compiler.GetExpressionString(expression, previousExpression));
                previousExpression = expression;
            }

            return returnString.ToString();
        }

    }
}
