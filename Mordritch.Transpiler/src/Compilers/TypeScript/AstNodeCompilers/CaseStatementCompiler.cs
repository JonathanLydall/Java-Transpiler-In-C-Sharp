using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class CaseStatementCompiler
    {
        private ICompiler _compiler;

        private CaseStatement _caseStatement;

        public CaseStatementCompiler(ICompiler compiler, CaseStatement caseStatement)
        {
            _compiler = compiler;
            _caseStatement = caseStatement;
        }

        public void Compile()
        {
            var caseValue =
                _caseStatement.CaseValue
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + y);

            _compiler.AddBlankLine();
            _compiler.AddLine(string.Format("case {0}:", caseValue));
            _compiler.AddBlankLine();
        }
    }
}
