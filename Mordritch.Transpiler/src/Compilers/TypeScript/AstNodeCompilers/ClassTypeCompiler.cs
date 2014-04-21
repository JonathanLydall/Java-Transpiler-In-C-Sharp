using Mordritch.Transpiler.Compilers.TypeScript.Helpers;
using Mordritch.Transpiler.Contracts;
using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.src;
using Mordritch.Transpiler.src.Compilers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class ClassTypeCompiler
    {
        private ICompiler _compiler;

        private ClassType _classType;

        public static string NULL_CONSTRUCTOR_OBJECT_NAME = "NullConstructorObject";

        public ClassTypeCompiler(ICompiler compiler, ClassType classType)
        {
            _compiler = compiler;
            _classType = classType;
        }

        public void GenerateDefinition()
        {
            var name = _classType.Name;

            var extends = string.IsNullOrEmpty(_classType.Extends)
                ? string.Empty
                : " extends " + _classType.Extends;

            _compiler.AddBlankLine();

            _compiler.AddLine(string.Format("export interface {0} {{", name));
            _compiler.IncreaseIndentation();
            {
                _compiler.CompileDefinition(_classType.Body);
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        public void Compile()
        {
            var classMetadata = JavaClassMetadata.GetClass(_classType.Name);

            var modifiers = _classType.ModifierStatic
                ? "static "
                : string.Empty;

            var implements = _classType.Implements.Count == 0
                ? string.Empty
                : " implements " + _classType.Implements.Aggregate((x, y) => x + ", " + y);

            var extends = string.IsNullOrEmpty(_classType.Extends)
                ? string.Empty
                : " extends " + _classType.Extends;

            var name = classMetadata.NeedsExtending()
                ? string.Format("{0}_NeedsExtending", _classType.Name)
                : _classType.Name;

            _compiler.AddBlankLine();
            _compiler.AddLine(string.Format("export {0}class {1}{2}{3} {{", modifiers, name, extends, implements));
            _compiler.IncreaseIndentation();
            {
                CompileJavaClassExtensions();
                CompileNullConstructorObject();

                var fields = _classType.Body.Where(x => !(x is MethodDeclaration) && !(x is StaticInitializerDeclaration)).ToList();
                _compiler.CompileBody(fields);
                
                CompileConstructors();
                CompileMethods();

                _compiler.CompileBody(_classType.Body.Where(x => x is StaticInitializerDeclaration).ToList());

                GenerateMethodSignatures();
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();

        }

        private void CompileMethods()
        {
            var methods = _classType.Body.Where(x => x is MethodDeclaration && !IsConstructorMethod(x)).ToList();
            var builtOverloadedMethods = new List<string>();
            
            foreach (var method in methods)
            {
                var asMethodDeclaration = (MethodDeclaration)method;
                var isOverloaded = methods.Select(x => x as MethodDeclaration).Count(x => x.Name.Data == asMethodDeclaration.Name.Data) > 1;

                if (!isOverloaded)
                {
                    _compiler.CompileBody(new List<IAstNode> { method });
                    continue;
                }

                if (builtOverloadedMethods.Any(x => x == asMethodDeclaration.Name.Data))
                {
                    continue;
                }

                builtOverloadedMethods.Add(asMethodDeclaration.Name.Data);

                BuildOverloadedMethod(methods.Where(x => ((MethodDeclaration)x).Name.Data == asMethodDeclaration.Name.Data));
            }
        }

        private void BuildOverloadedMethod(IEnumerable<IAstNode> astNodes)
        {
            var methodNumber = 0;
            var methodDeclarations = astNodes.Select(x => (MethodDeclaration)x);
            var skipCompile = MethodDeclarationCompiler.SkipCompile(_compiler, methodDeclarations.First());

            var signatures = methodDeclarations
                .Select(x => MethodDeclarationCompiler.GetSignature((TypeScriptCompiler)_compiler, x))
                .Distinct();

            var returnType = methodDeclarations.Any(x => x.ReturnType != null && !string.IsNullOrWhiteSpace(x.ReturnType.Data))
                ? "any"
                : "void";

            var methodName = methodDeclarations.First().Name.Data;

            foreach (var signature in signatures)
            {
                _compiler.AddLine(string.Format("{0}{1};", skipCompile ? "// " : string.Empty, signature));
            }

            

            if (skipCompile)
            {
                _compiler.BeginCommentingOut();
            }

            OverloadHelper.BuildDispatcher(_compiler, astNodes.Select(x => (MethodDeclaration)x).ToList(), methodName, returnType, string.Format("{0}_", methodName));

            if (skipCompile)
            {
                _compiler.EndCommentingOut();
            }

            foreach (var methodDeclaration in methodDeclarations)
            {
                _compiler.PushToContextStack(methodDeclaration);

                var methodDeclarationCompiler = new MethodDeclarationCompiler(_compiler, methodDeclaration);
                methodDeclarationCompiler.Compile(string.Format("_{0}", methodNumber++));

                _compiler.PopFromContextStack();
            }
        }

        private void GenerateMethodSignatures()
        {
            var classMetadata = JavaClassMetadata.GetClass(_classType.Name);

            var list = classMetadata.Methods.Where(x => x.Action == MethodAction.GenerateSignature).ToList();

            foreach (var method in list)
            {
                _compiler.AddLine(string.Format("// {0}", method.Comments));
                _compiler.AddLine(string.Format("public {0} {{", method.Name.Trim()));
                _compiler.IncreaseIndentation();
                {
                    _compiler.AddLine("throw new Error(\"This is only a signature method and should never be called, check above comment.\");");
                }
                _compiler.DecreaseIndentation();
                _compiler.AddLine("}");
                _compiler.AddBlankLine();
            }
        }

        private bool IsConstructorMethod(IAstNode astNode)
        {
            if (!(astNode is MethodDeclaration)) {
                return false;
            }

            var methodDeclaration = astNode as MethodDeclaration;
            return methodDeclaration.Name.Data == _classType.Name;
        }

        private void CompileConstructors()
        {
            var constructors = _classType.Body.Count(x => x is MethodDeclaration && IsConstructorMethod(x)) < 1
                ? new List<MethodDeclaration>()
                : _classType.Body
                    .Where(x => x is MethodDeclaration && IsConstructorMethod(x))
                    .Select(x => x as MethodDeclaration)
                    .ToList();

            _compiler.AddBlankLine();
            var constructorCompiler = new ConstructorCompiler(_compiler, constructors, _classType);
            constructorCompiler.Compile();
        }

        private void CompileJavaClassExtensions()
        {
            _compiler.AddLine("// Begin providing a subset of Java class features:");
            _compiler.AddLine(string.Format("public static class: any = {0};", _classType.Name));
            _compiler.AddBlankLine();

            JavaClassFunctionality_Implements();
            JavaClassFunctionality_NewInstanceMethod();
            JavaClassFunctionality_IsInstanceMethod();
            JavaClassFunctionality_InstanceOfMethod();
            JavaClassFunctionality_IsAssignableFromMethod();
            JavaClassFunctionality_GetClassMethod();

            _compiler.AddLine("// Finished providing subset of Java class features.");
            _compiler.AddBlankLine();
        }

        private void JavaClassFunctionality_Implements()
        {
            var implements = _classType.Implements != null && _classType.Implements.Count > 0
                ? _classType.Implements.Select(x => string.Format("\"{0}\"", x)).Aggregate((x, y) => x + ", " + y)
                : string.Empty;

            _compiler.AddLine("// List of Java Interfaces this class implements:");
            _compiler.AddLine(string.Format("public static implements: string[] = [{0}];", implements));
            _compiler.AddBlankLine();
        }

        private void JavaClassFunctionality_NewInstanceMethod()
        {
            _compiler.AddLine(string.Format("public static newInstance(): {0} {{", _classType.Name));
            _compiler.IncreaseIndentation();
            {
                _compiler.AddLine(string.Format("return new {0}.class();", _classType.Name));
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }
        
        private void JavaClassFunctionality_InstanceOfMethod()
        {
            _compiler.AddLine("public instanceOf(object: any): boolean {");
            _compiler.IncreaseIndentation();
            {
                _compiler.AddLine(string.Format("return {0}.isInstance(object);", _classType.Name));
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private void JavaClassFunctionality_IsInstanceMethod()
        {
            _compiler.AddLine("public static isInstance(object: any): boolean {");
            _compiler.IncreaseIndentation();
            {
                var parentCheck = _classType.Extends == null || JavaLangPackages.Contains(_classType.Extends)
                    ? string.Empty
                    : string.Format(" || {0}.isInstance(object)", _classType.Extends);

                _compiler.AddLine("if (typeof object == \"string\") {");
                _compiler.IncreaseIndentation();
                {
                    _compiler.AddLine(string.Format("return {0}.implements.indexOf(object) > -1{1};", _classType.Name, parentCheck));
                }
                _compiler.DecreaseIndentation();
                _compiler.AddLine("}");
                _compiler.AddLine("else {");
                _compiler.IncreaseIndentation();
                {
                    _compiler.AddLine(string.Format("return {0}.class == object.class{1};", _classType.Name, parentCheck));
                }
                _compiler.DecreaseIndentation();
                _compiler.AddLine("}");
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }
        
        private void JavaClassFunctionality_IsAssignableFromMethod()
        {
            _compiler.AddLine("public static isAssignableFrom(object: any): boolean {");
            _compiler.IncreaseIndentation();
            {
                _compiler.AddLine(string.Format("return object.isInstance({0});", _classType.Name));
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private void JavaClassFunctionality_GetClassMethod()
        {
            _compiler.AddLine(string.Format("public getClass(): {0} {{", _classType.Name));
            _compiler.IncreaseIndentation();
            {
                _compiler.AddLine(string.Format("return {0}.class;", _classType.Name));
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private void CompileNullConstructorObject()
        {
            _compiler.AddLine("// TypeScript enforces that the super method is always called first in the constructor in some cases. This gets in the way of reproducing having overloaded constructors which can call");
            _compiler.AddLine("// each other or various overloaded constructors in parent classes. The function below is part of a pattern which allows us to work around this.");
            _compiler.AddLine(string.Format("public {0}(): any {{ if (typeof this[\"__{0}\"] == \"undefined\") {{ this[\"__{0}\"] = {{}}; }} return this[\"__{0}\"]; }}", NULL_CONSTRUCTOR_OBJECT_NAME));
            _compiler.AddBlankLine();
        }
    }
}
