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

        public void GenerateDefinition()
        {
            Compile();
        }

        public void Compile()
        {
            var className = _importDeclaration.Content.Split('.').Last();

            _compiler.AddLine(string.Format("import {0} = Mordritch.{1};", className, _importDeclaration.Content));

        }
    }
}
