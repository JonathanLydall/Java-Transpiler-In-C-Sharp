using Mordritch.Transpiler.Java.AstGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class SwitchDefaultStatement : AstNode
    {
        public override string DebugOut()
        {
            return "default:";
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();
            return returnList;
        }
    }
}
