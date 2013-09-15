using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class PackageDeclarationCompiler
    {
        private ICompiler _compiler;

        private PackageDeclaration _packageDeclaration;

        public PackageDeclarationCompiler(ICompiler compiler, PackageDeclaration packageDeclaration)
        {
            _compiler = compiler;
            _packageDeclaration = packageDeclaration;
        }

        public void GenerateDefinition()
        {
            Compile();
        }

        public void Compile()
        {
            // Handled by TypeScriptCompiler's Compile and GenerateDefinition methods.
        }
    }
}
