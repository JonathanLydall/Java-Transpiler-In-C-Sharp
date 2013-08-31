using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class MethodDeclarationCompiler
    {
        private ICompiler _compiler;

        private MethodDeclaration _methodDeclaration;

        public MethodDeclarationCompiler(ICompiler compiler, MethodDeclaration methodDeclaration)
        {
            _compiler = compiler;
            _methodDeclaration = methodDeclaration;
        }

        public void GenerateDefinition()
        {
            var className = GetClassName();
            var methodName = _methodDeclaration.Name.Data;
            var skipCompile = className != null && Excluder.ShouldExclude(className, methodName) != null;
            var potentiallyCommented = skipCompile ? "// " : string.Empty;

            if (skipCompile)
            {
                Excluder.IsExcluding = true;
            }

            var methodArguments = GetArguments();
            var returnType = GetReturnType();
            var arrayDepth = GetArrayDepth();


            if (isConstructorMethod())
            {
                _compiler.AddLine(string.Format("{0}constructor({1});", potentiallyCommented, methodArguments));
            }
            else
            {
                _compiler.AddLine(string.Format("{0}{1}({2}): {3}{4};", potentiallyCommented, methodName, methodArguments, returnType, arrayDepth));
            }

            if (skipCompile)
            {
                Excluder.IsExcluding = false;
            }
        }

        public void Compile()
        {
            var methodName = _methodDeclaration.Name.Data;
            var className = GetClassName();
            var skipCompile = className != null && Excluder.ShouldExclude(className, methodName) != null;
            var skipBody = className != null && Excluder.ShouldExcludeBody(className, methodName) != null;

            var isAbstractMethod = false;

            _compiler.AddBlankLine();
            if (skipCompile)
            {
                _compiler.AddWarning(
                    _methodDeclaration.Name.Line,
                    _methodDeclaration.Name.Column,
                    string.Format("Excluded methodDeclaration {0}: {1}", methodName, Excluder.ShouldExclude(className, methodName)));

                Excluder.IsExcluding = true;

                _compiler.AddLine("/*");
            }            
            
            var modifiers = GetModifiers();
            var methodArguments = GetArguments();
            var returnType = GetReturnType();
            var arrayDepth = GetArrayDepth();

            if (_methodDeclaration.ThrowsType != null)
            {
                var description = string.Format("Detected a throws type on method declaration, this is not being included in the TypeScript output and may need to be handled manually. Throws Type: {0}",
                    _methodDeclaration.ThrowsType.Data);

                _compiler.AddWarning(
                    _methodDeclaration.ThrowsType.Line,
                    _methodDeclaration.ThrowsType.Column,
                    description);
            }

            if (_methodDeclaration.Body == null)
            {
                Debug.Assert(_methodDeclaration.Modifiers.Any(x => x.Data == Keywords.Abstract), "Unexpected null body, without abstract keyword.");
                isAbstractMethod = true;
            }

            if (isConstructorMethod())
            {
                _compiler.AddLine(string.Format("constructor({0}) {{", methodArguments));
            }
            else
            {
                _compiler.AddLine(string.Format("{0}{1}({2}): {3}{4} {{", modifiers, methodName, methodArguments, returnType, arrayDepth));
            }

            _compiler.IncreaseIndentation();
            {
                if (skipBody)
                {
                    _compiler.AddWarning(_methodDeclaration.Name.Line, _methodDeclaration.Name.Column, "Skipped compiling body of method declaration as per entry in ExcludeBodyOnly.csv.");
                }
                else if (isAbstractMethod)
                {
                    _compiler.AddLine(@"throw new Error(""This was an abstract method in the Java source code and should not be called unless overridden in a derived class first."");");
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
                _compiler.AddLine("*/");
                Excluder.IsExcluding = false;
            }
            _compiler.AddBlankLine();
        }

        private bool isConstructorMethod()
        {
            var parentContext = _compiler.GetPreviousContextFromStack(1);
            
            return
                parentContext != null &&
                parentContext is ClassType &&
                (parentContext as ClassType).Name == _methodDeclaration.Name.Data;
        }

        private string GetClassName()
        {
            var parentContext = _compiler.GetPreviousContextFromStack(1);

            return parentContext != null && parentContext is ClassType
                ? (parentContext as ClassType).Name
                : null;
        }

        private string GetArrayDepth()
        {
            var arrayString = string.Empty;

            for (var i = 0; i < _methodDeclaration.ReturnArrayDepth; i++)
            {
                arrayString += "[]";
            }

            return arrayString;
        }

        private string GetModifiers()
        {
            var allowedModifiers = new[] { 
                Keywords.Private,
                Keywords.Public,
                Keywords.Static };

            return
                _methodDeclaration.Modifiers == null ||
                _methodDeclaration.Modifiers.Count == 0 ||
                _methodDeclaration.Modifiers.Count(x => allowedModifiers.Any(m => m == x.Data)) == 0
                    ? string.Empty
                    : _methodDeclaration.Modifiers
                        .Where(x => allowedModifiers.Any(m => m == x.Data))
                        .Select(x => x.Data)
                        .Aggregate((x, y) => x + " " + y) + " ";
        }

        private string GetArguments()
        {
            return
                _methodDeclaration.Arguments == null ||
                _methodDeclaration.Arguments.Count == 0
                    ? string.Empty
                    : _methodDeclaration.Arguments
                        .Select(x => _compiler.GetMethodArgumentString(x))
                        .Aggregate((x, y) => x + ", " + y);
        }

        private string GetReturnType()
        {
            return 
                _methodDeclaration.ReturnType == null
                    ? string.Empty
                    : _compiler.GetTypeString(_methodDeclaration.ReturnType, "MethodDeclarationCompiler -> GetReturnType");
        }
    }
}
