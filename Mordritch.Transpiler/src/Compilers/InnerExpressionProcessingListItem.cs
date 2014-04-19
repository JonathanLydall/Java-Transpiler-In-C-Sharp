using Mordritch.Transpiler.Compilers;
using Mordritch.Transpiler.Java.AstGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers
{
    public class InnerExpressionProcessingListItem
    {
        public bool Processed { get; set; }
        public IAstNode AstNode { get; set; }
        public string Output { get; set; }
    }
}
