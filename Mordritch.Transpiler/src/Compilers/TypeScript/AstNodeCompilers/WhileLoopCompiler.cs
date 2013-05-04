using Mordritch.Transpiler.Java.AstGenerator.ControlStructures;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Loops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class WhileLoopCompiler
    {
        private ICompiler _compiler;

        private WhileLoop _whileLoop;

        public WhileLoopCompiler(ICompiler compiler, WhileLoop whileLoop)
        {
            _compiler = compiler;
            _whileLoop = whileLoop;
        }

        public void Compile()
        {
            _compiler.AddBlankLine();

            _compiler.AddLine(string.Format("while ({0}) {{", _compiler.GetInnerExpressionString(_whileLoop.Condition)));
            
            _compiler.IncreaseIndentation();
            {
                _compiler.CompileBody(_whileLoop.Body);
            }
            _compiler.DecreaseIndentation();

            _compiler.AddLine("}");

            _compiler.AddBlankLine();
        }
    }
}
