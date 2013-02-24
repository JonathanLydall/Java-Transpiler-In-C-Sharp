using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class MethodFunctionCallParamater : IFunctionCallParameter
    {
        private FunctionCallStatement _functionCallParameter;

        public MethodFunctionCallParamater(FunctionCallStatement functionCallParameter)
        {
            _functionCallParameter = functionCallParameter;
        }

        public string DebugOut()
        {
            return _functionCallParameter.DebugOut(true);
        }
    }
}
