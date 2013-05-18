using Mordritch.Transpiler.Java.AstGenerator.ControlStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class ForLoopCompiler
    {
        private ICompiler _compiler;

        private ForLoop _forLoop;

        public ForLoopCompiler(ICompiler compiler, ForLoop forLoop)
        {
            _compiler = compiler;
            _forLoop = forLoop;
        }

        public void Compile()
        {
            var initializers = GetInitializersString();
            var condition = _compiler.GetInnerExpressionString(_forLoop.Condition);
            
            var counterExpressions = _forLoop.CounterExpressions.Count > 0
                ? _forLoop.CounterExpressions
                    .Select(x => _compiler.GetSimpleStatementString(x))
                    .Aggregate((x, y) => x + ", " + y)
                : string.Empty;

            _compiler.AddBlankLine();
            _compiler.AddLine(string.Format("for ({0}; {1}; {2}) {{", initializers, condition, counterExpressions));
            
            _compiler.IncreaseIndentation();
            {
                _compiler.CompileBody(_forLoop.Body);
            }
            _compiler.DecreaseIndentation();
            
            _compiler.AddLine("}");
            _compiler.AddBlankLine();
        }

        private string GetInitializersString()
        {
            if (_forLoop.Initializers.Count == 0)
            {
                return string.Empty;
            }
            
            var initializers = new List<string>();

            foreach (var initializer in _forLoop.Initializers)
            {
                var initializedValue = initializer.AssignedValue
                    .Select(x => _compiler.GetExpressionString(x))
                    .Aggregate((x, y) => x + y);
                
                initializers.Add(
                    string.Format("{0}{1} = {2}",
                        initializer.VariableName.Data,
                        initializer.InitializedType == null ? string.Empty : string.Format(": {0}", _compiler.GetTypeString(initializer.InitializedType, "ForLoop -> GetInitializersString")),
                        initializedValue));
            }



            var varString = _forLoop.Initializers.Any(x => x.InitializedType != null)
                ? "var "
                : string.Empty;

            return varString + initializers.Aggregate((x, y) => x + ", ");
        }
    }
}
