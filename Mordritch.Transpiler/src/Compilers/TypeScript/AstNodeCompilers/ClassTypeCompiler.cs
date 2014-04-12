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
            if (_classType.Body.Count(x => x is MethodDeclaration && IsConstructorMethod(x)) <= 1)
            {
                _compiler.CompileBody(_classType.Body
                    .Where(x => x is MethodDeclaration && IsConstructorMethod(x))
                    .ToList());

                return;
            }

            var constructors = _classType.Body
                .Where(x => x is MethodDeclaration && IsConstructorMethod(x))
                .Select(x => x as MethodDeclaration)
                .ToList();

            _compiler.AddBlankLine();
            var constructorCompiler = new ConstructorCompiler(_compiler, constructors, _classType);
            constructorCompiler.Compile();

            return;

            _compiler.PushToContextStack(constructors.First());
            _compiler.AddLine(string.Format("constructor({0}) {{", GetConstructorParameters(constructors)));
            _compiler.IncreaseIndentation();
            {
                foreach (var constructor in constructors)
                {

                    _compiler.AddLine("if (");
                    _compiler.IncreaseIndentation();
                    {
                        var parametersAreDefined = GetParametersAreDefined(constructors, constructor);
                        var parametersAreOfRightType = GetParametersAreOfRightType(constructors, constructor);
                        
                        if (string.IsNullOrEmpty(parametersAreOfRightType))
                        {
                            _compiler.AddLine(parametersAreDefined);
                        }
                        else
                        {
                            _compiler.AddLine(string.Format("{0} &&", parametersAreDefined));
                            _compiler.AddLine(parametersAreOfRightType);
                        }
                    }
                    _compiler.DecreaseIndentation();
                    _compiler.AddLine(") {");
                    _compiler.IncreaseIndentation();
                    {
                        _compiler.AddLine(string.Format("// Original: constructor({0})", GetOriginalConstructorArguments(constructor)));
                        GenerateConstructorToVariables(constructor);

                        _compiler.CompileBody(constructor.Body);

                        _compiler.AddBlankLine();
                        _compiler.AddLine("return;");
                    }
                    _compiler.DecreaseIndentation();
                    _compiler.AddLine("}");
                    _compiler.AddBlankLine();
                }
                _compiler.AddBlankLine();
                _compiler.AddLine(@"throw new Error(""Unrecognized constructor called."")");
            }
            _compiler.DecreaseIndentation();
            _compiler.PopFromContextStack();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private string GetOriginalConstructorArguments(MethodDeclaration constructor)
        {
            return constructor.Arguments == null || constructor.Arguments.Count == 0
                ? string.Empty
                : constructor.Arguments
                    .Select(x => _compiler.GetMethodArgumentString(x))
                    .Aggregate((x, y) => x + ", " + y);
        }

        private void GenerateConstructorToVariables(MethodDeclaration constructor)
        {
            if (constructor.Arguments == null || constructor.Arguments.Count == 0)
            {
                return;
            }

            var arguments = constructor.Arguments.Select(x => _compiler.GetMethodArgumentString(x)).ToList();
            var parNumber = 1;
            foreach (var argument in arguments)
            {
                _compiler.AddLine(string.Format("var {0} = c_par{1};", argument, parNumber++));
            }
            _compiler.AddBlankLine();
        }

        private string GetParametersAreOfRightType(IList<MethodDeclaration> constructors, MethodDeclaration constructor)
        {
            var isOfRightTypeNumber = 1;
            return constructor.Arguments.Count == 0
                ? string.Empty
                : constructor.Arguments
                    .Select(x => GetTypeCheck(x, isOfRightTypeNumber++))
                    .Aggregate((x, y) => x + " && " + y);
        }

        private string GetConstructorParameters(IList<MethodDeclaration> constructors)
        {
            var argumentNumber = 1;
            
            return constructors
                .First(x => x.Arguments.Count == constructors.Max(y => y.Arguments.Count))
                .Arguments
                .Select(x => string.Format("c_par{0}?: any", argumentNumber++))
                .Aggregate((x, y) => x + ", " + y);
        }

        private string GetParametersAreDefined(IList<MethodDeclaration> constructors, MethodDeclaration constructor)
        {
            var isDefinedNumber = 0;

            return constructors
                .First(x => x.Arguments.Count == constructors.Max(y => y.Arguments.Count))
                .Arguments
                .Select(x => string.Format(@"(typeof c_par{0} {1} ""undefined"")", isDefinedNumber + 1, isDefinedNumber++ < constructor.Arguments.Count ? "!=" : "=="))
                .Aggregate((x, y) => x + " && " + y);
        }

        private string GetTypeCheck(MethodArgument methodArgument, int count)
        {
            if (methodArgument.ArrayDepth > 0)
            {
                return string.Format("(c_par{0} instanceof Array)", count);
            }

            var type = PrimitiveMapper.IsTypeOfMap(methodArgument.Type.Data);
            if (type != null)
            {
                return string.Format(@"(typeof c_par{0} == ""{1}"")", count, type);
            }

            return string.Format(@"(typeof c_par{0} == ""function"" || typeof c_par{0} == ""object"")", count);
            
        }

        private void CompileJavaClassExtensions()
        {
            _compiler.AddLine("// Used to provide certain Class features provided by Java and used by the Minecraft source code:");
            _compiler.AddLine(string.Format("public static class: any = {0}; public static newInstance(): {0} {{ return new {0}.class(); }} public getClass(): {0} {{ return {0}.class; }}", _classType.Name));
            _compiler.AddBlankLine();
        }
    }
}
