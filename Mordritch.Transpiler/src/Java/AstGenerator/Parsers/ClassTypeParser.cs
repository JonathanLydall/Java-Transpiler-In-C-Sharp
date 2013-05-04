using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            Debug.Assert(CurrentInputElement.Data == Keywords.Class);
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

                    Debug.Assert(CurrentInputElement is KeywordToken);
                    Debug.Assert(CurrentInputElement.Data == Keywords.Extends);
                    MoveToNextToken();

                    _classType.Extends = CurrentInputElement.Data;
                    MoveToNextToken();
                    continue;
                }

                if (CurrentInputElement.Data == Keywords.Implements)
                {
                    Debug.Assert(CurrentInputElement is KeywordToken);
                    Debug.Assert(CurrentInputElement.Data == Keywords.Implements);
                    MoveToNextToken();

                    isImplementing = true;
                    continue;
                }

                if (isImplementing && CurrentInputElement.Data == ",")
                {
                    MoveToNextToken();
                    continue;
                }

                if (isImplementing)
                {
                    Debug.Assert(CurrentInputElement is IdentifierToken);
                    _classType.Implements.Add(CurrentInputElement.Data);
                    MoveToNextToken();
                    continue;
                }

                throw new Exception(string.Format("Unexpected input element '{0}', with data '{1}'.", CurrentInputElement.GetInputElementType(), CurrentInputElement.Data));
            }

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "{");
        }
    }
}
