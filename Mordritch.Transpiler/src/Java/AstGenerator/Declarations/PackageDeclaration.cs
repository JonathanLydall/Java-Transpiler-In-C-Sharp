using Mordritch.Transpiler.Java.AstGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Declarations
{
    class PackageDeclaration : AstNode
    {
        public PackageDeclaration()
        {
            Content = "";
        }
        
        public string Content { get; set; }

        public override string DebugOut()
        {
            return string.Format("package {0};", Content);
        }
    }
}
