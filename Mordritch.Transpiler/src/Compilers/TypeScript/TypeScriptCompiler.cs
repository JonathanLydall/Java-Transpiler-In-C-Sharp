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

using Mordritch.Transpiler.Java.Common;

using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes;

using Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers;
using Mordritch.Transpiler.Compilers.TypeScript;
using Mordritch.Transpiler.src.Compilers.TypeScript;

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

        private static IDictionary<string, IList<IAstNode>> _sourceFiles;

        public IList<IAstNode> ParsedFile { get; set; }

        public TypeScriptCompiler(IList<IAstNode> parsedFile)
        {
            ParsedFile = parsedFile;
        }

        public void AddJavaDotLangImports()
        {
            var usedTypes = new List<string>();
            foreach (var astNode in ParsedFile)
            {
                usedTypes = usedTypes.Union(astNode.GetUsedTypes()).ToList();
            }

            var javaLangTypes = usedTypes.Where(x => JavaLangPackages.Contains(x));
            foreach (var usedType in javaLangTypes)
            {
                Console.WriteLine("    Adding usedType {0}", usedType);
                AddLine(string.Format("import {0} = java.lang.{0};", usedType));
            }
        }

        public static string Compile(IDictionary<string, IList<IAstNode>> sourceFiles, string file)
        {
            _sourceFiles = sourceFiles;

            var parsedFile = _sourceFiles[file];
            var packageDeclarationContent = GetPackageDeclararion(parsedFile);
            var instance = new TypeScriptCompiler(parsedFile);

            if (packageDeclarationContent != null)
            {
                instance.AddLine(string.Format("module Mordritch.{0} {{", packageDeclarationContent));
                instance.IncreaseIndentation();
            }

            instance.AddJavaDotLangImports();
            instance.CompileBody(parsedFile);

            if (packageDeclarationContent != null)
            {
                instance.DecreaseIndentation();
                instance.AddLine("}");
            }

            return instance.GetOutput();
        }

        public static string GenerateDefinition(IDictionary<string, IList<IAstNode>> sourceFiles, string file)
        {
            var parsedFile = _sourceFiles[file];
            var packageDeclarationContent = GetPackageDeclararion(parsedFile);
            var instance = new TypeScriptCompiler(parsedFile);

            if (packageDeclarationContent != null)
            {
                instance.AddLine(string.Format("module Mordritch.{0} {{", packageDeclarationContent));
                instance.IncreaseIndentation();
            }

            instance.CompileDefinition(_sourceFiles[file]);

            if (packageDeclarationContent != null)
            {
                instance.DecreaseIndentation();
                instance.AddLine("}");
            }

            return instance.GetOutput();
        }

        private static string GetPackageDeclararion(IList<IAstNode> parsedFile)
        {
            var packageDeclaration = parsedFile.FirstOrDefault(x => x is PackageDeclaration) as PackageDeclaration;
            return
                packageDeclaration == null ||
                packageDeclaration.Content == null ||
                packageDeclaration.Content.Count == 0
                    ? null
                    : packageDeclaration.Content
                        .Select(x => x.Data)
                        .Aggregate((x, y) => x + y);
        }

        public IList<IAstNode> GetFullContextStack()
        {
            return _contextStack;
        }
        
        public int GetContextStackSize()
        {
            return _contextStack.Count;
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

        public void CompileClassInitializerDeclarationDefinition(ClassInitializerDeclaration classInitializerDeclaration)
        {
            var compiler = new ClassInitializerDeclarationCompiler(this, classInitializerDeclaration);
            compiler.GenerateDefinition();
        }

        public void CompileImportDeclaration(ImportDeclaration importDeclaration)
        {
            var compiler = new ImportDeclarationCompiler(this, importDeclaration);
            compiler.Compile();
        }

        public void CompileImportDeclarationDefinition(ImportDeclaration importDeclaration)
        {
            var compiler = new ImportDeclarationCompiler(this, importDeclaration);
            compiler.GenerateDefinition();
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

        public void CompileMethodDeclarationDefinition(MethodDeclaration methodDeclaration)
        {
            var compiler = new MethodDeclarationCompiler(this, methodDeclaration);
            compiler.GenerateDefinition();
        }

        public void CompilePackageDeclaration(PackageDeclaration packageDeclaration)
        {
            var compiler = new PackageDeclarationCompiler(this, packageDeclaration);
            compiler.Compile();
        }

        public void CompilePackageDeclarationDefinition(PackageDeclaration packageDeclaration)
        {
            var compiler = new PackageDeclarationCompiler(this, packageDeclaration);
            compiler.GenerateDefinition();
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

        public void CompileVariableDeclarationDefinition(VariableDeclaration variableDeclaration)
        {
            var compiler = new VariableDeclarationCompiler(this, variableDeclaration);
            compiler.GenerateDefinition();
        }

        public void CompileSynchronizedStatement(SynchronizedStatement synchronizedStatement)
        {
            AddWarning(
                synchronizedStatement.LockObject.First().Line,
                synchronizedStatement.LockObject.First().Column,
                "Synchronized Statement unsupported by TypeScript.");
        }

        public void CompileSynchronizedStatementDefinition(SynchronizedStatement synchronizedStatement)
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

        public void CompileClassTypeDefinition(ClassType classType)
        {
            var compiler = new ClassTypeCompiler(this, classType);
            compiler.GenerateDefinition();
        }

        public void CompileStaticInitializerDeclaration(StaticInitializerDeclaration staticInitializerDeclaration)
        {
            var compiler = new StaticInitializerDeclarationCompiler(this, staticInitializerDeclaration);
            compiler.Compile();
        }

        public void CompileStaticInitializerDeclarationDefinition(StaticInitializerDeclaration staticInitializerDeclaration)
        {
            var compiler = new StaticInitializerDeclarationCompiler(this, staticInitializerDeclaration);
            compiler.GenerateDefinition();
        }

        public string GetInnerExpressionString(IList<IAstNode> expressionList)
        {
            // TODO: The output formatting here is likely not very neat, nevertheless it should work.
            
            IAstNode previousExpression = null;
            var returnList = new List<string>();
            foreach (var astNode in expressionList) {
                returnList.Add(GetExpressionString(astNode, previousExpression));
                previousExpression = astNode;
            }
            
            return returnList
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

        public string GetTypeString(IInputElement inputElement, string contextDescription)
        {
            if (PrimitiveMapper.IsPrimitive(inputElement.Data))
            {
                return PrimitiveMapper.Map(inputElement.Data);
            }
            else if (inputElement.Data == "Map")
            {
                //TODO: Put this somewhere else
                return "java.util.Map";
            }
            else if (inputElement.Data == "Set")
            {
                //TODO: Put this somewhere else
                return "java.util.Set";
            }

            OtherTypes.AddToList(inputElement, contextDescription);
            return inputElement.Data;
        }

        public string GetSimpleStatementString(SimpleStatement simpleStatement)
        {
            // TODO: Output formatting here is likely not very neat looking
            return simpleStatement.Expressions
                .Select(x => GetExpressionString(x))
                .Aggregate((x, y) => x + " " + y);
        }

        public void CompileDefinition(IList<IAstNode> bodyStatements)
        {
            foreach (var bodyStatement in bodyStatements)
            {
                if (bodyStatement is ImportDeclaration)
                {
                    CompileImportDeclarationDefinition(bodyStatement as ImportDeclaration);
                    continue;
                }

                if (bodyStatement is StaticInitializerDeclaration)
                {
                    AddToContextStack(bodyStatement);
                    CompileStaticInitializerDeclarationDefinition(bodyStatement as StaticInitializerDeclaration);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is ClassInitializerDeclaration)
                {
                    AddToContextStack(bodyStatement);
                    CompileClassInitializerDeclarationDefinition(bodyStatement as ClassInitializerDeclaration);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is MethodDeclaration)
                {
                    AddToContextStack(bodyStatement);
                    CompileMethodDeclarationDefinition(bodyStatement as MethodDeclaration);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is PackageDeclaration)
                {
                    CompilePackageDeclarationDefinition(bodyStatement as PackageDeclaration);
                    continue;
                }

                if (bodyStatement is SynchronizedStatement)
                {
                    CompileSynchronizedStatementDefinition(bodyStatement as SynchronizedStatement);
                    continue;
                }

                if (bodyStatement is ClassType)
                {
                    AddToContextStack(bodyStatement);
                    CompileClassTypeDefinition(bodyStatement as ClassType);
                    RemoveFromContextStack();
                    continue;
                }

                if (bodyStatement is VariableDeclaration)
                {
                    CompileVariableDeclarationDefinition(bodyStatement as VariableDeclaration);
                    continue;
                }

                throw new Exception(string.Format("Unknown statement type: {0}", bodyStatement.GetType().Name));
            }
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
        public string GetExpressionString(IAstNode expression, IAstNode previousExpression = null)
        {
            if (expression is BracketedExpression)
            {
                return GetBracketedExpressionString(expression as BracketedExpression);
            }

            if (expression is IdentifierExpression)
            {
                return GetIdentifierExpressionString(expression as IdentifierExpression, previousExpression);
            }

            if (expression is TypeCastExpression)
            {
                return GetTypeCastExpressionString(expression as TypeCastExpression);
            }

            if (expression is MethodCallExpression)
            {
                return GetMethodCallExpressionString(expression as MethodCallExpression, previousExpression);
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
            return string.Format("({0})", GetInnerExpressionString(bracketedExpression.InnerExpressions));
        }

        public string GetIdentifierExpressionString(IdentifierExpression identifierExpression, IAstNode previousExpression = null)
        {
            var compiler = new IdentifierExpressionCompiler(this, identifierExpression, previousExpression);
            return compiler.GetIdentifierExpressionString();
        }

        public string GetTypeCastExpressionString(TypeCastExpression typeCastExpression)
        {
            var compiler = new TypeCastExpressionCompiler(this, typeCastExpression);
            return compiler.GetTypeCastExpressionString();
        }

        public string GetMethodCallExpressionString(MethodCallExpression methodCallExpression, IAstNode previousExpression)
        {
            var compiler = new MethodCallExpressionCompiler(this, methodCallExpression, previousExpression);
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

        //private struct CachedScopeClarifierIdentifier
        //{
        //    public ClassType ClassTypeInstance { get; set; }

        //    public string IdentifierName { get; set; }

        //    public string ScopeClarifier { get; set; }
        //}

        //private static IList<CachedScopeClarifierIdentifier> cachedScopeClarifierIdentifiers = new List<CachedScopeClarifierIdentifier>();

        private static IDictionary<string, string> cachedScopeClarifierIdentifiers = new Dictionary<string, string>();

        public string GetScopeClarifier(string identifierName, IAstNode previousExpression)
        {
            if (previousExpression is IdentifierExpression &&
                (previousExpression as IdentifierExpression).Token is SeperatorToken &&
                ((previousExpression as IdentifierExpression).Token as SeperatorToken).Data == ".")
            {
                return string.Empty;
            }

            return GetScopeClarifier(identifierName);
        }

        public string GetScopeClarifier(string identifierName, string previousExpression)
        {
            if (previousExpression == ".")
            {
                return string.Empty;
            }

            return GetScopeClarifier(identifierName);
        }

        private string GetScopeClarifier(string identifierName)
        {
            var stack = GetFullContextStack();
            var classTypeItem = stack.LastOrDefault(x => x is ClassType);

            if (classTypeItem == null)
            {
                return string.Empty;
            }

            var classType = classTypeItem as ClassType;
            var keyName = classType.Name + "." + identifierName;

            if (cachedScopeClarifierIdentifiers.ContainsKey(keyName))
            {
                return cachedScopeClarifierIdentifiers[keyName];
            }

            var staticMembers = classType.Body
                .Where(x => x is VariableDeclaration && ((VariableDeclaration)x).Modifiers.Any(y => y.Data == Keywords.Static))
                .Select(x => ((VariableDeclaration)x).VariableName.Data)
                .ToList();

            var members = classType.Body
                .Where(x => x is VariableDeclaration && ((VariableDeclaration)x).Modifiers.All(y => y.Data != Keywords.Static))
                .Select(x => ((VariableDeclaration)x).VariableName.Data)
                .ToList();

            staticMembers = staticMembers.Union(classType.Body
                    .Where(x => x is MethodDeclaration && ((MethodDeclaration)x).Modifiers.Any(y => y.Data == Keywords.Static))
                    .Select(x => ((MethodDeclaration)x).Name.Data)
                    .ToList())
                .ToList();

            members = members.Union(classType.Body
                    .Where(x => x is MethodDeclaration && ((MethodDeclaration)x).Modifiers.Any(y => y.Data != Keywords.Static))
                    .Select(x => ((MethodDeclaration)x).Name.Data)
                    .ToList())
                .ToList();

            if (staticMembers.Any(x => x == identifierName))
            {
                cachedScopeClarifierIdentifiers.Add(keyName, string.Format("{0}.", classType.Name));
                return string.Format("{0}.", classType.Name);
            }

            var parentClassName = classType.Extends;
            while (parentClassName != null && _sourceFiles.ContainsKey(parentClassName))
            {
                var parentClass = _sourceFiles[parentClassName].First(x => x is ClassType) as ClassType;

                staticMembers = staticMembers.Union(parentClass.Body
                        .Where(x => x is VariableDeclaration && ((VariableDeclaration)x).Modifiers.Any(y => y.Data == Keywords.Static))
                        .Select(x => ((VariableDeclaration)x).VariableName.Data)
                        .ToList())
                    .ToList();

                staticMembers = staticMembers.Union(parentClass.Body
                        .Where(x => x is MethodDeclaration && ((MethodDeclaration)x).Modifiers.Any(y => y.Data == Keywords.Static))
                        .Select(x => ((MethodDeclaration)x).Name.Data)
                        .ToList())
                    .ToList();

                members = members.Union(parentClass.Body
                        .Where(x => x is VariableDeclaration && ((VariableDeclaration)x).Modifiers.All(y => y.Data != Keywords.Static))
                        .Select(x => ((VariableDeclaration)x).VariableName.Data)
                        .ToList())
                    .ToList();

                members = members.Union(parentClass.Body
                        .Where(x => x is MethodDeclaration && ((MethodDeclaration)x).Modifiers.Any(y => y.Data != Keywords.Static))
                        .Select(x => ((MethodDeclaration)x).Name.Data)
                        .ToList())
                    .ToList();

                if (staticMembers.Any(x => x == identifierName))
                {
                    cachedScopeClarifierIdentifiers.Add(keyName, string.Format("{0}.", parentClass.Name));
                    return string.Format("{0}.", parentClass.Name);
                }

                parentClassName = parentClass.Extends;
            }

            if (staticMembers.Any(x => x == identifierName))
            {
                cachedScopeClarifierIdentifiers.Add(keyName, "this.");
                return "this.";
            }

            cachedScopeClarifierIdentifiers.Add(keyName, string.Empty);
            return string.Empty;
        }
    }
}
