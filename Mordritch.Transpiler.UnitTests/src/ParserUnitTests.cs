using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

using Mordritch.Transpiler.Java.Tokenizer;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.AstGenerator.Parsers;
using Mordritch.Transpiler.Java.AstGenerator.Statements;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes;
using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.AstGenerator.Assignments;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Loops;
using Mordritch.Transpiler.Java.AstGenerator.Types;
using Mordritch.Transpiler.Java.AstGenerator.ControlStructures.Statements;

namespace Mordritch.Transpiler.UnitTests
{
    [TestClass]
    public class ParserUnitTests
    {
        private TReturnType GetParsedResult<TParser, TReturnType>(string input) where TParser : IParser, new()
        {
            var tokenizer = new Tokenizer(input, "Test Data");
            return (TReturnType)ParserHelper.Parse<TParser>(tokenizer.GetInputElements());
        }

        private IList<IAstNode> GetParsedResultFromFile(string fileName)
        {
            var tokenizer = new Tokenizer(fileName);
            var astGenerator = new AstGenerator();
            return astGenerator.Parse(tokenizer.GetInputElements(), fileName);
        }

        private IList<IAstNode> GetParsedBody<TParser>(string input) where TParser : Parser, new()
        {
            var tokenizer = new Tokenizer("{ " + input + " }", "Test Data");
            var parser = new TParser();
            var inputElements = tokenizer.GetInputElements();
            parser.DataSource = new InputElementDataSource(inputElements);

            return parser.ParseBody();
        }

        [TestMethod]
        public void AssertStatementParser()
        {
            int number;
            var results = GetParsedResult<AssertStatementParser, AssertStatement>(
@"assert n != 0 : ""n was equal to zero"";
"
            );

            number = 0;
            Assert.IsInstanceOfType(results.Condition[number], typeof(IdentifierToken));
            Assert.IsTrue(results.Condition[number].Data == "n");

            number++;
            Assert.IsInstanceOfType(results.Condition[number], typeof(WhiteSpaceInputElement));

            number++;
            Assert.IsInstanceOfType(results.Condition[number], typeof(OperatorToken));
            Assert.IsTrue(results.Condition[number].Data == "!=");

            number++;
            Assert.IsInstanceOfType(results.Condition[number], typeof(WhiteSpaceInputElement));

            number++;
            Assert.IsInstanceOfType(results.Condition[number], typeof(IntegerLiteral));
            Assert.IsTrue(results.Condition[number].Data == "0");

            number = 0;
            Assert.IsInstanceOfType(results.Message[number], typeof(StringLiteral));
            Assert.IsTrue(results.Message[number].Data == "\"n was equal to zero\"");
            number++;
        }

        [TestMethod]
        public void DeclarationParser()
        {
            VariableDeclaration result;

            result = GetParsedResult<VariableDeclarationParser, VariableDeclaration>(@"float var3 = 0.5F;");
            Assert.IsTrue(result.HasInitialization);
            Assert.AreEqual(result.ArrayCount, 0);
            Assert.AreEqual(result.Modifiers.Count, 0);
            Assert.AreEqual(result.AssignedValue.Count, 1);

            Assert.IsInstanceOfType(result.VariableType, typeof(KeywordToken));
            Assert.AreEqual(result.VariableType.Data, "float");

            Assert.IsInstanceOfType(result.VariableName, typeof(IdentifierToken));
            Assert.AreEqual(result.VariableName.Data, "var3");


            result = GetParsedResult<VariableDeclarationParser, VariableDeclaration>(@"int var6 = this.getFullMetadata(par1IBlockAccess, par2, par3, par4);");
            result = GetParsedResult<VariableDeclarationParser, VariableDeclaration>(@"int var6 = (float)(this.getFullMetadata((double)((float)par2 + 0.5F), par2, par3, par4));");
            result = GetParsedResult<VariableDeclarationParser, VariableDeclaration>(@"int var7 = this.blockIndexInTexture;");
            result = GetParsedResult<VariableDeclarationParser, VariableDeclaration>(@"boolean var9 = (var6 & 4) != 0;");
            result = GetParsedResult<VariableDeclarationParser, VariableDeclaration>(@"int var8 = var6 & 3;");
            result = GetParsedResult<VariableDeclarationParser, VariableDeclaration>(@"int var1 = (double)((float)par2 + 0.5F);");

            result = GetParsedResult<VariableDeclarationParser, VariableDeclaration>(@"EntityItem var13 = new EntityItem(par1World, (double)par2 + var7, (double)par3 + var9, (double)par4 + var11, par5ItemStack);");


            string testData;
            testData =
                @"EntityItem var13 = new EntityItem(par1World, (double)par2 + var7, (double)par3 + var9, (double)par4 + var11, par5ItemStack);";
            var newResult = GetParsedBody<VariableDeclarationParser>(testData);

        }

