﻿using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    class PackageDeclarationParser : Parser, IParser
    {
        public override IAstNode ImplementationSpecificParse()
        {
            AssertKeyword(Keywords.Package);
            MoveToNextInputElement();

            AssertWhiteSpace();
            MoveToNextInputElement();

            var packageDeclaration = new PackageDeclaration();
            while (CurrentInputElement.Data != ";")
            {
                packageDeclaration.Content += CurrentInputElement.Data;
                MoveToNextInputElement();
            }
            MoveToNextInputElement();

            return packageDeclaration;
        }

    }
}
