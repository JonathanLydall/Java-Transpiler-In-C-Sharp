using Mordritch.Transpiler.Java.AstGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Declarations
{
    public class ImportDeclaration : AstNode
    {
        public ImportDeclaration()
        {
            Content = "";
        }
        
        public string Content { get; set; }

        public override string DebugOut()
        {
            return string.Format("import {0};", Content);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            return returnList;
        }
    }
}
