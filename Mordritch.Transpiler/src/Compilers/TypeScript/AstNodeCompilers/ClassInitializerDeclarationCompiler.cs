using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class ClassInitializerDeclarationCompiler
    {
        private ICompiler _compiler;

        private ClassInitializerDeclaration _classInitializerDeclaration;

        public ClassInitializerDeclarationCompiler(ICompiler compiler, ClassInitializerDeclaration classInitializerDeclaration)
        {
            _compiler = compiler;
            _classInitializerDeclaration = classInitializerDeclaration;
        }

        public void Compile()
        {
            // TODO: I don't think we even parse initializers properly
            throw new NotImplementedException();
        }
    }
}
