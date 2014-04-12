using Mordritch.Transpiler.Java.AstGenerator;
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
                
                _compiler.CompileBody(_classType.Body.Where(x =>  !(x is MethodDeclaration) && !(x is StaticInitializerDeclaration)).ToList());
                CompileConstructors();

                _compiler.CompileBody(_classType.Body.Where(x => x is MethodDeclaration && !IsConstructorMethod(x)).ToList());
                _compiler.CompileBody(_classType.Body.Where(x => x is StaticInitializerDeclaration).ToList());
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();

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
            _compiler.AddLine("// Used to provide certain Class features provided by Java and used by the Minecraft source code:");
            _compiler.AddLine(string.Format("public static class: any = {0}; public static newInstance(): {0} {{ return new {0}.class(); }} public getClass(): {0} {{ return {0}.class; }}", _classType.Name));
            _compiler.AddBlankLine();
        }

        private void CompileNullConstructorObject()
        {
            _compiler.AddLine("// TypeScript enforces that the super method is always called first in the constructor in some");
            _compiler.AddLine("// cases. This gets in the way of reproducing having overloaded constructors which can call");
            _compiler.AddLine("// each other or various overloaded constructors in parent classes. The function below is part");
            _compiler.AddLine("// of a pattern which allows us to work around this.");
            _compiler.AddLine(string.Format("public {0}(): {0} {{ if (typeof this[\"__{0}\"] == \"undefined\") {{ this[\"__{0}\"] = {{}}; }} return this[\"__{0}\"]; }}", NULL_CONSTRUCTOR_OBJECT_NAME));
            _compiler.AddBlankLine();
        }
    }
}
