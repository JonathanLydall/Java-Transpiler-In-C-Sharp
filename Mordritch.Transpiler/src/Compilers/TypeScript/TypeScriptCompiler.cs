using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mordritch.Transpiler.Java.AstGenerator.Assignments;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Loops;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Statements;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures;
using Mordritch.Transpiler.Java.AstGenerator;

using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;

using Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers;
using Mordritch.Transpiler.Compilers.TypeScript;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes;
using Mordritch.Transpiler.Java.Common;

namespace Mordritch.Transpiler.Compilers.TypeScript
{
    public class TypeScriptCompiler : ICompiler
    {
        private int _indentation = 0;

        private const string _indenter = "    ";

        private string _indentationString = string.Empty;

        private StringBuilder _stringBuilder = new StringBuilder();

        private bool _blankLinePending = false;

        private IList<CompilerWarning> _warnings = new List<CompilerWarning>();

        private IList<IAstNode> _contextStack = new List<IAstNode>();

        private bool _previousLineEndedWithOpeningCurlyBrace = false;

        public static string Compile(IList<IAstNode> data)
        {
            var instance = new TypeScriptCompiler();
            instance.AddLine("module Mordritch {");
            instance.IncreaseIndentation();
            {
                instance.CompileBody(data);
            }
            instance.DecreaseIndentation();
            instance.AddLine("}");
            return instance.GetOutput();
        }

        public void AddToContextStack(IAstNode astNode)
        {
            _contextStack.Add(astNode);
        }

        public void RemoveFromContextStack()
        {
            _contextStack.Remove(_contextStack.Last());
        }

        public IAstNode GetCurrentContextFromStack()
        {
            return _contextStack.Last();
        }

        public IAstNode GetPreviousContextFromStack(int depth)
        {
            var elementNumber = _contextStack.Count - 1 - depth;

            return elementNumber >= 0 ? _contextStack[elementNumber] : null;
        }
        
        public void AddBlankLine()
        {
            _blankLinePending = true;
        }
        
        public void AddLine(string line)
        {
            if (_blankLinePending == true && line != "}" && !_previousLineEndedWithOpeningCurlyBrace)
            {
                _stringBuilder.AppendLine(_indentationString);
            }
            _blankLinePending = false;

            _stringBuilder.AppendLine(_indentationString + line);
            _previousLineEndedWithOpeningCurlyBrace = line.Substring(line.Length - 1) == "{";
        }

        public void AddWarning(int line, int column, string description)
        {
            AddLine(string.Format("// TODO - Review warning: {0}", description));
            _warnings.Add(new CompilerWarning(line, column, description));
        }

        public void IncreaseIndentation()
        {
            _indentation++;
            SetIndentationString();
        }

        public void DecreaseIndentation()
        {
            if (_indentation == 0)
            {
                throw new Exception("Already at no indentation.");
            }

            _indentation--;
            SetIndentationString();
        }

        public string GetOutput()
        {
            return _stringBuilder.ToString();
        }

        private void SetIndentationString()
        {
            _indentationString = string.Empty;
            for (var i = 0; i < _indentation; i++)
            {
                _indentationString += _indenter;
            }
        }

        public void CompileVariableAssignment(VariableAssignment variableAssignment)
        {
            var compiler = new VariableAssignmentCompiler(this, variableAssignment);
            compiler.Compile();
        }

        public void CompileDoWhileLoop(DoWhileLoop doWhileLoop)
        {
            var compiler = new DoWhileLoopCompiler(this, doWhileLoop);
            compiler.Compile();
        }

        public void CompileForLoop(ForLoop forLoop)
        {
            var compiler = new ForLoopCompiler(this, forLoop);
            compiler.Compile();
        }

        public void CompileWhileLoop(WhileLoop whileLoop)
        {
            var compiler = new WhileLoopCompiler(this, whileLoop);
            compiler.Compile();
        }

        public void CompileCatchStatement(CatchStatement catchStatement)
        {
            var compiler = new CatchStatementCompiler(this, catchStatement);
            compiler.Compile();
        }

        public void CompileIfElseStatement(IfElseStatement ifElseStatement)
        {
            var compiler = new IfElseStatementCompiler(this, ifElseStatement);
            compiler.Compile();
        }

        public void CompileSwitchStatement(SwitchStatement switchStatement)
        {
            var compiler = new SwitchStatementCompiler(this, switchStatement);
            compiler.Compile();
        }

