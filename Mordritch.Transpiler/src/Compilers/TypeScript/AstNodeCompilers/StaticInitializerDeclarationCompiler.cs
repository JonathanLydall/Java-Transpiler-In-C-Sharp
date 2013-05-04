using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class StaticInitializerDeclarationCompiler
    {
        private ICompiler _compiler;

        private StaticInitializerDeclaration _staticInitializerDeclaration;

        public StaticInitializerDeclarationCompiler(ICompiler compiler, StaticInitializerDeclaration staticInitializerDeclaration)
        {
            _compiler = compiler;
            _staticInitializerDeclaration = staticInitializerDeclaration;
        }

        public void Compile()
        {
            // TODO: Static initializers aren't yet (2012-04-29) supported by TypeScript, so the below pattern is a hack, however, the feature has been requested: https://typescript.codeplex.com/workitem/862
            // This hack below will work, however, it needs to have implemented that this is the last part compiled of the class or it may not work right.

            _compiler.AddBlankLine();
            _compiler.AddWarning(
                _staticInitializerDeclaration.StaticKeywordToken.Line,
                _staticInitializerDeclaration.StaticKeywordToken.Column,
                "TypeScript does not yet (2012-04-29) support static class initializers, so this is a hack and should be at the end of the line. See https://typescript.codeplex.com/workitem/862 for more information.");

            _compiler.AddLine("static staticConstructor = (() => {");
            _compiler.IncreaseIndentation();
            {
                _compiler.CompileBody(_staticInitializerDeclaration.Body);
                _compiler.AddLine("return null;");
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("})();");
            _compiler.AddBlankLine();
        }
    }
}
