using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes;
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

        private IList<IInputElement> ElementBuffer = new List<IInputElement>();
        
        public void ResetBuffer()
        {
            for (var i = 0; i < BufferSize; i++)
            {
                MoveToPreviousInputElement();
            }

            ElementBuffer.Clear();
            BufferSize = 0;
        }

        public IList<IAstNode> GetInnerExpression(string endCharacter)
        {
            var contents = new List<IAstNode>();

            while (CurrentInputElement.Data != endCharacter)
            {
                contents.Add(ParseExpression());
            }

            return contents;
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

        public bool CurrentTokenIs(string data)
        {
            return CurrentInputElement.Data == data;
        }

        public bool IsPrimitive(IInputElement inputElement)
        {
            return inputElement is KeywordToken && Primitives.AsList.Any(p => p == inputElement.Data);
        }

        public bool IsMethodModifier(IInputElement inputElement)
        {
            return Keywords.MethodModifiers.Any(m => m == inputElement.Data);
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

        public IList<IInputElement> GetBufferElements()
        {
            var buffer = new List<IInputElement>();
            
            for (var i = 1; i <= BufferSize; i++)
            {
                buffer.Add(ForwardInputElement(-i));
            }

            return buffer;
        }

        public IList<IAstNode> ParseBody()
        {
            var body = new List<IAstNode>();
            IInputElement previousNonWhiteSpace = new WhiteSpaceInputElement();

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "{");
            MoveToNextToken();

            while (CurrentInputElement.Data != "}")
            {
                if (CurrentInputElement is WhiteSpaceInputElement)
                {
                    MoveToNextInputElement();
                    continue;
                }

                body.Add(ParseSingleStatement());
            }

            Debug.Assert(CurrentInputElement is SeperatorToken);
            Debug.Assert(CurrentInputElement.Data == "}");
            MoveToNextToken();

            return body;
        }

        public IAstNode ParseSingleStatement()
        {
            while (true)
            {
                if (MethodDeclarationParser.IsMatch(this))
                {
                    ResetBuffer();
                    return ParserHelper.Parse<MethodDeclarationParser>(DataSource);
                }

                //if (CurrentInputElement.Data == "{" && PreviousNonWhiteSpaceInputElement.Data != ")")
                //{
                //    // TODO: Parse initilializer - http://http://en.wikipedia.org/wiki/Java_syntax#Constructors_and_initializers
                //    throw new NotImplementedException();
                //}

                if (IsVariableDeclaration())
                {
                    ResetBuffer();
                    return ParserHelper.Parse<VariableDeclarationParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.Static && ForwardToken(1).Data == "{")
                {
                    ResetBuffer();
                    return ParserHelper.Parse<StaticInitializerDeclarationParser>(DataSource) as StaticInitializerDeclaration;
                }

                if (SimpleStatementParser.IsSimpleStatement(this))
                {
                    ResetBuffer();
                    return ParserHelper.Parse<SimpleStatementParser>(DataSource) as SimpleStatement;
                }

                if (CurrentInputElement.Data == ";" &&
                    ElementBuffer.Any(x => x.Data == Keywords.Abstract) &&
                    ElementBuffer.Any(x => x.Data == "(") &&
                    ElementBuffer.Any(x => x.Data == ")"))
                {
                    ResetBuffer();
                    return ParserHelper.Parse<MethodDeclarationParser>(DataSource);
                }

                if (JavaUtils.IsAssignmentOperator(CurrentInputElement.Data) || (CurrentInputElement.Data == ";" && PreviousNonWhiteSpaceInputElement.Data != ")"))
                {
                    ResetBuffer();
                    return ParserHelper.Parse<VariableAssignmentParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.If)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<IfElseStatementParser>(DataSource);
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
                    ResetBuffer();
                    return ParserHelper.Parse<DoWhileLoopParser>(DataSource);
                }

                if (CurrentInputElement.Data == Keywords.For)
                {
                    ResetBuffer();
                    return ParserHelper.Parse<ForLoopParser>(DataSource);
                }

                if (LabelStatementParser.IsLabel(CurrentInputElement, BufferSize, DataSource))
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
                    ResetBuffer();
                    return ParserHelper.Parse<TryStatementParser>(DataSource);
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

                ElementBuffer.Add(CurrentInputElement);
                BufferSize++;
                MoveToNextInputElement();
            }
        }

        private bool IsVariableDeclaration()
        {
            if (!IsPrimitive(CurrentInputElement) && !(CurrentInputElement is IdentifierToken))
            {
                return false;
            }

            var possibleOffsetDueToArray = 0;
            while (ForwardToken(possibleOffsetDueToArray + 1).Data == "[")
            {
                if (ForwardToken(possibleOffsetDueToArray + 2).Data != "]")
                {
                    return false;
                }

                possibleOffsetDueToArray += 2;
            }

            if (!(ForwardToken(1 + possibleOffsetDueToArray) is IdentifierToken))
            {
                return false;
            }

            var tokenAfterIdentifier = ForwardToken(2 + possibleOffsetDueToArray);
            return (tokenAfterIdentifier.Data == ";" || tokenAfterIdentifier.Data == "=");
        }

        public IAstNode ParseExpression()
        {

            if (CurrentInputElement.Data == "(" &&
                ForwardInputElement(2).Data != ")")
            {
                return ParseExpressionBracketed();
            }

            if (CurrentInputElement.Data == "(" &&
                ForwardInputElement(2).Data == ")")
            {
                return ParseExpressionTypeCast();
            }

            if (CurrentInputElement.Data == Keywords.New)
            {
                return ParserHelper.Parse<ClassInstantiationExpressionParser>(DataSource);
            }

            if (
                (CurrentInputElement is IdentifierToken || CurrentInputElement.Data == Keywords.Super || CurrentInputElement.Data == Keywords.This) &&
                ForwardInputElement(1).Data == "(")
            {
                return ParseExpressionMethodCall();
            }

            var identifier = new IdentifierExpression(CurrentInputElement);
            MoveToNextToken();

            return identifier;
        }

        public MethodCallExpression ParseExpressionMethodCall()
        {
            var methodCallExpression = new MethodCallExpression();

            methodCallExpression.MethodIdentifier = CurrentInputElement;
            MoveToNextToken();

            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextToken();

            while (CurrentInputElement.Data != ")")
            {
                methodCallExpression.Parameters.Add(ParseExpressionParameter());
            }

            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextToken();

            return methodCallExpression;
        }

        public IList<IAstNode> ParseExpressionParameter()
        {
            var parameter = new List<IAstNode>();

            var delimeters = new[] { ",", ")", "}" };

            while (delimeters.All(x => x != CurrentInputElement.Data))
            {
                parameter.Add(ParseExpression());
            }

            if (CurrentInputElement.Data == ",")
            {
                MoveToNextToken();
            }

            return parameter;
        }

        public IAstNode ParseExpressionTypeCast()
        {
            var typeCastExpression = new TypeCastExpression();

            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextToken();

            typeCastExpression.CastTarget = CurrentInputElement;
            MoveToNextToken();

            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextToken();

            while (CurrentInputElement.Data != ")" && CurrentInputElement.Data != ";" && CurrentInputElement.Data != ",")
            {
                typeCastExpression.InnerExpressions.Add(ParseExpression());
            }

            return typeCastExpression;
        }

        public IAstNode ParseExpressionBracketed()
        {
            var bracketedExpression = new BracketedExpression();

            Debug.Assert(CurrentInputElement.Data == "(");
            MoveToNextToken();

            while (CurrentInputElement.Data != ")")
            {
                bracketedExpression.InnerExpressions.Add(ParseExpression());
            }

            Debug.Assert(CurrentInputElement.Data == ")");
            MoveToNextToken();

            return bracketedExpression;
        }

        public virtual IAstNode ImplementationSpecificParseSingleStatement()
        {
            return null;
        }
    }
}
