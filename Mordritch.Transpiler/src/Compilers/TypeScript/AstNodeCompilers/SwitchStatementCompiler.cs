using Mordritch.Transpiler.Java.AstGenerator.ControlStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class SwitchStatementCompiler
    {
        private ICompiler _compiler;

        private SwitchStatement _switchStatement;

        public SwitchStatementCompiler(ICompiler compiler, SwitchStatement switchStatement)
        {
            _compiler = compiler;
            _switchStatement = switchStatement;
        }

        public void Compile()
        {
            var controlStatement = _compiler.GetInnerExpressionString(_switchStatement.ControlStatement);

            _compiler.AddBlankLine();
            _compiler.AddLine(string.Format("switch ({0}) {{", controlStatement));
            _compiler.IncreaseIndentation();
            {
                // TODO: This seems less than ideal, the case statements aren't seperated and are just all in the body, at some point change the parser, then update this compiler.
                _compiler.CompileBody(_switchStatement.Body);
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }
    }
}
