using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class TryStatementCompiler
    {
        private ICompiler _compiler;

        private TryStatement _tryStatement;

        public TryStatementCompiler(ICompiler compiler, TryStatement tryStatement)
        {
            _compiler = compiler;
            _tryStatement = tryStatement;
        }

        public void Compile()
        {
            _compiler.AddBlankLine();

            _compiler.AddLine("try {");
            _compiler.IncreaseIndentation();
            {
                _compiler.CompileBody(_tryStatement.TryBody);
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");

            if (_tryStatement.CatchStatements.Count > 1)
            {
                var description = "Multiple catch statements found for a try/catch statement which is not supported by TypeScript, catch statements will need to be merged manually.";
                
                _compiler.AddWarning(
                    _tryStatement.CatchStatements.First().ExceptionName.Line,
                    _tryStatement.CatchStatements.First().ExceptionName.Column,
                    description);
            }
            
            foreach (var catchStatement in _tryStatement.CatchStatements)
            {
                _compiler.AddLine(string.Format("catch ({0}) {{", catchStatement.ExceptionName.Data));
                _compiler.IncreaseIndentation();
                {
                    _compiler.CompileBody(catchStatement.Body);
                }
                _compiler.DecreaseIndentation();
                _compiler.AddLine("}");
            }

            if (_tryStatement.FinallyBody != null && _tryStatement.FinallyBody.Count > 0)
            {
                _compiler.AddLine("finally {");
                _compiler.IncreaseIndentation();
                {
                    _compiler.CompileBody(_tryStatement.FinallyBody);
                }
                _compiler.DecreaseIndentation();
                _compiler.AddLine("}");
            }

            _compiler.AddBlankLine();
        }
    }
}
