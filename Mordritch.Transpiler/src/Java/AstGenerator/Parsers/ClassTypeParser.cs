using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class ClassTypeParser : Parser, IParser
    {
        private ClassType _classType = new ClassType();

        public override IAstNode ImplementationSpecificParse()
        {
            ProcessModifiers();
            
            AssertKeyword(Keywords.Class);
            MoveToNextToken();

            _classType.Name = CurrentInputElement.Data;
            MoveToNextToken();
            
            ProcessImplementsAndExtends();

            _classType.Body = ParseBody();

            return _classType;
        }

        private void ProcessModifiers()
        {
            var buffer = new List<IInputElement>();
            while (CurrentInputElement.Data != "class")
            {
                buffer.Add(CurrentInputElement);
                MoveToNextToken();
            }
            
            _classType.AccessModifierPublic = buffer.Any(b => b.Data == Keywords.Public);
            _classType.AccessModifierPrivate = buffer.Any(b => b.Data == Keywords.Private);
            _classType.AccessModifierProtected = buffer.Any(b => b.Data == Keywords.Protected);
            _classType.ModifierAbstract = buffer.Any(b => b.Data == Keywords.Abstract);
            _classType.ModifierFinal = buffer.Any(b => b.Data == Keywords.Final);
            _classType.ModifierStatic = buffer.Any(b => b.Data == Keywords.Static);
            _classType.ModifierStrictfp = buffer.Any(b => b.Data == Keywords.Strictfp);
        }

        private void ProcessImplementsAndExtends()
        {
            var isImplementing = false;
            while (CurrentInputElement.Data != "{")
            {
                if (CurrentInputElement.Data == Keywords.Extends)
                {
                    isImplementing = false;

                    AssertKeyword(Keywords.Extends);
                    MoveToNextInputElement();

                    AssertWhiteSpace();
                    MoveToNextInputElement();

                    _classType.Extends = CurrentInputElement.Data;
                    MoveToNextInputElement();
                    continue;
                }

                if (CurrentInputElement.Data == Keywords.Implements)
                {
                    AssertKeyword(Keywords.Extends);
                    MoveToNextInputElement();

                    AssertWhiteSpace();
                    MoveToNextInputElement();

                    isImplementing = true;
                    MoveToNextInputElement();
                    continue;
                }

                if (IsWhiteSpace)
                {
                    MoveToNextInputElement();
                    continue;
                }

                if (isImplementing && CurrentInputElement.Data == ",")
                {
                    MoveToNextInputElement();
                    continue;
                }

                if (isImplementing)
                {
                    AssertTokenType(TokenTypeEnum.Identifier);
                    _classType.Implements.Add(CurrentInputElement.Data);
                    MoveToNextInputElement();
                    continue;
                }

                throw new Exception(string.Format("Unexpected input element '{0}', with data '{1}'.", CurrentInputElement.GetInputElementType(), CurrentInputElement.Data));
            }

            AssertSeperator("{");
        }
    }
}
