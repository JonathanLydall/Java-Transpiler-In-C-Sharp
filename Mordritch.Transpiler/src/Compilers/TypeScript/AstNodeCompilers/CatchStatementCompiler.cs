using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class CatchStatementCompiler
    {
        private ICompiler _compiler;

        private CatchStatement _catchStatement;

        public CatchStatementCompiler(ICompiler compiler, CatchStatement catchStatement)
        {
            _compiler = compiler;
            _catchStatement = catchStatement;
        }

        public void Compile()
        {
            _compiler.AddLine(string.Format("catch ({0}) {{", _catchStatement.ExceptionName));
            _compiler.IncreaseIndentation();
            {
                _compiler.CompileBody(_catchStatement.Body);
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }
    }
}
