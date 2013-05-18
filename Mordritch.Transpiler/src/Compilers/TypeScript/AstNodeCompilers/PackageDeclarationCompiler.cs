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
            var content = _packageDeclaration.Content == null || _packageDeclaration.Content.Count == 0
                ? string.Empty
                : _packageDeclaration.Content
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + y);

            _compiler.AddWarning(
                _packageDeclaration.Content.First().Line,
                _packageDeclaration.Content.First().Column,
                string.Format("Package declaration detected, not handled in TypeScript compiler, may need to be addressed manually. package {0} ;", content));
        }
    }
}
