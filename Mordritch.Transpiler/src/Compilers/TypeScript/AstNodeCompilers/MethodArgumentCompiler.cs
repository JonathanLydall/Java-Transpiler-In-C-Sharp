using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class MethodArgumentCompiler
    {
        private ICompiler _compiler;

        private MethodArgument _methodArgument;

        public MethodArgumentCompiler(ICompiler compiler, MethodArgument methodArgument)
        {
            _compiler = compiler;
            _methodArgument = methodArgument;
        }

        public string GetMethodArgumentString()
        {
            var argumentModifier = _methodArgument.Modifier == null ? string.Empty : _methodArgument.Modifier.Data; // 

            var variableArity = !_methodArgument.IsVariableArity ? string.Empty : "...";

            var argumentName = _methodArgument.Name.Data;

            if (_methodArgument.IsVariableArity)
            {
                var description = "VariableArity parameter detected which is unsupported by TypeScript. This situation will need to be handled manually in the output code.";

                _compiler.AddWarning(
                    _methodArgument.Name.Line,
                    _methodArgument.Name.Column,
                    description);
            }

            if (_methodArgument.Modifier != null)
            {
                var description = string.Format(
                    "Modifier detected in method argument which is unsupported by TypeScript, may need manual handling in the code. Name: {0} Modifer: {1}",
                    argumentName,
                    argumentModifier);

                _compiler.AddWarning(
                    _methodArgument.Modifier.Line,
                    _methodArgument.Modifier.Column,
                    description);
            }

            return string.Format("{0}: {1}", argumentName, _compiler.GetTypeString(_methodArgument.Type));
        }
    }
}
