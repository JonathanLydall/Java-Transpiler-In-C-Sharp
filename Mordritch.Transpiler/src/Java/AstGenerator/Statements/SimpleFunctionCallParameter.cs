using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class SimpleFunctionCallParameter : IFunctionCallParameter
    {
        public IList<IInputElement> InputElements
        {
            get
            {
                return _parameterInputElements;
            }
        }
        
        private IList<IInputElement> _parameterInputElements;
        
        public SimpleFunctionCallParameter(IList<IInputElement> parameterInputElements)
        {
            _parameterInputElements = parameterInputElements;
        }

        public string DebugOut()
        {
            return _parameterInputElements.Count == 0 ? string.Empty : _parameterInputElements.Select(x => x.Data).Aggregate((x, y) => x + " " + y);
        }
    }
}
