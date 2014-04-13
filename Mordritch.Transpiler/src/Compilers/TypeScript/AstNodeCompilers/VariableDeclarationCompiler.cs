using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mordritch.Transpiler.src.Compilers;
using Mordritch.Transpiler.src;
using Mordritch.Transpiler.Contracts;
using Mordritch.Transpiler.Java.Common;


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

            _compiler.AddLine(string.Format("{0}: {1}{2};", variableName, variableType, array));
        }

        public void Compile()
        {
            var variableType = _compiler.GetTypeString(_variableDeclaration.VariableType, "VariableDeclarationCompiler");
            var variableName = _variableDeclaration.VariableName.Data;
            var arrayString = GetArrayString();
            var assignedValue = _compiler.GetInnerExpressionString(_variableDeclaration.AssignedValue);

            var lineToAdd = _variableDeclaration.HasInitialization
                ? string.Format("var {0}: {1}{2} = {3};", variableName, variableType, arrayString, assignedValue)
                : string.Format("var {0}: {1}{2};", variableName, variableType, arrayString);

            _compiler.AddLine(lineToAdd);
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
    }
}
