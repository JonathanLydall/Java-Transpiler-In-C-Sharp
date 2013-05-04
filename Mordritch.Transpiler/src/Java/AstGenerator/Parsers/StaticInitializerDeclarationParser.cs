using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public class StaticInitializerDeclarationParser : Parser, IParser
    {
        private StaticInitializerDeclaration _staticInitializerDeclaration = new StaticInitializerDeclaration();

        public override IAstNode ImplementationSpecificParse()
        {
            _staticInitializerDeclaration.PreComment = GetComment();

            Debug.Assert(CurrentInputElement is KeywordToken);
            Debug.Assert(CurrentInputElement.Data == Keywords.Static);
            _staticInitializerDeclaration.StaticKeywordToken = CurrentInputElement;
            MoveToNextToken();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "{");
            _staticInitializerDeclaration.Body = ParseBody();

            return _staticInitializerDeclaration;
        }
    }
}
