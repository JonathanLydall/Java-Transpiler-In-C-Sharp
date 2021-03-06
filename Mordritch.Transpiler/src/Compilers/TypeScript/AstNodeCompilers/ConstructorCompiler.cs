﻿using Mordritch.Transpiler.Compilers.TypeScript.Helpers;
using Mordritch.Transpiler.Contracts;
using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    class ConstructorCompiler
    {
        public const string CONSTRUCTOR_DISPATCHER_FUNCTION_NAME = "constructor_dispatcher";

        private readonly ICompiler _compiler;
        private readonly IList<MethodDeclaration> _constructors;
        private readonly ClassType _classType;
        private readonly bool _classIsExtending;
        private readonly JavaClass _classMetadata;
        private readonly string _targetMethodName;

        public ConstructorCompiler(ICompiler compiler, IList<MethodDeclaration> constructors, ClassType classType)
        {
            _compiler = compiler;
            _constructors = constructors;
            _classType = classType;
            _classIsExtending = !string.IsNullOrEmpty(_classType.Extends);
            _classMetadata = JavaClassMetadata.GetClass(_classType.Name);
            
            _targetMethodName = string.Format("{0}Constructor_", _classType.Name);
        }

        public void Compile()
        {
            if (_classMetadata.ConstructorNeedsExclusion())
            {
                BuildExclusionConstructor();
            }
            else
            {
                BuildNormalConstructor();
            }
        }

        private void BuildNormalConstructor()
        {
            BuildConstructorSignatures();
            
            BuildConstructor();

            if (_constructors.Count == 0)
            {
                BuildDispatcherForNoConstructor();
            }
            else if (_constructors.Count == 1)
            {
                BuildDispatcherForSingleConstructor();
            }
            else
            {
                BuildDispatcherForMultipleConstructors();
            }

            BuildConstructors();
        }

        private void BuildExclusionConstructor()
        {
            var superCall = _classIsExtending
                ? string.Format("super(super.{0}()); ", ClassTypeCompiler.NULL_CONSTRUCTOR_OBJECT_NAME)
                : string.Empty;

            _compiler.AddLine(string.Format("constructor(n?: any) {{ {0}}}", superCall));
            _compiler.AddLine(string.Format("public {0}(): void {{ }}", CONSTRUCTOR_DISPATCHER_FUNCTION_NAME));
            _compiler.AddBlankLine();

            _compiler.AddLine(string.Format("// Constructor marked as 'Exclude', compilation skipped: {0}", _classMetadata.ConstructorGetComment()));
            _compiler.BeginCommentingOut();
            {
                BuildNormalConstructor();
            }
            _compiler.EndCommentingOut();

        }

        private string GetClassName()
        {
            return _classType.Name;
        }

        private void BuildConstructorSignatures()
        {
            if (_constructors.Count == 0)
            {
                _compiler.AddLine("constructor();");
            }

            BuildNullConstructorSignature();

            foreach (var constructor in _constructors)
            {
                _compiler.AddLine(string.Format("constructor({0});", GetArguments(constructor)));
            }
        }

        private void BuildNullConstructorSignature()
        {
            _compiler.AddLine("constructor(n: any);");
        }

        private void BuildConstructor()
        {
            _compiler.AddLine(string.Format("constructor({0}) {{", OverloadHelper.GetDispatcherParameters(_constructors, "c_par")));
            _compiler.IncreaseIndentation();
            {
                if (_classIsExtending)
                {
                    _compiler.AddLine(string.Format("super(super.{0}());", ClassTypeCompiler.NULL_CONSTRUCTOR_OBJECT_NAME));
                    _compiler.AddBlankLine();
                }

                _compiler.AddLine(string.Format("if (c_par0 == this.{0}()) return;", ClassTypeCompiler.NULL_CONSTRUCTOR_OBJECT_NAME));
                _compiler.AddBlankLine();

                if (_constructors.Count > 0)
                {
                    _compiler.AddLine(string.Format("this.{0}({1});", CONSTRUCTOR_DISPATCHER_FUNCTION_NAME, GetConstructorDispatcherCallParameters()));
                }
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private void BuildDispatcherForNoConstructor()
        {
            _compiler.AddLine(string.Format("public {0}(): void {{", CONSTRUCTOR_DISPATCHER_FUNCTION_NAME));
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private void BuildDispatcherForSingleConstructor()
        {
            var argCount = _constructors.First().Arguments.Count;
            var constructorParms = argCount > 0
                ? OverloadHelper.GetDispatcherParameters(_constructors, "c_par")
                : string.Empty;

            _compiler.AddLine(string.Format("public {0}({1}): void {{", CONSTRUCTOR_DISPATCHER_FUNCTION_NAME, constructorParms));
            _compiler.IncreaseIndentation();
            {
                if (argCount == 0)
                {
                    _compiler.AddLine(string.Format("this.{0}0();", _targetMethodName));
                }
                else
                {
                    var parNumber = 0;
                    var arguments = _constructors.First().Arguments
                        .Select(x => string.Format("<{0}{1}>c_par{2}", _compiler.GetTypeString(x.Type, "GetConstructorCallCastType"), GetArrayDepth(x.ArrayDepth), parNumber++))
                        .Aggregate((x, y) => x + ", " + y);

                    _compiler.AddLine(string.Format("this.{0}0({1});", _targetMethodName, arguments));
                }
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private void BuildDispatcherForMultipleConstructors()
        {
            OverloadHelper.BuildDispatcher(_compiler, _constructors, CONSTRUCTOR_DISPATCHER_FUNCTION_NAME, Keywords.Void, _targetMethodName);
        }

        private void BuildConstructors()
        {
            var constructorNumber = 0;
            foreach (var constructor in _constructors)
            {
                _compiler.PushToContextStack(constructor);

                if (_classMetadata.ConstructorNeedsExtending())
                {
                    _compiler.AddLine(string.Format("// Constructor marked as 'Extend', compilation skipped: {0}", _classMetadata.ConstructorGetComment()));
                }
                else if (_classMetadata.ConstructorNeedsBodyOnlyExclusion())
                {
                    _compiler.AddLine(string.Format("// Constructor marked as 'ExcludeBodyOnly', compilation skipped: {0}", _classMetadata.ConstructorGetComment()));
                }

                _compiler.AddLine(string.Format("private {0}{1}({2}): void {{", _targetMethodName, constructorNumber++, GetArguments(constructor)));
                _compiler.IncreaseIndentation();
                {
                    var commentOut = _classMetadata.ConstructorNeedsExtending() || _classMetadata.ConstructorNeedsBodyOnlyExclusion();

                    if (_classMetadata.ConstructorNeedsExtending())
                    {
                        _compiler.AddLine("throw new Error(\"Constructor marked for 'Extend'.\");");
                    }

                    if (commentOut)
                    {
                        _compiler.BeginCommentingOut();
                        _compiler.CompileBody(constructor.Body);
                        _compiler.EndCommentingOut();
                    }
                    else
                    {
                        _compiler.CompileBody(constructor.Body);
                    }

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

        private string GetConstructorDispatcherCallParameters()
        {
            var argumentNumber = 0;

            if (_constructors.Count == 0 || _constructors.Max(x => x.Arguments.Count) == 0)
            {
                return string.Empty;
            }

            return _constructors
                .First(x => x.Arguments.Count == _constructors.Max(y => y.Arguments.Count))
                .Arguments
                .Select(x => string.Format("c_par{0}", argumentNumber++))
                .Aggregate((x, y) => x + ", " + y);
        }
    }
}
