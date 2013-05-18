using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Loops;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class DoWhileLoopCompiler
    {
        private ICompiler _compiler;

        private DoWhileLoop _doWhileLoop;

        public DoWhileLoopCompiler(ICompiler compiler, DoWhileLoop doWhileLoop)
        {
            _compiler = compiler;
            _doWhileLoop = doWhileLoop;
        }

        public void Compile()
        {
            _compiler.AddBlankLine();
            _compiler.AddLine("do {");
            _compiler.IncreaseIndentation();
            {
                _compiler.CompileBody(_doWhileLoop.Body);
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine(string.Format("}} do ({0});", _compiler.GetInnerExpressionString(_doWhileLoop.Condition)));
            _compiler.AddBlankLine();
        }
    }
}
