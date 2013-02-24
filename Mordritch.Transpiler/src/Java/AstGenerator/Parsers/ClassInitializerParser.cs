using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    class ClassInitializerParser : Parser, IParser
    {
        private ClassInitializerDeclaration _classInitilializerDeclaration = new ClassInitializerDeclaration();

        public override IAstNode ImplementationSpecificParse()
        {
            return _classInitilializerDeclaration;
        }

        private void ProcessComment()
        {
            
        }
    }
}
