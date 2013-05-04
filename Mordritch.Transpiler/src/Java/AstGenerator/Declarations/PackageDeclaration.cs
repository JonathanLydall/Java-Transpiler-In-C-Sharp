using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Declarations
{
    public class PackageDeclaration : AstNode
    {
        public IList<IInputElement> Content = new List<IInputElement>();

        public override string DebugOut()
        {
            var content = Content == null || Content.Count == 0
                ? string.Empty
                : Content
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + y);
            
            return string.Format("package {0};", content);
        }
    }
}