        public void CompileTryStatement(TryStatement tryStatement)
        {
            var compiler = new TryStatementCompiler(this, tryStatement);
            compiler.Compile();
        }

        public void CompileClassInitializerDeclaration(ClassInitializerDeclaration classInitializerDeclaration)
        {
            var compiler = new ClassInitializerDeclarationCompiler(this, classInitializerDeclaration);
            compiler.Compile();
        }

        public void CompileImportDeclaration(ImportDeclaration importDeclaration)
        {
            var compiler = new ImportDeclarationCompiler(this, importDeclaration);
            compiler.Compile();
        }

        public string GetMethodArgumentString(MethodArgument methodArgument)
        {
            var compiler = new MethodArgumentCompiler(this, methodArgument);
            return compiler.GetMethodArgumentString();
        }

        public void CompileMethodDeclaration(MethodDeclaration methodDeclaration)
        {
            var compiler = new MethodDeclarationCompiler(this, methodDeclaration);
            compiler.Compile();
        }

        public void CompilePackageDeclaration(PackageDeclaration packageDeclaration)
        {
            var compiler = new PackageDeclarationCompiler(this, packageDeclaration);
            compiler.Compile();
        }

        public void CompileAssertStatement(AssertStatement assertStatement)
        {
            var compiler = new AssertStatementCompiler(this, assertStatement);
            compiler.Compile();
        }

        public void CompileCaseStatement(CaseStatement caseStatement)
        {
            var compiler = new CaseStatementCompiler(this, caseStatement);
            compiler.Compile();
        }

        public void CompileJumpStatement(JumpStatement jumpStatement)
        {
            var compiler = new JumpStatementCompiler(this, jumpStatement);
            compiler.Compile();
        }

        public void CompileLabelStatement(LabelStatement labelStatement)
        {
            var compiler = new LabelStatementCompiler(this, labelStatement);
            compiler.Compile();
        }

        public void CompileReturnStatement(ReturnStatement returnStatement)
        {
            var compiler = new ReturnStatementCompiler(this, returnStatement);
            compiler.Compile();
        }

        public void CompileSimpleStatement(SimpleStatement simpleStatement)
        {
            var compiler = new SimpleStatementCompiler(this, simpleStatement);
            compiler.Compile();
        }

        public void CompileSwitchDefaultStatement(SwitchDefaultStatement switchDefaultStatement)
        {
            var compiler = new SwitchDefaultStatementCompiler(this, switchDefaultStatement);
            compiler.Compile();
        }

        public void CompileVariableDeclaration(VariableDeclaration variableDeclaration)
        {
            var compiler = new VariableDeclarationCompiler(this, variableDeclaration);
            compiler.Compile();
        }

        public void CompileSynchronizedStatement(SynchronizedStatement synchronizedStatement)
        {
            AddWarning(
                synchronizedStatement.LockObject.First().Line,
                synchronizedStatement.LockObject.First().Column,
                "Synchronized Statement unsupported by TypeScript.");
        }

        public void CompileThrowStatement(ThrowStatement throwStatement)
        {
            var compiler = new ThrowStatementCompiler(this, throwStatement);
            compiler.Compile();
        }

        public void CompileClassType(ClassType classType)
        {
            var compiler = new ClassTypeCompiler(this, classType);
            compiler.Compile();
        }

        public void CompileStaticInitializerDeclaration(StaticInitializerDeclaration staticInitializerDeclaration)
        {
            var compiler = new StaticInitializerDeclarationCompiler(this, staticInitializerDeclaration);
            compiler.Compile();
        }

        public string GetInnerExpressionString(IList<IAstNode> condition)
        {
            // TODO: The output formatting here is likely not very neat, nevertheless it should work.
            return condition
                .Select(x => GetExpressionString(x))
                .Aggregate((x, y) => x + " " + y);
        }

