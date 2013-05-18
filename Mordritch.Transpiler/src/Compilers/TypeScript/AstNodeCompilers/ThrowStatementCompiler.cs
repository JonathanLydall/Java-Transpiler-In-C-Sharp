using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class ThrowStatementCompiler
    {
        private ICompiler _compiler;

        private ThrowStatement _throwStatement;

        public ThrowStatementCompiler(ICompiler compiler, ThrowStatement throwStatement)
        {
            _compiler = compiler;
            _throwStatement = throwStatement;
        }

        public void Compile()
        {
            _compiler.AddWarning(
                _throwStatement.ExceptionInstance.First().Line,
                _throwStatement.ExceptionInstance.First().Column,
                "Throw statement's not fully support by TypeScript transpiler, original is: " + _throwStatement.DebugOut());

            _compiler.AddLine("throw new Error(\"See above comment for more details on this exception.\");");
        }
    }
}
