using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Declarations
{
    public class VariableDeclaration : AstNode
    {
        public bool HasInitialization = false;

        public int ArrayCount = 0;

        public IList<IInputElement> Modifiers = new List<IInputElement>();

        public IList<IAstNode> AssignedValue = new List<IAstNode>();

        public IInputElement VariableType;

        public IInputElement VariableName;

        public override string DebugOut()
        {
            var assignedValue = !HasInitialization
                ? string.Empty
                : " = " + AssignedValue
                    .Select(x => x.DebugOut())
                    .Aggregate((x, y) => x + " " + y);
            
            var modifiers = Modifiers.Count == 0
                ? string.Empty
                : Modifiers
                    .Select(x => x.Data)
                    .Aggregate((x, y) => x + " " + y) + " ";

            var array = string.Empty;
            for (var a = 0; a < ArrayCount; a++)
            {
                array += "[]";
            }

            return string.Format("{0}{1} {2}{3}{4};", modifiers, VariableType.Data, VariableName.Data, array, assignedValue);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            returnList = returnList.Union(GetUsedTypesFromAstNodes(AssignedValue)).ToList();
            returnList = returnList.Union(GetUsedTypesFromInputElements(Modifiers)).ToList();
            AddUsedTypeIfIdentifierToken(VariableType, returnList);

            return returnList;
        }
    }
}
