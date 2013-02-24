using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator
{
    public class InputElementDataSource
    {
        private IList<IInputElement> _inputElementList;

        public int Count
        {
            get { return _inputElementList.Count; }
        }

        public int Pointer = 0;

        public InputElementDataSource(IList<IInputElement> inputElementList)
        {
            _inputElementList = inputElementList;
        }

        public bool IsWhiteSpace
        {
            get
            {
                return GetCurrentInputElement() is WhiteSpaceInputElement;
            }
        }

        public IInputElement GetCurrentInputElement()
        {
            return _inputElementList[Pointer];
        }

        /*
        Contrary to the method name, the fallback / default value is a whitespace. This allows us to be able to
        use the .Data field of the result without risking a null value exception.
        */
        public IInputElement NextNonWhiteSpaceInputElement()
        {
            var pointer = Pointer + 1;
            while (pointer < Count)
            {
                if (_inputElementList[pointer].InputElementType != InputElementTypeEnum.WhiteSpace)
                {
                    return _inputElementList[pointer];
                }
                pointer++;
            }
            
            return new WhiteSpaceInputElement();
        }

        /*
        Contrary to the method name, the fallback / default value is a whitespace. This allows us to be able to
        use the .Data field of the result without risking a null value exception.
        */
        public IInputElement PreviousNonWhiteSpaceInputElement()
        {
            var pointer = Pointer - 1;
            while (pointer >= 0)
            {
                if (!(_inputElementList[pointer] is WhiteSpaceInputElement))
                {
                    return _inputElementList[pointer];
                }
                pointer--;
            }

            return new WhiteSpaceInputElement();
        }

        internal IInputElement ForwardInputElement(int count)
        {
            var pointer = Pointer + count;

            return _inputElementList.Count < pointer ? null : _inputElementList[pointer];
        }

        internal IInputElement ForwardToken(int desiredTokenCount)
        {
            var pointer = Pointer + 1;
            var tokenCounter = 0;

            while (pointer < _inputElementList.Count)
            {
                if (_inputElementList[pointer] is TokenInputElement)
                {
                    tokenCounter++;
                }

                if (tokenCounter < desiredTokenCount && pointer < _inputElementList.Count)
                {
                    pointer++;
                }
                else
                {
                    break;
                }
            }

            return tokenCounter < desiredTokenCount ? null : _inputElementList[pointer];
        }
    }
}
