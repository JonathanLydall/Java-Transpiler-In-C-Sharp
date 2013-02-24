using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.AstGenerator.Parsers;

namespace Mordritch.Transpiler.Java.AstGenerator
{
    public class AstGenerator : Parser
    {
        public IList<IAstNode> Parse(IList<IInputElement> inputElements)
        {
            DataSource = new InputElementDataSource(inputElements);

            var nodes = new List<IAstNode>();

            while (!Eof)
            {
                if (CurrentInputElement.InputElementType == InputElementTypeEnum.Token &&
                    ((IToken)CurrentInputElement).TokenType == TokenTypeEnum.Keyword &&
                    CurrentInputElement.Data == Keywords.Import)
                {
                    ResetBuffer();
                    nodes.Add(ParserHelper.Parse<ImportDeclarationParser>(DataSource));
                    continue;
                }

                if (CurrentInputElement.InputElementType == InputElementTypeEnum.Token &&
                    ((IToken)CurrentInputElement).TokenType == TokenTypeEnum.Keyword &&
                    CurrentInputElement.Data == Keywords.Package)
                {
                    ResetBuffer();
                    nodes.Add(ParserHelper.Parse<PackageDeclarationParser>(DataSource));
                    continue;
                }

                if (CurrentInputElement.InputElementType == InputElementTypeEnum.Token &&
                    ((IToken)CurrentInputElement).TokenType == TokenTypeEnum.Keyword &&
                    CurrentInputElement.Data == Keywords.Class)
                {
                    ResetBuffer();
                    nodes.Add(ParserHelper.Parse<ClassTypeParser>(DataSource));
                    continue;
                }

                Utils.Log(string.Format("Adding to buffer. Line {0}. Column {1}. Data: {2}", CurrentInputElement.Line, CurrentInputElement.Column, CurrentInputElement.Data), ConsoleColor.DarkGray);

                BufferSize++;
                MoveToNextInputElement();
            }

            return nodes;
        }

        public override IAstNode ImplementationSpecificParse()
        {
            throw new NotImplementedException();
        }
    }
}
