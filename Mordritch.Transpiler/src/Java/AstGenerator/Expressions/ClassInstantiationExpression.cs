using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Expressions
{
    public class ClassInstantiationExpression : AstNode
    {
        public IInputElement ClassName;

        public IList<IList<IAstNode>> ArraySizes = new List<IList<IAstNode>>();

        public ArrayInitializationExpression ArrayContents = null;

        public int ArraySize = 0;

        public IList<IList<IAstNode>> InitializationData = new List<IList<IAstNode>>();

        public override string DebugOut()
        {
            var className = ClassName.Data;

            var initializationData = InitializationData.Count == 0
                ? string.Empty
                : InitializationData
                    .Select(xx => xx
                        .Select(x => x.DebugOut())
                        .Aggregate((x, y) => x + y))
                    .Aggregate((x, y) => x + ", " + y);

            var arraySizes = ArraySizes.Count == 0
                ? string.Empty
                : ArraySizes
                    .Select(x => string.Format("[{0}]", x.Count == 0
                        ? string.Empty
                        : x
                            .Select(xx => xx.DebugOut())
                            .Aggregate((xx, yy) => xx + " " + yy)))
                    .Aggregate((xx, yy) => xx + yy);

            var extraData = string.Empty;
            if (ArraySizes.Count == 0)
            {
                extraData = string.Format("({0})", initializationData);
            }
            else
            {
                extraData = ArrayContents != null
                                ? ArrayContents.DebugOut()
                                : arraySizes;
            }

            return string.Format("new {0}{1}", className, extraData);
        }

        public override IList<string> GetUsedTypes()
        {
            var returnList = new List<string>();

            AddUsedTypeIfIdentifierToken(ClassName, returnList);

            foreach (var arraySize in ArraySizes)
            {
                returnList = returnList.Union(GetUsedTypesFromAstNodes(arraySize)).ToList();
            }

#warning This is will return locally defined variables too.
            foreach (var initializationData in InitializationData)
            {
                returnList = returnList.Union(GetUsedTypesFromAstNodes(initializationData)).ToList();
            }

            return returnList;
        }
    }
}
