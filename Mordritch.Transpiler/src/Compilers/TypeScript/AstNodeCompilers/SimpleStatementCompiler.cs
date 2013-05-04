using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class SimpleStatementCompiler
    {
        private ICompiler _compiler;

        private SimpleStatement _simpleStatement;

        public SimpleStatementCompiler(ICompiler compiler, SimpleStatement simpleStatement)
        {
            _compiler = compiler;
            _simpleStatement = simpleStatement;
        }

        public void Compile()
        {
            var expressions = _simpleStatement.Expressions
                .Select(x => _compiler.GetExpressionString(x))
                .Aggregate((x, y) => x + " " + y);

            _compiler.AddLine(string.Format("{0};", expressions));
        }
    }
}