        public string GetValueString(IList<IInputElement> inputElements)
        {
            var returnString = new StringBuilder();

            foreach (var inputElement in inputElements)
            {
                if (inputElement is OperatorToken)
                {
                    var operatorToken = inputElement as OperatorToken;
                    returnString.Append(operatorToken.Data);
                    continue;
                }

                if (inputElement is IdentifierToken)
                {
                    var identifierToken = inputElement as IdentifierToken;
                    returnString.Append(identifierToken.Data);
                    continue;
                }
                
                if (inputElement is BooleanLiteral)
                {
                    var booleanLiteral = inputElement as BooleanLiteral;
                    returnString.Append(booleanLiteral.Data.ToLower());
                    continue;
                }

                if (inputElement is FloatingPointLiteral)
                {
                    var floatingPointLiteral = inputElement as FloatingPointLiteral;
                    returnString.Append(floatingPointLiteral.Data.Substring(0, floatingPointLiteral.Data.Length - 1));
                    continue;
                }

                if (inputElement is CharacterLiteral)
                {
                    throw new NotImplementedException();
                }

                if (inputElement is IntegerLiteral)
                {
                    var integerLiteral = inputElement as IntegerLiteral;
                    returnString.Append(integerLiteral.Data);
                    continue;
                }

                if (inputElement is NullLiteral)
                {
                    var nullLiteral = inputElement as NullLiteral;
                    returnString.Append("null");
                    continue;
                }

                if (inputElement is StringLiteral)
                {
                    var stringLiteral = inputElement as StringLiteral;
                    returnString.Append(stringLiteral);
                    continue;
                }

                if (inputElement is SeperatorToken)
                {
                    var seperatorToken = inputElement as SeperatorToken;
                    returnString.Append(seperatorToken.Data);
                    continue;
                }

                if (inputElement.Data == Keywords.This)
                {
                    returnString.Append(Keywords.This);
                    continue;
                }

                throw new Exception(string.Format("Unknown value type: {0}", inputElement.GetType().Name));
            }
            
            return returnString.ToString();
        }

        public string GetTypeString(IInputElement inputElement)
        {
            if (PrimitiveMapper.IsPrimitive(inputElement.Data))
            {
                return PrimitiveMapper.Map(inputElement.Data);
            }
            
            return inputElement.Data;
        }

        public string GetSimpleStatementString(SimpleStatement simpleStatement)
        {
            // TODO: Output formatting here is likely not very neat looking
            return simpleStatement.Expressions
                .Select(x => GetExpressionString(x))
                .Aggregate((x, y) => x + " " + y);
        }

