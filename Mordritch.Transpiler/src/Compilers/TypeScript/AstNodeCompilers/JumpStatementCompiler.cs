using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class JumpStatementCompiler
    {
        private ICompiler _compiler;

        private JumpStatement _jumpStatement;

        public JumpStatementCompiler(ICompiler compiler, JumpStatement jumpStatement)
        {
            _compiler = compiler;
            _jumpStatement = jumpStatement;
        }

        public void Compile()
        {
            var jumpStatement = _jumpStatement.Statement.Data;
            var jumpTo = _jumpStatement.JumpTo == null ? string.Empty : " " + _jumpStatement.JumpTo.Data;

            _compiler.AddLine(string.Format("{0}{1};", jumpStatement, jumpTo));
        }
    }
}
