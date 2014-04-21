using Mordritch.Transpiler.Compilers.TypeScript;
using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Assignments;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Loops;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Statements;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Expressions;
using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using Mordritch.Transpiler.src.Compilers;
using Mordritch.Transpiler.src.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers
{
    public interface ICompiler
    {
        void PushToContextStack(IAstNode astNode);

        void PopFromContextStack();

        IAstNode GetCurrentContextFromStack();

        IAstNode GetPreviousContextFromStack(int depth);

        TAstNode GetPreviousContextFromStack<TAstNode>() where TAstNode : IAstNode;
        
        void AddBlankLine();

        void AddLine(string line);

        void AddWarning(int line, int column, string description);

        void IncreaseIndentation();

        void DecreaseIndentation();

        void CompileVariableAssignment(VariableAssignment variableAssignment);

        void CompileDoWhileLoop(DoWhileLoop doWhileLoop);

        void CompileForLoop(ForLoop forLoop);

        void CompileWhileLoop(WhileLoop whileLoop);

        void CompileCatchStatement(CatchStatement catchStatement);

        void CompileIfElseStatement(IfElseStatement ifElseStatement);

        void CompileSwitchStatement(SwitchStatement switchStatement);

        void CompileTryStatement(TryStatement tryStatement);

        void CompileClassInitializerDeclaration(ClassInitializerDeclaration classInitializerDeclaration);

        void CompileImportDeclaration(ImportDeclaration importDeclaration);

        void CompileMethodDeclaration(MethodDeclaration methodDeclaration);

        void CompilePackageDeclaration(PackageDeclaration packageDeclaration);

        void CompileAssertStatement(AssertStatement assertStatement);

        void CompileCaseStatement(CaseStatement caseStatement);

        void CompileJumpStatement(JumpStatement jumpStatement);

        void CompileLabelStatement(LabelStatement labelStatement);

        void CompileReturnStatement(ReturnStatement returnStatement);

        void CompileSwitchDefaultStatement(SwitchDefaultStatement switchDefaultStatement);

        void CompileSynchronizedStatement(SynchronizedStatement synchronizedStatement);

        void CompileThrowStatement(ThrowStatement throwStatement);

        void CompileClassType(ClassType classType);

        void CompileSimpleStatement(SimpleStatement simpleStatement);

        void CompileVariableDeclaration(VariableDeclaration variableDeclaration);

        void CompileStaticInitializerDeclaration(StaticInitializerDeclaration staticInitializerDeclaration);

        void CompileBody(IList<IAstNode> bodyStatements);

        string GetMethodArgumentString(MethodArgument methodArgument);

        string GetBracketedExpressionString(BracketedExpression bracketedExpression);

        string GetIdentifierExpressionString(IdentifierExpression identifierExpression, IList<InnerExpressionProcessingListItem> list = null);

        string GetMethodCallExpressionString(MethodCallExpression methodCallExpression, IList<InnerExpressionProcessingListItem> list = null);

        string GetTypeCastExpressionString(TypeCastExpression typeCastExpression);

        string GetInnerExpressionString(IList<IAstNode> condition);

        string GetExpressionString(IAstNode expression, IList<InnerExpressionProcessingListItem> list = null);

        string GetArrayInitializationExpressionString(ArrayInitializationExpression arrayInitialization);

        string GetParameterExpressionString(ParameterExpression parameterExpression);

        string GetSimpleStatementString(SimpleStatement simpleStatement);

        string GetTypeString(IInputElement inputElement, string contextDescription);

        string GetValueString(IList<IInputElement> inputElements);

        void CompileStaticInitializerDeclarationDefinition(StaticInitializerDeclaration staticInitializerDeclaration);

        void CompileClassInitializerDeclarationDefinition(ClassInitializerDeclaration classInitializerDeclaration);

        void CompileMethodDeclarationDefinition(MethodDeclaration methodDeclaration);

        void CompilePackageDeclarationDefinition(PackageDeclaration packageDeclaration);

        void CompileSynchronizedStatementDefinition(SynchronizedStatement synchronizedStatement);

        void CompileClassTypeDefinition(ClassType classType);

        void CompileVariableDeclarationDefinition(VariableDeclaration variableDeclaration);

        void CompileImportDeclarationDefinition(ImportDeclaration importDeclaration);

        void CompileDefinition(IList<IAstNode> bodyStatements);

        int GetContextStackSize();

        IList<IAstNode> GetFullContextStack();

        string GetScopeClarifier(string identifierName, IAstNode previousExpression);

        string GetScopeClarifier(string identifierName, string previousExpression);
        
        void BeginCommentingOut();

        void EndCommentingOut();

        IList<string> GetClassInheritanceStack(string className);

        IList<InnerExpressionProcessingListItem> ProcessToInnerExpressionItemList(IEnumerable<IAstNode> astNodeList);
    }
}
