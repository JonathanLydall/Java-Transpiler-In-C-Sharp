﻿using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    class ConstructorCompiler
    {
        public const string CONSTRUCTOR_DISPATCHER_FUNCTION_NAME = "constructor_dispatcher";
        
        private ICompiler _compiler;
        private IList<MethodDeclaration> _constructors;
        private ClassType _classType;

        public ConstructorCompiler(ICompiler compiler, IList<MethodDeclaration> constructors, ClassType classType)
        {
            _compiler = compiler;
            _constructors = constructors;
            _classType = classType;
        }

        public void Compile()
        {
            if (!string.IsNullOrEmpty(_classType.Extends))
            {

            }
            
            BuildConstructorSignatures();
            BuildConstructorDispatcher();
            BuildConstructors();
        }

        public string GetClassName()
        {
            return _classType.Name;
        }

        private void BuildConstructorSignatures()
        {
            foreach (var constructor in _constructors)
            {
                _compiler.AddLine(string.Format("constructor({0});", GetArguments(constructor)));
            }

            _compiler.AddLine(string.Format("constructor({0}) {{", GetConstructorParameters()));
            _compiler.IncreaseIndentation();
            {
                _compiler.AddLine(string.Format("this.{0}({1});", CONSTRUCTOR_DISPATCHER_FUNCTION_NAME, GetConstructorDispatcherCallParameters()));
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private void BuildConstructorDispatcher()
        {
            var constructorNumber = 0;

            _compiler.AddLine(string.Format("private {0}({1}): void {{", CONSTRUCTOR_DISPATCHER_FUNCTION_NAME, GetConstructorParameters()));
            _compiler.IncreaseIndentation();
            {
                foreach (var constructor in _constructors)
                {
                    _compiler.AddLine("if (");
                    _compiler.IncreaseIndentation();
                    {
                        var parametersAreDefined = GetParametersAreDefined(constructor);
                        var parametersAreOfRightType = GetParametersAreOfRightType(constructor);

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
                        var parNumber = 1;
                        var arguments = constructor.Arguments
                            .Select(x => string.Format("<{0}{1}>c_par{2}", _compiler.GetTypeString(x.Type, "GetConstructorCallCastType"), GetArrayDepth(x.ArrayDepth), parNumber++))
                            .Aggregate((x, y) => x + ", " + y);

                        _compiler.AddLine(string.Format("this.constructor_{0}({1});", constructorNumber++, arguments));
                    }
                    _compiler.DecreaseIndentation();

                    _compiler.AddLine("}");
                    _compiler.AddBlankLine();
                }
            }

            _compiler.AddLine(@"throw new Error(""Unrecognized constructor called."")");

            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private void BuildConstructors()
        {
            var constructorNumber = 0;
            foreach (var constructor in _constructors)
            {
                _compiler.PushToContextStack(constructor); 
                
                _compiler.AddLine(string.Format("private constructor_{0}({1}): void {{", constructorNumber++, GetArguments(constructor)));
                _compiler.IncreaseIndentation();
                {
                    _compiler.CompileBody(constructor.Body);

                }
                _compiler.DecreaseIndentation();
                _compiler.AddLine("}");
                _compiler.AddBlankLine();

                _compiler.PopFromContextStack();
            }
        }

        private string GetArrayDepth(int depth)
        {
            var returnString = string.Empty;
            for (var i = 0; i < depth; i++)
            {
                returnString += "[]";
            }

            return returnString;
        }

        private string GetParametersAreOfRightType(MethodDeclaration constructor)
        {
            var isOfRightTypeNumber = 1;
            return constructor.Arguments.Count == 0
                ? string.Empty
                : constructor.Arguments
                    .Select(x => GetTypeCheck(x, isOfRightTypeNumber++))
                    .Aggregate((x, y) => x + " && " + y);
        }

        private string GetParametersAreDefined(MethodDeclaration constructor)
        {
            var isDefinedNumber = 0;

            return _constructors
                .First(x => x.Arguments.Count == _constructors.Max(y => y.Arguments.Count))
                .Arguments
                .Select(x => string.Format(@"(typeof c_par{0} {1} ""undefined"")", isDefinedNumber + 1, isDefinedNumber++ < constructor.Arguments.Count ? "!=" : "=="))
                .Aggregate((x, y) => x + " && " + y);
        }

        private string GetArguments(MethodDeclaration methodDeclaration)
        {
            return
                methodDeclaration.Arguments == null ||
                methodDeclaration.Arguments.Count == 0
                    ? string.Empty
                    : methodDeclaration.Arguments
                        .Select(x => _compiler.GetMethodArgumentString(x))
                        .Aggregate((x, y) => x + ", " + y);
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

        private string GetConstructorParameters()
        {
            var argumentNumber = 1;

            return _constructors
                .First(x => x.Arguments.Count == _constructors.Max(y => y.Arguments.Count))
                .Arguments
                .Select(x => string.Format("c_par{0}?: any", argumentNumber++))
                .Aggregate((x, y) => x + ", " + y);
        }

        private string GetConstructorDispatcherCallParameters()
        {
            var argumentNumber = 1;

            return _constructors
                .First(x => x.Arguments.Count == _constructors.Max(y => y.Arguments.Count))
                .Arguments
                .Select(x => string.Format("c_par{0}", argumentNumber++))
                .Aggregate((x, y) => x + ", " + y);
        }
    }
}