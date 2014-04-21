using Mordritch.Transpiler.Contracts;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.src;
using Mordritch.Transpiler.src.Compilers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class MethodDeclarationCompiler
    {
        private TypeScriptCompiler _compiler;

        private MethodDeclaration _methodDeclaration;

        public MethodDeclarationCompiler(ICompiler compiler, MethodDeclaration methodDeclaration)
        {
            _compiler = (TypeScriptCompiler)compiler;
            _methodDeclaration = methodDeclaration;
        }

        public void GenerateDefinition()
        {
            var className = GetClassName(_compiler);
            var methodName = _methodDeclaration.Name.Data;
            var classInheritanceStack = _compiler.GetClassInheritanceStack(className);
            var skipCompile = className != null && GetMethodDetail(_compiler, _methodDeclaration).NeedsExclusion(classInheritanceStack);
            var potentiallyCommented = skipCompile ? "// " : string.Empty;

            if (skipCompile)
            {
                _compiler.BeginCommentingOut();
            }

            var methodArguments = GetArguments(_compiler, _methodDeclaration);
            var returnType = GetReturnType(_compiler, _methodDeclaration);
            var arrayDepth = GetArrayDepth(_methodDeclaration);


            if (IsConstructorMethod())
            {
                throw new InvalidOperationException("Should never be called on a constructor.");
            }

            _compiler.AddLine(string.Format("{0}{1}({2}): {3}{4};", potentiallyCommented, methodName, methodArguments, returnType, arrayDepth));

            if (skipCompile)
            {
                _compiler.EndCommentingOut();
            }
        }

        public void Compile(string methodNameSuffix = null)
        {
            var methodName = _methodDeclaration.Name.Data;
            var className = GetClassName(_compiler);
            var methodDetail = GetMethodDetail(_compiler, _methodDeclaration);
            var classInheritanceStack = _compiler.GetClassInheritanceStack(className);

            var skipCompile = SkipCompile(_compiler, _methodDeclaration);
            var skipBody = className != null && methodDetail.NeedsBodyOnlyExclusion();
            var needsExtension = methodDetail.NeedsExtending();
            var comment = skipCompile ? methodDetail.GetExclusionComment(classInheritanceStack) : methodDetail.GetComment(classInheritanceStack);

            var isAbstractMethod = false;

            _compiler.AddBlankLine();
            if (skipCompile)
            {
                _compiler.AddWarning(
                    _methodDeclaration.Name.Line,
                    _methodDeclaration.Name.Column,
                    string.Format("Excluded methodDeclaration {0}: {1}", methodName, comment));

                _compiler.BeginCommentingOut();
            }

            if (needsExtension)
            {
                _compiler.AddLine("// Method below needs to be implemented manually in extended class.");
                if (_methodDeclaration.Modifiers.Any(x => x.Data == Keywords.Private))
                {
                    _compiler.AddLine("// Was private, changed to be public.");
                }
                _compiler.AddLine(string.Format("// {0}", comment));
            }

            var dependantMethods = methodDetail.GetDependantMethods();
            if (dependantMethods.Any())
            {
                _compiler.AddLine(string.Format("// Forced public due to dependancy by: {0}", dependantMethods.Aggregate((x, y) => x + ", " + y)));
            }

            if (_methodDeclaration.ThrowsType != null)
            {
                var description = string.Format("Detected a throws type on method declaration, this is not being included in the TypeScript output and may need to be handled manually. Throws Type: {0}",
                    _methodDeclaration.ThrowsType.Data);

                _compiler.AddWarning(
                    _methodDeclaration.ThrowsType.Line,
                    _methodDeclaration.ThrowsType.Column,
                    description);
            }

            if (skipBody)
            {
                _compiler.AddWarning(_methodDeclaration.Name.Line, _methodDeclaration.Name.Column, string.Format("Excluded body for method '{0}': {1}", methodName, comment));
            }

            if (_methodDeclaration.Body == null)
            {
                Debug.Assert(_methodDeclaration.Modifiers.Any(x => x.Data == Keywords.Abstract), "Unexpected null body, without abstract keyword.");
                isAbstractMethod = true;
            }

            _compiler.AddLine(string.Format("{0} {{", GetSignature(_compiler, _methodDeclaration, methodNameSuffix)));

            _compiler.IncreaseIndentation();
            {
                if (skipBody)
                {
                    _compiler.BeginCommentingOut();
                    _compiler.CompileBody(_methodDeclaration.Body);
                    _compiler.EndCommentingOut();
                }
                else if (isAbstractMethod)
                {
                    _compiler.AddLine(@"throw new Error(""This was an abstract method in the Java source code and should not be called unless overridden in a derived class first."");");
                }
                else if (needsExtension)
                {
                    _compiler.AddLine("throw new Error(\"This method needs manual implementation in the extension of this class.\");");
                    _compiler.BeginCommentingOut();
                    _compiler.CompileBody(_methodDeclaration.Body);
                    _compiler.EndCommentingOut();
                }
                else
                {
                    _compiler.CompileBody(_methodDeclaration.Body);
                }
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            if (skipCompile)
            {
                _compiler.EndCommentingOut();
            }
            _compiler.AddBlankLine();
        }

        public static bool SkipCompile(ICompiler compiler, MethodDeclaration methodDeclaration)
        {
            var classInheritanceStack = compiler.GetClassInheritanceStack(GetClassName(compiler));
            return GetMethodDetail(compiler, methodDeclaration).NeedsExclusion(classInheritanceStack);
        }

        public static string GetSignature(TypeScriptCompiler compiler, MethodDeclaration methodDeclaration, string methodNameSuffix = "")
        {
            var modifiers = GetModifiers(compiler, methodDeclaration);
            var methodName = methodDeclaration.Name.Data;
            var methodArguments = GetArguments(compiler, methodDeclaration);
            var returnType = GetReturnType(compiler, methodDeclaration);
            var arrayDepth = GetArrayDepth(methodDeclaration);

            return string.Format("{0}{1}{2}({3}): {4}{5}", modifiers, methodName, methodNameSuffix, methodArguments, returnType, arrayDepth);
        }

        private bool IsConstructorMethod()
        {
            var parentContext = _compiler.GetPreviousContextFromStack(1);
            
            return
                parentContext != null &&
                parentContext is ClassType &&
                (parentContext as ClassType).Name == _methodDeclaration.Name.Data;
        }

        private static string GetClassName(ICompiler compiler)
        {
            var classType = compiler.GetPreviousContextFromStack<ClassType>();

            return classType == null ? null : classType.Name;
            
        }

        private static string GetArrayDepth(MethodDeclaration methodDeclaration)
        {
            var arrayString = string.Empty;

            for (var i = 0; i < methodDeclaration.ReturnArrayDepth; i++)
            {
                arrayString += "[]";
            }

            return arrayString;
        }

        private static string GetModifiers(TypeScriptCompiler compiler, MethodDeclaration methodDeclaration)
        {
            var modifiers = new List<string>();
            var methodDetail = GetMethodDetail(compiler, methodDeclaration);

            modifiers.Add(
                methodDetail.NeedsExtending() ||
                methodDetail.GetDependantMethods().Any() ||
                (methodDeclaration.Modifiers != null && methodDeclaration.Modifiers.All(x => x.Data != Keywords.Private))
                    ? Keywords.Public
                    : Keywords.Private);

            if (methodDeclaration.Modifiers != null && methodDeclaration.Modifiers.Any(x => x.Data == Keywords.Static))
            {
                modifiers.Add(Keywords.Static);
            }

            return modifiers.Aggregate((x, y) => x + " " + y) + " ";
        }

        private static string GetArguments(TypeScriptCompiler compiler, MethodDeclaration methodDeclaration)
        {
            return
                methodDeclaration.Arguments == null ||
                methodDeclaration.Arguments.Count == 0
                    ? string.Empty
                    : methodDeclaration.Arguments
                        .Select(x => compiler.GetMethodArgumentString(x))
                        .Aggregate((x, y) => x + ", " + y);
        }

        private static string GetReturnType(TypeScriptCompiler compiler, MethodDeclaration methodDeclaration)
        {
            return 
                methodDeclaration.ReturnType == null
                    ? string.Empty
                    : compiler.GetTypeString(methodDeclaration.ReturnType, "MethodDeclarationCompiler -> GetReturnType");
        }

        private static MethodDetail GetMethodDetail(ICompiler compiler, MethodDeclaration methodDeclaration)
        {
            return JavaClassMetadata.GetClass(GetClassName(compiler)).GetMethod(methodDeclaration.Name.Data);
        }
    }
}
