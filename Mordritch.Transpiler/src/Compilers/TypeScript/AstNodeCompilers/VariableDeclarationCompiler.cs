using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class VariableDeclarationCompiler
    {
        private ICompiler _compiler;

        private VariableDeclaration _variableDeclaration;

        public VariableDeclarationCompiler(ICompiler compiler, VariableDeclaration variableDeclaration)
        {
            _compiler = compiler;
            _variableDeclaration = variableDeclaration;
        }

        public void GenerateDefinition()
        {
            var variableName = _variableDeclaration.VariableName.Data;
            var variableType = _compiler.GetTypeString(_variableDeclaration.VariableType, "VariableDeclarationCompiler variableType");
            var array = string.Empty;

            for (var a = 0; a < _variableDeclaration.ArrayCount; a++)
            {
                array += "[]";
            }

            var lineToAdd = string.Format("{0}: {1}{2};", variableName, variableType, array);
        }

        public void Compile()
        {
            var isClassTypeContext = _compiler.GetCurrentContextFromStack() is ClassType;
            
            var assignedValue = !_variableDeclaration.HasInitialization
                ? string.Empty
                : " = " + _variableDeclaration.AssignedValue
                    .Select(x => _compiler.GetExpressionString(x))
                    .Aggregate((x, y) => x + y);

            var modifiers = _variableDeclaration.Modifiers.Count == 0 || _variableDeclaration.Modifiers.All(x => TypeScriptUtils.Modifiers.All(tsm => x.Data != tsm))
                ? string.Empty
                : _variableDeclaration.Modifiers
                    .Where(x => TypeScriptUtils.Modifiers.Any(tsm => x.Data == tsm))
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + " " + y) + " ";

            if (!isClassTypeContext)
            {
                modifiers += "var ";
            }

            var array = string.Empty;
            for (var a = 0; a < _variableDeclaration.ArrayCount; a++)
            {
                array += "[]";
            }

            var variableType = _compiler.GetTypeString(_variableDeclaration.VariableType, "VariableDeclarationCompiler");

            var variableName = _variableDeclaration.VariableName.Data;

            var lineToAdd = string.Format("{0}{1}: {2}{3}{4};", modifiers, variableName, variableType, array, assignedValue);

            _compiler.AddLine(lineToAdd);
        }
    }
}
