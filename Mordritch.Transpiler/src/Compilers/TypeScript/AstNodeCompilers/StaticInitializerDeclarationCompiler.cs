using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.src;
using Mordritch.Transpiler.src.Compilers;
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

        public void GenerateDefinition()
        {
            // Nothing to do here, move along.
        }

        public void Compile()
        {
            var className = GetClassName();
            var methodDetails = JavaClassMetadata.GetClass(GetClassName()).GetMethod("static");
            var classInheritanceStack = _compiler.GetClassInheritanceStack(className);

            var methodComment = methodDetails.GetComment();
            var methodNeedsExclusion = methodDetails.NeedsExclusion(classInheritanceStack);
            var methodNeedsBodyOnlyExclusion = methodDetails.NeedsBodyOnlyExclusion();
            var methodNeedsExtending = methodDetails.NeedsExtending();

            // TODO: Static initializers aren't yet (2013-03-21) supported by TypeScript, so the below pattern is a hack, however,
            // the feature has been requested: https://typescript.codeplex.com/workitem/862
            // This hack below will work, however, if this is not called as the last part of the class, it may cause a runtime error.

            _compiler.AddBlankLine();
            _compiler.AddWarning(
                _staticInitializerDeclaration.StaticKeywordToken.Line,
                _staticInitializerDeclaration.StaticKeywordToken.Column,
                "TypeScript does not yet (2014-03-21) support static class initializers, so this is a hack and should be at the end " +
                "of the line. See https://typescript.codeplex.com/workitem/862 for more information.");

            if (methodNeedsExclusion)
            {
                _compiler.AddLine(string.Format("// Static initializer excluded: {0}", methodComment));
            }

            if (methodNeedsBodyOnlyExclusion)
            {
                _compiler.AddLine(string.Format("// Static initializer has body only excluded: {0}", methodComment));
            }

            if (methodNeedsExtending)
            {
                _compiler.AddLine(string.Format("// Static initializer needs extending: {0}", methodComment));
            }

            if (methodNeedsExclusion || methodNeedsExtending)
            {
                _compiler.BeginCommentingOut();
            }
            
            _compiler.AddLine("static staticConstructor = (() => {");
            
            _compiler.IncreaseIndentation();
            {
                if (methodNeedsBodyOnlyExclusion)
                {
                    _compiler.BeginCommentingOut();
                }

                _compiler.CompileBody(_staticInitializerDeclaration.Body);
                
                if (methodNeedsBodyOnlyExclusion)
                {
                    _compiler.EndCommentingOut();
                }
                
                _compiler.AddLine("return null;");
            }
            _compiler.DecreaseIndentation();

            _compiler.AddLine("})();");

            if (methodNeedsExclusion || methodNeedsExtending)
            {
                _compiler.EndCommentingOut();
            }

            _compiler.AddBlankLine();
        }

        private string GetClassName()
        {
            var parentContext = _compiler.GetPreviousContextFromStack(1);

            return parentContext != null && parentContext is ClassType
                ? (parentContext as ClassType).Name
                : null;
        }

    }
}