        [TestMethod]
        public void VariableAssignmentParser()
        {
            VariableAssignment result;

            result = GetParsedResult<VariableAssignmentParser, VariableAssignment>(@"tricksySlashAtEnd = ""hello.\\"";");
            Assert.IsInstanceOfType(result.VariableName[0], typeof(IdentifierToken));
            Assert.IsNull(result.PreComment);
            Assert.IsNull(result.PostComment);
            Assert.AreEqual(result.AssignedValue.Count, 1);

            result = GetParsedResult<VariableAssignmentParser, VariableAssignment>(@"++this.blockIndexInTexture;");
            result = GetParsedResult<VariableAssignmentParser, VariableAssignment>(@"this.blockIndexInTexture++;");
            result = GetParsedResult<VariableAssignmentParser, VariableAssignment>(@"var1 = (double)((float)par2 + 0.5F);");
        }

        [TestMethod]
        public void ChainedMethodCallsInBodyTest()
        {
            SimpleStatement result;
            string testData;
            string debugOut;

            testData = @"par5EntityPlayer.getFoodStats().addStats(2, 0.1F);";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);
        }

        [TestMethod]
        public void FunctionCallParserTest()
        {
            SimpleStatement result;
            string testData;
            string debugOut;

            testData = @"this.setBlockBounds(0.5F - var3, 0.0F, 0.5F - var3, 0.5F + var3, var4, 0.5F + var3);";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);
        }

        [TestMethod]
        public void SuperCallTest()
        {
            SimpleStatement result;
            string testData;
            string debugOut;

            testData = @"super(par1);";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);
        }

        [TestMethod]
        public void NestedFunctionCallParserTest()
        {
            SimpleStatement result;
            string testData;
            string debugOut;

            testData = @"this.setDoorRotation(this.getFullMetadata(par1IBlockAccess, par2, par3, par4));";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);

            testData = @"this.dropBlockAsItem_do(par1World, par2, par3, par4, ItemStack(var10, 1, this.damageDropped(par5)));";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);
        }

        [TestMethod]
        public void CastFunctionCallTest()
        {
            SimpleStatement result;
            string debugOut;

            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(@"((EntityPlayerMP)par5EntityPlayer).sendContainerToPlayer(par5EntityPlayer.inventoryContainer);");
            debugOut = result.DebugOut();

            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(@"((TileEntityBeacon)par1World.getBlockTileEntity(par2, par3, par4)).func_94047_a(par6ItemStack.getDisplayName());");
            debugOut = result.DebugOut();
        }

        [TestMethod]
        public void FunctionCallParserTestWithCastParameter()
        {
            SimpleStatement result;
            string testData;
            string debugOut;

            testData = @"par1World.playAuxSFXAtEntity((EntityPlayer)null, 1003, par2, par3, par4, 0);";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);

            testData = @"par1World.newExplosion((Entity)null, (double)((float)par2 + 0.5F), (double)((float)par3 + 0.5F), (double)((float)par4 + 0.5F), 5.0F, true, true);";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);

            testData = @"this.dropBlockAsItem_do(par1World, par2, par3, par4, new ItemStack(var10, 1, this.damageDropped(par5)));";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);

            testData = @"par1World.spawnEntityInWorld(new EntityXPOrb(par1World, (double)par2 + 0.5D, (double)par3 + 0.5D, (double)par4 + 0.5D, var6));";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);
            //Assert.AreEqual(3, result.Method.Count);
            //Assert.AreEqual(1, result.Parameters.Count);

            testData = @"par1World.newExplosion((Entity)null, (double)((float)par2 + 0.5F), (double)((float)par3 + 0.5F), (double)((float)par4 + 0.5F), 5.0F, true, true);";
            result = GetParsedResult<SimpleStatementParser, SimpleStatement>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual(testData, debugOut);
            //Assert.AreEqual(7, result.Parameters.Count);
        }


        [TestMethod]
        public void MethodDeclarationTest()
        {
            MethodDeclaration result;

            result = GetParsedResult<MethodDeclarationParser, MethodDeclaration>(@"public boolean isOpaqueCube() {}");
            Assert.AreEqual(0, result.Arguments.Count);
            Assert.AreEqual("boolean", result.ReturnType.Data);
            Assert.AreEqual(0, result.Body.Count);
            Assert.AreEqual("isOpaqueCube", result.Name.Data);
            Assert.AreEqual(1, result.Modifiers.Count);
            Assert.AreEqual("public", result.Modifiers[0].Data);
        }

        [TestMethod]
        public void ForLoopTest()
        {
            var testData =
                @"/* 01 */ for (int var9 = 0; var9 < var8; ++var9)
                  /* 02 */ {
                  /* 03 */     if (par1World.rand.nextFloat() <= par6)
                  /* 04 */     {
                  /* 05 */         int var10 = this.idDropped(par5, par1World.rand, par7);
                  /* 06 */ 
                  /* 07 */         if (var10 > 0)
                  /* 08 */         {
                  /* 09 */             this.dropBlockAsItem_do(par1World, par2, par3, par4, ItemStack(var10, 1, this.damageDropped(par5)));
                  /* 10 */         }
                  /* 11 */     }
                  /* 12 */ }";

            ForLoop result;
            result = GetParsedResult<ForLoopParser, ForLoop>(testData);

            Assert.AreEqual(1, result.Initializers.Count);
            var initializer = result.Initializers[0];
            Assert.AreEqual(1, initializer.AssignedValue.Count);
            //Assert.AreEqual("0", initializer.AssignedValue[0].Data);
            Assert.AreEqual("int", initializer.InitializedType.Data);
            Assert.AreEqual("var9", initializer.VariableName.Data);

            Assert.AreEqual(1, result.CounterExpressions.Count);
            //Assert.AreEqual(2, result.CounterExpressions[0].Count);
            //Assert.AreEqual("++", result.CounterExpressions[0][0].Data);
            //Assert.AreEqual("var9", result.CounterExpressions[0][1].Data);

            Assert.AreEqual(3, result.Condition.Count);
            //Assert.AreEqual("var9", result.Condition[0] );
            //Assert.AreEqual("<", result.Condition[2].Data);
            //Assert.AreEqual("var8", result.Condition[4].Data);

            Assert.AreEqual(1, result.Body.Count);
            Assert.IsInstanceOfType(result.Body[0], typeof(IfElseStatement));
        }

        [TestMethod]
        public void ForLoopTest2()
        {
            ForLoop result;
            string testData;

            testData =
                @"/* 01 */ for (float var12 = this.random.nextFloat() * 0.8F + 0.1F; var9.stackSize > 0; par1World.spawnEntityInWorld(var14))
                  /* 02 */ {
                  /* 03 */ }";

            result = GetParsedResult<ForLoopParser, ForLoop>(testData);

        }

        [TestMethod]
        public void WhileLoopTest()
        {
            var testData =
                @"while (par5 > 0)
                {
                    int var6 = EntityXPOrb.getXPSplit(par5);
                    par5 -= var6;
                }";

            WhileLoop result;
            result = GetParsedResult<WhileLoopParser, WhileLoop>(testData);

            Assert.AreEqual(2, result.Body.Count);
            Assert.IsInstanceOfType(result.Body[0], typeof(VariableDeclaration));
            Assert.IsInstanceOfType(result.Body[1], typeof(VariableAssignment));

            //Assert.AreEqual(5, result.Condition.Count);
            //Assert.AreEqual("par5", result.Condition[0].Data);
            //Assert.IsInstanceOfType(result.Condition[1], typeof(WhiteSpaceInputElement));
            //Assert.AreEqual(">", result.Condition[2].Data);
            //Assert.IsInstanceOfType(result.Condition[3], typeof(WhiteSpaceInputElement));
            //Assert.AreEqual("0", result.Condition[4].Data);
        }

        [TestMethod]
        public void DoWhileLoopTest()
        {
            var testData =
                @"do
                {
                    if (!var4.hasNext())
                    {
                        return false;
                    }

                    EntityOcelot var5 = (EntityOcelot)var4.next();
                    var6 = (EntityOcelot)var5;
                }
                while (!var6.isSitting());";

            DoWhileLoop result;
            result = GetParsedResult<DoWhileLoopParser, DoWhileLoop>(testData);
            Assert.AreEqual("do { ... } while (!var6.isSitting());", result.DebugOut());
        }

        [TestMethod]
        public void IfStatementTest()
        {
            string debugOut;
            string testData;
            IfElseStatement result;

            testData =
                @"if (!var4.hasNext())
                {
                    return false;
                }";

            testData =
                @"if (this.adjacentTreeBlocks == null)
                {
                    this.adjacentTreeBlocks = new int[var9 * var9 * var9];
                }";

            result = GetParsedResult<IfElseStatementParser, IfElseStatement>(testData);
            debugOut = result.DebugOut();

            //Assert.AreEqual("if (!var4.hasNext()) {...", debugOut);
        }

        [TestMethod]
        public void ClassTest()
        {
            string debugOut;
            string testData;
            ClassType result;

            testData =
                @"public class BlockComparator extends BlockRedstoneLogic implements ITileEntityProvider
                {
                }
                ";

            result = GetParsedResult<ClassTypeParser, ClassType>(testData);
            debugOut = result.DebugOut();
            Assert.AreEqual("public class BlockComparator extends BlockRedstoneLogic implements ITileEntityProvider {...", debugOut);
        }

        [TestMethod]
        public void TernaryStatement()
        {
            string debugOut;
            string testData;
            VariableAssignment result;

            testData =
                @"var13 |= var11 ? 8 : 0;";

            result = GetParsedResult<VariableAssignmentParser, VariableAssignment>(testData);
            debugOut = result.DebugOut();
            //Assert.AreEqual(testData, debugOut);
        }

        [TestMethod]
        public void SwitchTest()
        {
            var testData =
                /* 01 */ @"switch (this.GetValueOfVar3())
                /* 02 */ {
                /* 03 */   case 1:
                /* 04 */       return this.blockIndexInTexture + 1;
                /* 05 */ 
                /* 06 */   case 2:
                /* 07 */       return this.blockIndexInTexture + 16 + 1;
                /* 08 */ 
                /* 09 */   default:
                /* 10 */       return this.blockIndexInTexture + 16;
                /* 11 */ }";

            SwitchStatement result;
            result = GetParsedResult<SwitchStatementParser, SwitchStatement>(testData);

            Assert.AreEqual(5, result.ControlStatement.Count);
            //Assert.AreEqual("this", result.ControlStatement[0].Data);
            //Assert.AreEqual(".", result.ControlStatement[1].Data);
            //Assert.AreEqual("GetValueOfVar3", result.ControlStatement[2].Data);
            //Assert.AreEqual("(", result.ControlStatement[3].Data);
            //Assert.AreEqual(")", result.ControlStatement[4].Data);

            Assert.AreEqual(6, result.Body.Count);
            Assert.IsInstanceOfType(result.Body[0], typeof(CaseStatement));
            Assert.IsInstanceOfType(result.Body[1], typeof(ReturnStatement));
            Assert.IsInstanceOfType(result.Body[2], typeof(CaseStatement));
            Assert.IsInstanceOfType(result.Body[3], typeof(ReturnStatement));
            Assert.IsInstanceOfType(result.Body[4], typeof(SwitchDefaultStatement));
            Assert.IsInstanceOfType(result.Body[5], typeof(ReturnStatement));
        }

        [TestMethod]
        public void TryStatementTest()
        {
            string debugOut;
            string testData;
            TryStatement result;

            testData =
                @"try
                {
                    return (TileEntity)this.signEntityClass.newInstance();
                }
                catch (Exception var3)
                {
                    throw new RuntimeException(var3);
                }";

            result = GetParsedResult<TryStatementParser, TryStatement>(testData);
            debugOut = result.DebugOut();
        }

        [TestMethod]
        public void MultidimensionalArraysTest()
        {
            var testData = @"public static final int[][] footBlockToHeadBlockMap = new int[][] {{0, 1}, { -1, 0}, {0, -1}, {1, 0}};";
            var newResult = GetParsedBody<VariableDeclarationParser>(testData);
        }

        [TestMethod]
        public void TryParseBlockFileTest()
        {
            var path = @"D:\Users\Jonathan Lydall\Downloads\mcp\mcp742\src\minecraft_server\net\minecraft\src\";

            var fileList = Directory.GetFiles(path, "Block*.java");

            foreach (var file in fileList)
            {
                GetParsedResultFromFile(file);
            }
        }

        [TestMethod]
        public void ParseVariableDeclarationTest()
        {
            var testData =
                //@"private static final String[] field_94467_a = new String[] {""doorWood_lower"", ""doorWood_upper"", ""doorIron_lower"", ""doorIron_upper""};";
                "public static final Block[] blocksList = new Block[4096];";

            var newResult = GetParsedBody<VariableDeclarationParser>(testData);
        }
    }
}
