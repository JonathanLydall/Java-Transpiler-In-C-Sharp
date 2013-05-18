using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class ClassInstantiationExpressionCompiler
    {
        private ICompiler _compiler;

        private ClassInstantiationExpression _classInstantiationExpression;

        public ClassInstantiationExpressionCompiler(ICompiler compiler, ClassInstantiationExpression classInstantiationExpression)
        {
            _compiler = compiler;
            _classInstantiationExpression = classInstantiationExpression;
        }

        public string GetClassInstantiationExpressionString()
        {
            var className = _compiler.GetTypeString(_classInstantiationExpression.ClassName, "GetClassInstantiationExpressionString");

            var initializationData = _classInstantiationExpression.InitializationData.Count == 0
                ? string.Empty
                : _classInstantiationExpression.InitializationData
                    .Select(x => GetParameterString(x))
                    .Aggregate((x, y) => x + ", " + y);

            var initializedArray = _classInstantiationExpression.ArrayContents == null
                ? null
                : _compiler.GetArrayInitializationExpressionString(_classInstantiationExpression.ArrayContents);

            var isArray = _classInstantiationExpression.ArraySizes.Count > 0;
            var isArrayAndInitialized = _classInstantiationExpression.ArrayContents != null;

            if (!isArray)
            {
                return string.Format("new {0}({1})", className, initializationData);
            }

            if (isArray && !isArrayAndInitialized)
            {
                return string.Format("[]", className, initializationData); // TODO: I can't work out the syntax for initializing the array size in TypeScript (assuming it's possible)
            }

            if (isArray && isArrayAndInitialized)
            {
                return initializedArray;
            }

            throw new Exception("Unexpected combination.");
        }

        private string GetParameterString(IList<IAstNode> parameter)
        {
            var returnString = new StringBuilder();

            IAstNode previousExpresion = null;

            foreach (var expression in parameter)
            {
                returnString.Append(_compiler.GetExpressionString(expression, previousExpresion));
                previousExpresion = expression;
            }

            return returnString.ToString();
        }
    }
}
