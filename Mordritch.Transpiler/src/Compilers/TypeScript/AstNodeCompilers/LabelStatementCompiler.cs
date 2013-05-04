using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class LabelStatementCompiler
    {
        private ICompiler _compiler;

        private LabelStatement _labelStatement;

        public LabelStatementCompiler(ICompiler compiler, LabelStatement labelStatement)
        {
            _compiler = compiler;
            _labelStatement = labelStatement;
        }

        public void Compile()
        {
            _compiler.AddLine(string.Format("{0}:", _labelStatement.Name.Data));
        }
    }
}