        public void CompileBody(IList<IAstNode> bodyStatements)
        {
            foreach (var bodyStatement in bodyStatements)
            {
                if (bodyStatement is VariableAssignment)
                {
                    CompileVariableAssignment(bodyStatement as VariableAssignment);
                    continue;
                }

                if (bodyStatement is DoWhileLoop)
                {
                    AddToContextStack(bodyStatement);
                    CompileDoWhileLoop(bodyStatement as DoWhileLoop);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is StaticInitializerDeclaration)
                {
                    AddToContextStack(bodyStatement);
                    CompileStaticInitializerDeclaration(bodyStatement as StaticInitializerDeclaration);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is ForLoop)
                {
                    AddToContextStack(bodyStatement);
                    CompileForLoop(bodyStatement as ForLoop);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is WhileLoop)
                {
                    AddToContextStack(bodyStatement);
                    CompileWhileLoop(bodyStatement as WhileLoop);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is CatchStatement)
                {
                    AddToContextStack(bodyStatement);
                    CompileCatchStatement(bodyStatement as CatchStatement);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is IfElseStatement)
                {
                    AddToContextStack(bodyStatement);
                    CompileIfElseStatement(bodyStatement as IfElseStatement);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is SwitchStatement)
                {
                    AddToContextStack(bodyStatement);
                    CompileSwitchStatement(bodyStatement as SwitchStatement);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is TryStatement)
                {
                    AddToContextStack(bodyStatement);
                    CompileTryStatement(bodyStatement as TryStatement);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is ClassInitializerDeclaration)
                {
                    AddToContextStack(bodyStatement);
                    CompileClassInitializerDeclaration(bodyStatement as ClassInitializerDeclaration);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is ImportDeclaration)
                {
                    CompileImportDeclaration(bodyStatement as ImportDeclaration);
                    continue;
                }

                if (bodyStatement is MethodDeclaration)
                {
                    AddToContextStack(bodyStatement);
                    CompileMethodDeclaration(bodyStatement as MethodDeclaration);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is PackageDeclaration)
                {
                    CompilePackageDeclaration(bodyStatement as PackageDeclaration);
                    continue;
                }

                if (bodyStatement is AssertStatement)
                {
                    CompileAssertStatement(bodyStatement as AssertStatement);
                    continue;
                }

                if (bodyStatement is CaseStatement)
                {
                    AddToContextStack(bodyStatement);
                    CompileCaseStatement(bodyStatement as CaseStatement);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is JumpStatement)
                {
                    CompileJumpStatement(bodyStatement as JumpStatement);
                    continue;
                }

                if (bodyStatement is LabelStatement)
                {
                    CompileLabelStatement(bodyStatement as LabelStatement);
                    continue;
                }

                if (bodyStatement is ReturnStatement)
                {
                    CompileReturnStatement(bodyStatement as ReturnStatement);
                    continue;
                }

                if (bodyStatement is SwitchDefaultStatement)
                {
                    AddToContextStack(bodyStatement);
                    CompileSwitchDefaultStatement(bodyStatement as SwitchDefaultStatement);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is SynchronizedStatement)
                {
                    CompileSynchronizedStatement(bodyStatement as SynchronizedStatement);
                    continue;
                }

                if (bodyStatement is ThrowStatement)
                {
                    CompileThrowStatement(bodyStatement as ThrowStatement);
                    continue;
                }

                if (bodyStatement is SimpleStatement)
                {
                    CompileSimpleStatement(bodyStatement as SimpleStatement);
                    continue;
                }

                if (bodyStatement is ClassType)
                {
                    AddToContextStack(bodyStatement);
                    CompileClassType(bodyStatement as ClassType);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is VariableDeclaration)
                {
                    CompileVariableDeclaration(bodyStatement as VariableDeclaration);
                    continue;
                }

                throw new Exception(string.Format("Unknown statement type: {0}", bodyStatement.GetType().Name));
            }

        }
        public string GetExpressionString(IAstNode expression)
        {
            if (expression is BracketedExpression)
            {
                return GetBracketedExpressionString(expression as BracketedExpression);
            }

            if (expression is IdentifierExpression)
            {
                return GetIdentifierExpressionString(expression as IdentifierExpression);
            }

            if (expression is TypeCastExpression)
            {
                return GetTypeCastExpressionString(expression as TypeCastExpression);
            }

            if (expression is MethodCallExpression)
            {
                return GetMethodCallExpressionString(expression as MethodCallExpression);
            }

            if (expression is ClassInstantiationExpression)
            {
                return GetClassInstantiationExpressionString(expression as ClassInstantiationExpression);
            }

            if (expression is ArrayInitializationExpression)
            {
                return GetArrayInitializationExpressionString(expression as ArrayInitializationExpression);
            }

            if (expression is ParameterExpression)
            {
                return GetParameterExpressionString(expression as ParameterExpression);
            }

            throw new Exception("Unknown expression type.");
        }

        public string GetBracketedExpressionString(BracketedExpression bracketedExpression)
        {
            var compiler = new BracketedExpressionCompiler(this, bracketedExpression);
            return compiler.GetBracketedExpressionString();
        }

        public string GetIdentifierExpressionString(IdentifierExpression identifierExpression)
        {
            var compiler = new IdentifierExpressionCompiler(this, identifierExpression);
            return compiler.GetIdentifierExpressionString();
        }

        public string GetTypeCastExpressionString(TypeCastExpression typeCastExpression)
        {
            var compiler = new TypeCastExpressionCompiler(this, typeCastExpression);
            return compiler.GetTypeCastExpressionString();
        }

        public string GetMethodCallExpressionString(MethodCallExpression methodCallExpression)
        {
            var compiler = new MethodCallExpressionCompiler(this, methodCallExpression);
            return compiler.GetMethodCallExpressionString();
        }

        public string GetClassInstantiationExpressionString(ClassInstantiationExpression classInstantiationExpression)
        {
            var compiler = new ClassInstantiationExpressionCompiler(this, classInstantiationExpression);
            return compiler.GetClassInstantiationExpressionString();
        }

        public string GetArrayInitializationExpressionString(ArrayInitializationExpression arrayInitialization)
        {
            var arrayContents = arrayInitialization.Contents.Count == 0
                ? string.Empty
                : arrayInitialization.Contents
                    .Select(x => GetExpressionString(x))
                    .Aggregate((x, y) => x + ", " + y);

            return string.Format("[{0}]", arrayContents);
        }

        public string GetParameterExpressionString(ParameterExpression parameterExpression)
        {
            var returnString = parameterExpression.AstNodes.Count == 0
                ? string.Empty
                : parameterExpression.AstNodes
                    .Select(x => GetExpressionString(x))
                    .Aggregate((x, y) => x + " " + y);

            return returnString;
        }
    }
}
