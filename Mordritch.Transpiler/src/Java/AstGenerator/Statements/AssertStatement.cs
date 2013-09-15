using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Statements
{
    public class AssertStatement : AstNode
    {
        public IList<IInputElement> Message = null;

        public IList<IInputElement> Condition = new List<IInputElement>();

        public override string DebugOut()
        {
            var message = Message.Select(x => x.Data).Aggregate((x, y) => x + " " + y);

            var condition = Condition.Select(x => x.Data).Aggregate((x, y) => x + " " + y);

            return string.Format("assert({0}, {1});", condition, message);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromInputElements(Condition)).ToList();

            return returnList;
        }
    }
}
