using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class ReturnStatementCompiler
    {
        private ICompiler _compiler;

        private ReturnStatement _returnStatement;

        public ReturnStatementCompiler(ICompiler compiler, ReturnStatement returnStatement)
        {
            _compiler = compiler;
            _returnStatement = returnStatement;
        }

        public void Compile()
        {
            var returnExpressions = _returnStatement.ReturnValue
                .Select(x => _compiler.GetExpressionString(x))
                .Aggregate((x, y) => x + " " + y);

            _compiler.AddLine(string.Format("return {0};", returnExpressions));
        }
    }
}
