using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Parsers
{
    public abstract class Parser : IParser
    {
        public int BufferSize;

        public InputElementDataSource DataSource { get; set; }

        public void ResetBuffer()
        {
            for (var i = 0; i < BufferSize; i++)
            {
                MoveToPreviousInputElement();
            }

            BufferSize = 0;
        }

        public IAstNode Parse(InputElementDataSource inputElementDataSource)
        {
            DataSource = inputElementDataSource;
            return ImplementationSpecificParse();
        }

        public CommentInputElement GetComment()
        {
            if (CurrentInputElement is CommentInputElement)
            {
                var comment = (CommentInputElement)CurrentInputElement;
                MoveToNextToken();
                return comment;
            }

            return null;
        }

        public abstract IAstNode ImplementationSpecificParse();

        public void MoveBack(int count)
        {
            for (var i = 0; i < count; i++)
            {
                MoveToPreviousInputElement();
            }
        }

        public bool Eof
        {
            get { return DataSource.Pointer >= DataSource.Count; }
        }

        public IInputElement CurrentInputElement
        {
            get { return DataSource.GetCurrentInputElement(); }
        }

        public IInputElement NextNonWhiteSpaceInputElement
        {
            get { return DataSource.NextNonWhiteSpaceInputElement(); }
        }

        public IInputElement PreviousNonWhiteSpaceInputElement
        {
            get { return DataSource.PreviousNonWhiteSpaceInputElement(); }
        }

        public bool IsWhiteSpace
        {
            get
            {
                return DataSource.IsWhiteSpace;
            }
        }

        public bool IsComment
        {
            get
            {
                return CurrentInputElement is CommentInputElement;
            }
        }

        public void MoveToNextInputElement()
        {
            DataSource.Pointer++;
        }

        public void MoveToPreviousInputElement()
        {
            DataSource.Pointer = DataSource.Pointer > 0 ? DataSource.Pointer - 1 : 0;
        }

        public void AssertNotWhiteSpace()
        {
            if (IsWhiteSpace)
            {
                throw new Exception("Expected non-whitespace.");
            }
        }

        public void AssertWhiteSpace()
        {
            if (CurrentInputElement.InputElementType != InputElementTypeEnum.WhiteSpace)
            {
                throw new Exception("White space expected.");
            }
        }

        public void AssertKeyword(string data)
        {
            AssertTokenType(TokenTypeEnum.Keyword);

            if (CurrentInputElement.Data != data)
            {
                throw new Exception(string.Format("Expected keyword '{0}', but instead found '{1}'.", data, CurrentInputElement.Data));
            }
        }

        public void AssertSeperator(string data)
        {
            AssertTokenType(TokenTypeEnum.Separator);
            
            if (CurrentInputElement.Data != data)
            {
                throw new Exception(string.Format("Expected '{0}', but instead found '{1}'.", data, CurrentInputElement.Data));
            }
        }

        public void AssertOperator(string data)
        {
            AssertTokenType<OperatorToken>();

            if (CurrentInputElement.Data != data)
            {
                throw new Exception(string.Format("Expected '{0}', but instead found '{1}'.", data, CurrentInputElement.Data));
            }
        }

        public void AssertToken()
        {
            if (!IsToken(CurrentInputElement))
            {
                throw new Exception(string.Format("Expected inputElement type of 'Token', instead found '{0}'.", CurrentInputElement.InputElementType.ToString()));
            }
        }

        public void AssertTokenData(string data)
        {
            AssertToken();
            if (CurrentInputElement.Data != data)
            {
                throw new Exception(string.Format("Expected data '{0}', instead found '{1}'.", data, CurrentInputElement.Data));
            }
        }

        public void AssertTokenType(TokenTypeEnum tokenTypeEnum)
        {
            AssertToken();

            if (((IToken)CurrentInputElement).TokenType != tokenTypeEnum)
            {
                throw new Exception(string.Format("Expected token type of '{0}', instead found '{1}'.", tokenTypeEnum.ToString(), ((IToken)CurrentInputElement).TokenType.ToString()));
            }
        }

        public void AssertTokenType<TTokenType>()
        {
            if (!(CurrentInputElement is TTokenType))
            {
                throw new Exception(string.Format("Expected token type of '{0}', instead found '{1}'.", typeof(TTokenType).Name, CurrentInputElement.GetType().Name));
            }
        }

        public void AssertIdentifier()
        {
            if (!IsIdentifier(CurrentInputElement))
            {
                throw new Exception(string.Format("Expected token type of '{0}', instead found '{1}'.", TokenTypeEnum.Identifier.ToString(), CurrentInputElement.GetType()));
            }
        }

        public bool CurrentTokenIs(string data)
        {
            return CurrentInputElement.Data == data;
        }

        public bool IsToken(IInputElement inputElement)
        {
            return inputElement.InputElementType == InputElementTypeEnum.Token;
        }

        public bool IsIdentifier(IInputElement inputElement)
        {
            return IsToken(inputElement) && ((IToken)inputElement).TokenType == TokenTypeEnum.Identifier;
        }

        public bool IsKeyword(IInputElement inputElement)
        {
            return IsToken(inputElement) && ((IToken)inputElement).TokenType == TokenTypeEnum.Keyword;
        }

        public bool IsLiteral(IInputElement inputElement)
        {
            return IsToken(inputElement) && ((IToken)inputElement).TokenType == TokenTypeEnum.Literal;
        }

        public bool IsSeperator(IInputElement inputElement)
        {
            return IsToken(inputElement) && ((IToken)inputElement).TokenType == TokenTypeEnum.Separator;
        }

        public bool IsOperator(IInputElement inputElement)
        {
            return IsToken(inputElement) && ((IToken)inputElement).TokenType == TokenTypeEnum.Operator;
        }

        public bool IsPrimitive(IInputElement inputElement)
        {
            return IsKeyword(inputElement) && Primitives.AsList.Any(p => p == inputElement.Data);
        }

        public bool IsMethodModifier(IInputElement inputElement)
        {
            return Keywords.MethodModifiers.Any(m => m == inputElement.Data);
        }

        public bool IsWhiteSpaceElement(IInputElement inputElement)
        {
            return inputElement is WhiteSpaceInputElement;
        }

        public IInputElement ForwardInputElement(int count)
        {
            return DataSource.ForwardInputElement(count);
        }

        public IInputElement ForwardToken(int count)
        {
            return DataSource.ForwardToken(count);
        }

        public void MoveToNextToken()
        {
            MoveToNextInputElement();
            while (!Eof && (IsWhiteSpace || IsComment))
            {
                MoveToNextInputElement();
            }
        }

        public IList<IAstNode> ParseBody()
        {
            var body = new List<IAstNode>();
            IInputElement previousNonWhiteSpace = new WhiteSpaceInputElement();

            AssertSeperator("{");
            MoveToNextToken();

            while (CurrentInputElement.Data != "}")
            {
                body.Add(ParseSingleStatement());
            }

            AssertSeperator("}");
            MoveToNextToken();

            return body;
        }

        public IAstNode ParseSingleStatement()
        {
            while (true)
            {
                if (CurrentInputElement.Data == ")" && NextNonWhiteSpaceInputElement.Data == "{")
                {
                    ResetBuffer();
                    return ParserHelper.Parse<MethodDeclarationParser>(DataSource);
                }

                //if (CurrentInputElement.Data == "{" && PreviousNonWhiteSpaceInputElement.Data != ")")
                //{
                //    // TODO: Parse initilializer - http://http://en.wikipedia.org/wiki/Java_syntax#Constructors_and_initializers
                //    throw new NotImplementedException();
                //}

                if (
                    (IsPrimitive(CurrentInputElement) || IsIdentifier(CurrentInputElement)) &&
                    IsWhiteSpaceElement(ForwardInputElement(1)) &&
                    IsIdentifier(ForwardInputElement(2)) &&
                    IsWhiteSpaceElement(ForwardInputElement(3)) &&
                    (IsOperator(ForwardInputElement(4)) && ForwardInputElement(4).Data == "="))
                {
                    ResetBuffer();
                    return ParserHelper.Parse<VariableDeclarationParser>(DataSource);
                }

                if (
                    (CurrentInputElement.Data == ")" && NextNonWhiteSpaceInputElement.Data == ";") ||
                    (CurrentInputElement.Data == "." && PreviousNonWhiteSpaceInputElement.Data == ")") //Chained function calls, like: task().doWithTask();
                )
                {
                    ResetBuffer();
                    var functionCall = ParserHelper.Parse<FunctionCallParser>(DataSource) as FunctionCallStatement;

                    if (CurrentInputElement.Data != ".")
                    {
                        Debug.Assert(CurrentInputElement.Data == ";");
                        functionCall.IsStandaloneStatement = true;
                        MoveToNextToken();
                    }

                    return functionCall;
                }

                if (CurrentInputElement.Data == "=" || (CurrentInputElement.Data == ";" && PreviousNonWhiteSpaceInputElement.Data != ")"))
                {
                    ResetBuffer();
                    return ParserHelper.Parse<VariableAssignmentParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.If)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<IfElseParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.Switch)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<SwitchStatementParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.While)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<WhileLoopParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.Do)
                {
                    // TODO: Parse do ... while loop control structure
                    throw new NotImplementedException();
                }

                if (CurrentInputElement.Data == Keywords.For)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<ForLoopParser>(DataSource);
                }

                if (CurrentInputElement.Data == ":")
                {
                    ResetBuffer();
                    return ParserHelper.Parse<LabelStatementParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.Break || CurrentInputElement.Data == Keywords.Continue)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<JumpStatementParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.Return)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<ReturnStatementParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.Try)
                {
                    // TODO: Parse try statement
                    throw new NotImplementedException();
                }

                if (CurrentInputElement.Data == Keywords.Throw)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<ThrowStatementParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.Assert)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<AssertStatementParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.Synchronized)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<SynchronizedStatementParser>(DataSource);
                }

                var implementationSpecificSingleStatement = ImplementationSpecificParseSingleStatement();
                if (implementationSpecificSingleStatement != null)
                {
                    return implementationSpecificSingleStatement;
                }

                BufferSize++;
                MoveToNextInputElement();
            }
        }

        public virtual IAstNode ImplementationSpecificParseSingleStatement()
        {
            return null;
        }
    }
}
