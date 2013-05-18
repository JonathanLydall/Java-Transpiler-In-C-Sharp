using Mordritch.Transpiler.Java.AstGenerator.Assignments;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class VariableAssignmentCompiler
    {
        private ICompiler _compiler;

        private VariableAssignment _variableAssignment;
        
        public VariableAssignmentCompiler(ICompiler compiler, VariableAssignment variableAssignment)
        {
            _compiler = compiler;
            _variableAssignment = variableAssignment;
        }

        public void Compile()
        {
            string variableName = string.Empty;
            IInputElement previousElement = null;
            foreach (var inputElement in _variableAssignment.VariableName)
            {
                if (previousElement == null || previousElement.Data != ".")
                {
                    variableName += string.Format("{0}{1}", ClarifyScope(inputElement.Data), inputElement.Data);
                }
                else {
                    variableName += inputElement.Data;
                }
                previousElement = inputElement;
            }

            var assignmentOperator = string.Format(" {0} ", _variableAssignment.AssignmentOperator == null ? string.Empty : _variableAssignment.AssignmentOperator.Data);

            var assignedValue = _variableAssignment.AssignmentOperator == null
                ? string.Empty
                : _variableAssignment.AssignedValue
                    .Select(x => _compiler.GetExpressionString(x))
                    .Aggregate((x, y) => x + y);

            _compiler.AddLine(string.Format("{0}{1}{2};", variableName, assignmentOperator, assignedValue));
        }

        private string ClarifyScope(string identifierName)
        {
            if (identifierName.Contains("["))
            {
                identifierName = identifierName.Substring(0, identifierName.IndexOf("["));
            }
            
            if (identifierName.Contains("."))
            {
                return string.Empty;
            }

            var stack = _compiler.GetFullContextStack();
            var classTypeItem = stack.LastOrDefault(x => x is ClassType);

            if (classTypeItem == null)
            {
                return string.Empty;
            }

            var classType = classTypeItem as ClassType;

            var staticVariables = classType.Body
                .Where(x => x is VariableDeclaration && ((VariableDeclaration)x).Modifiers.Any(y => y.Data == Keywords.Static))
                .Select(x => ((VariableDeclaration)x).VariableName.Data).ToArray();

            var variables = classType.Body
                .Where(x => x is VariableDeclaration && ((VariableDeclaration)x).Modifiers.All(y => y.Data != Keywords.Static))
                .Select(x => ((VariableDeclaration)x).VariableName.Data).ToArray();

            if (staticVariables.Any(x => x == identifierName))
            {
                return string.Format("{0}.", classType.Name);
            }

            if (staticVariables.Any(x => x == identifierName))
            {
                return "this.";
            }

            return string.Empty;
        }
    }
}
