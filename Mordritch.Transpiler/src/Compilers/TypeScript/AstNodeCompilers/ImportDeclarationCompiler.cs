using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class ImportDeclarationCompiler
    {
        private ICompiler _compiler;

        private ImportDeclaration _importDeclaration;

        public ImportDeclarationCompiler(ICompiler compiler, ImportDeclaration importDeclaration)
        {
            _compiler = compiler;
            _importDeclaration = importDeclaration;
        }

        public void Compile()
        {
            // TODO: TypeScript doesn't have exact imports, but we may want to do something like it perhaps.
        }
    }
}
