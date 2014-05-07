using Mordritch.Transpiler.Contracts;
using Mordritch.Transpiler.Java.AstGenerator.Assignments;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.src;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class FieldCompiler
    {
        private ICompiler _compiler;
        private JavaClass _javaClass;
        private VariableDeclaration _variableDeclaration;

        public FieldCompiler(ICompiler compiler, VariableDeclaration variableDeclaration)
        {
            _compiler = compiler;
            _variableDeclaration = variableDeclaration;
            
            var className = ((ClassType)_compiler.GetPreviousContextFromStack(0)).Name;
            _javaClass = JavaClassMetadata.GetClass(className);
        }

        public void GenerateDefinition()
        {
            var fieldName = _variableDeclaration.VariableName.Data;
            var fieldType = _compiler.GetTypeString(_variableDeclaration.VariableType, "FieldCompiler GenerateDefinition()");
            var array = string.Empty;

            for (var a = 0; a < _variableDeclaration.ArrayCount; a++)
            {
                array += "[]";
            }

            var lineToAdd = IsExcluded()
                ? string.Format("// {0}: {1}{2};", fieldName, fieldType, array)
                : string.Format("{0}: {1}{2};", fieldName, fieldType, array);

            _compiler.AddLine(lineToAdd);
        }

        public void Compile()
        {
            var modifiers = GetModifiers();
            var fieldName = _variableDeclaration.VariableName.Data;
            var fieldType = _compiler.GetTypeString(_variableDeclaration.VariableType, "FieldCompiler Compile()");
            var arrayString = GetArrayString();
            var assignedValue = _compiler.GetInnerExpressionString(_variableDeclaration.AssignedValue);

            if (NeedsDelayedInitialisation() && _variableDeclaration.Modifiers.Any(x => x.Data == Keywords.Static) && _variableDeclaration.HasInitialization)
            {
                assignedValue = string.Format("(function () {{ window.addEventListener(\"load\", () => {{ {0}.{1} = {2}; }}); return null; }})()", _javaClass.Name, fieldName, assignedValue);
            }

            if (NeedsDelayedInitialisation() && !_variableDeclaration.Modifiers.Any(x => x.Data == Keywords.Static))
            {
                _compiler.AddLine(string.Format("// Could not set up delayed execution for field '{0}' as not static.", fieldName));
            }

            if (NeedsDelayedInitialisation() && !_variableDeclaration.HasInitialization)
            {
                _compiler.AddLine(string.Format("// Could not set up delayed execution for field '{0}' as static field not initialised.", fieldName));
            }

            var lineToAdd = _variableDeclaration.HasInitialization
                ? string.Format("{0}{1}: {2}{3} = {4};", modifiers, fieldName, fieldType, arrayString, assignedValue)
                : string.Format("{0}{1}: {2}{3};", modifiers, fieldName, fieldType, arrayString);


            if (_javaClass.HasDependantMethods(fieldName))
            {
                lineToAdd += string.Format(" // Was private, but changed to public due to dependancy by: {0}", _javaClass.GetDependantMethods(fieldName).Aggregate((x, y) => x + ", " + y));
            }

            if (IsExcluded())
            {
                var fieldComment = _javaClass.GetField(fieldName).GetComment();

                _compiler.AddBlankLine();
                _compiler.AddLine(string.Format("//Manually excluded field: {0}", fieldComment));
                _compiler.BeginCommentingOut();
                _compiler.AddLine(lineToAdd);
                _compiler.EndCommentingOut();
            }
            else
            {
                _compiler.AddLine(lineToAdd);
            }
        }

        private string GetArrayString()
        {
            var array = string.Empty;
            for (var a = 0; a < _variableDeclaration.ArrayCount; a++)
            {
                array += "[]";
            }

            return array;
        }

        private string GetModifiers()
        {
            if (_variableDeclaration.Modifiers.Count == 0 ||
                _variableDeclaration.Modifiers.All(x => TypeScriptUtils.Modifiers.All(y => x.Data != y)))
            {
                return string.Empty;
            }

            
            return _variableDeclaration.Modifiers
                .Where(x => TypeScriptUtils.Modifiers.Any(y => x.Data == y))
                .Select(x => MakePublicIfHasDependancy(x.Data))
                .Aggregate((x, y) => x + " " + y) + " ";
        }

        private string MakePublicIfHasDependancy(string modifierName)
        {
            if (modifierName != Keywords.Private)
            {
                return modifierName;
            }
            
            var fieldName = _variableDeclaration.VariableName.Data;
            var hasAtLeastOneDependancy = _javaClass.HasDependantMethods(fieldName);

            return hasAtLeastOneDependancy ? Keywords.Public : modifierName;
        }

        private bool IsExcluded()
        {
            var fieldName = _variableDeclaration.VariableName.Data;
            return _javaClass.GetField(fieldName).NeedsExclusion();
        }

        private bool NeedsDelayedInitialisation()
        {
            var fieldName = _variableDeclaration.VariableName.Data;
            return _javaClass.GetField(fieldName).NeedsDelayedInitialisation();
        }
    }
}
