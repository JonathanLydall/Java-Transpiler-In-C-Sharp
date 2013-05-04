using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    class PackageDeclarationParser : Parser, IParser
    {
        public override IAstNode ImplementationSpecificParse()
        {
            Debug.Assert(CurrentInputElement.Data == Keywords.Package);
            MoveToNextInputElement();

            Debug.Assert(CurrentInputElement is WhiteSpaceInputElement);
            MoveToNextInputElement();

            var packageDeclaration = new PackageDeclaration();
            while (CurrentInputElement.Data != ";")
            {
                packageDeclaration.Content.Add(CurrentInputElement);
                MoveToNextInputElement();
            }
            MoveToNextInputElement();

            return packageDeclaration;
        }

    }
}
