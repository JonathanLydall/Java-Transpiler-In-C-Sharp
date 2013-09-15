using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator
{
    public abstract class AstNode : IAstNode
    {
        public CommentInputElement PreComment { get; set; }

        public CommentInputElement PostComment { get; set; }

        public abstract IList<string> GetUsedTypes();

        public abstract string DebugOut();

        public IList<string> GetUsedTypesFromAstNodes(IList<IAstNode> astNodes)
        {
            var returnList = new List<string>();

            if (astNodes == null)
            {
                return returnList;
            }

            foreach (var node in astNodes)
            {
                returnList = returnList.Union(node.GetUsedTypes()).ToList();
            }

            return returnList;
        }

        public IList<string> GetUsedTypesFromInputElements(IList<IInputElement> inputElements)
        {
            var returnList = new List<string>();

            foreach (var inputElement in inputElements)
            {
                if (inputElement is IdentifierToken)
                {
                    returnList.Add(inputElement.Data);
                }
            }

            return returnList;
        }

        public void AddUsedTypeIfIdentifierToken(IInputElement inputElement, IList<string> list)
        {
            if (inputElement is IdentifierToken)
            {
                list.Add(inputElement.Data);

                if (inputElement.Data == "par1")
                {
                }
            }
        }
    }
}
