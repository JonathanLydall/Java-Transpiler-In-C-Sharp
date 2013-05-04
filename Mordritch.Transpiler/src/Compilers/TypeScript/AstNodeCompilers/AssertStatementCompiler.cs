using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class AssertStatementCompiler
    {
        private ICompiler _compiler;

        private AssertStatement _assertStatement;

        public AssertStatementCompiler(ICompiler compiler, AssertStatement assertStatement)
        {
            _compiler = compiler;
            _assertStatement = assertStatement;
        }

        public void Compile()
        {
            _compiler.AddWarning(
                _assertStatement.Condition.First().Line,
                _assertStatement.Condition.First().Column,
                "Assert statement not compiled as unsupported by TypeScript");
        }
    }
}
