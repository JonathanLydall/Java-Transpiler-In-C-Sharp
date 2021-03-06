﻿using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    class ImportDeclarationParser : Parser, IParser
    {
        public override IAstNode ImplementationSpecificParse()
        {
            Debug.Assert(CurrentInputElement.Data == Keywords.Import);
            MoveToNextInputElement();

            Debug.Assert(CurrentInputElement is WhiteSpaceInputElement);
            MoveToNextInputElement();

            var importDeclaration = new ImportDeclaration();
            while (CurrentInputElement.Data != ";")
            {
                importDeclaration.Content += CurrentInputElement.Data;
                MoveToNextInputElement();
            }
            MoveToNextInputElement();

            return importDeclaration;
        }

    }
}
