using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class SwitchDefaultStatementCompiler
    {
        private ICompiler _compiler;

        private SwitchDefaultStatement _switchDefaultStatement;

        public SwitchDefaultStatementCompiler(ICompiler compiler, SwitchDefaultStatement switchDefaultStatement)
        {
            _compiler = compiler;
            _switchDefaultStatement = switchDefaultStatement;
        }

        public void Compile()
        {
            _compiler.AddLine("default:");
        }
    }
}
