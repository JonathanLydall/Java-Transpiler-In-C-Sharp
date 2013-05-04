using Mordritch.Transpiler.Java.AstGenerator.Assignments;
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
            var variableName = _variableAssignment.VariableName.Count == 0
                ? string.Empty
                : _variableAssignment.VariableName
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + y);

            var assignmentOperator = string.Format(" {0} ", _variableAssignment.AssignmentOperator == null ? string.Empty : _variableAssignment.AssignmentOperator.Data);

            var assignedValue = _variableAssignment.AssignmentOperator == null
                ? string.Empty
                : _variableAssignment.AssignedValue
                    .Select(x => _compiler.GetExpressionString(x))
                    .Aggregate((x, y) => x + y);

            _compiler.AddLine(string.Format("{0}{1}{2};", variableName, assignmentOperator, assignedValue));
        }
    }
}
