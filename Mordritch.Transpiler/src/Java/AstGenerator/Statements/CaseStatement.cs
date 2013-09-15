using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class CaseStatement : AstNode
    {
        public IList<IInputElement> CaseValue = new List<IInputElement>();

        public override string DebugOut()
        {
            var caseValue =
                CaseValue
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + y);

            return string.Format("case {0}:", caseValue);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromInputElements(CaseValue)).ToList();

            return returnList;
        }
    }
}
