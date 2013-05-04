using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class IfElseStatementCompiler
    {
        private ICompiler _compiler;

        private IfElseStatement _ifElseStatement;

        public IfElseStatementCompiler(ICompiler compiler, IfElseStatement ifElseStatement)
        {
            _compiler = compiler;
            _ifElseStatement = ifElseStatement;
        }

        public void Compile()
        {
            _compiler.AddBlankLine();

            _compiler.AddLine(string.Format("if ({0}) {{", _compiler.GetInnerExpressionString(_ifElseStatement.Condition)));
            _compiler.IncreaseIndentation();
            {
                _compiler.CompileBody(_ifElseStatement.Body);
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");

            foreach (var conditionalElse in _ifElseStatement.ConditionalElses)
            {
                _compiler.AddLine(string.Format("else if ({0}) {{", _compiler.GetInnerExpressionString(conditionalElse.Condition)));
                _compiler.IncreaseIndentation();
                {
                    _compiler.CompileBody(conditionalElse.Body);
                }
                _compiler.DecreaseIndentation();
                _compiler.AddLine("}");
            }

            if (_ifElseStatement.ElseBody != null)
            {
                _compiler.AddLine("else {");
                _compiler.IncreaseIndentation();
                {
                    _compiler.CompileBody(_ifElseStatement.ElseBody);
                }
                _compiler.DecreaseIndentation();
                _compiler.AddLine("}");
            }

            _compiler.AddBlankLine();
        }
    }
}
