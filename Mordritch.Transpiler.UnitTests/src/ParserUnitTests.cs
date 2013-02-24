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

namespace Mordritch.Transpiler.UnitTests
{
    [TestClass]
    public class ParserUnitTests
    {
        private TReturnType GetParsedResult<TParser, TReturnType>(string input) where TParser : IParser, new()
        {
            var tokenizer = new Tokenizer(input, false);
            return (TReturnType)ParserHelper.Parse<TParser>(tokenizer.GetInputElements());
        }

        private IList<IAstNode> GetParsedResultFromFile(string fileName)
        {
            var tokenizer = new Tokenizer(fileName);
            var astGenerator = new AstGenerator();
            return astGenerator.Parse(tokenizer.GetInputElements());
        }

        private IList<IAstNode> GetParsedBody<TParser>(string input) where TParser : Parser, new()
        {
            var tokenizer = new Tokenizer("{ " + input + " }", false);
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
            Assert.IsFalse(result.IsArray);
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
        public void ChainedMethodCallsInBodyTest()
        {
            string testData;

            
            testData = @"par5EntityPlayer.getFoodStats().addStats(2, 0.1F);";
            var result = GetParsedBody<FunctionCallParser>(testData);
        }

        [TestMethod]
        public void VariableAssignmentParser()
        {
            string doubleQuote = "\"";
            VariableAssignment result;
            int number;

            number = 0;
            result = GetParsedResult<VariableAssignmentParser, VariableAssignment>(@"tricksySlashAtEnd = ""hello.\\"";");
            Assert.IsInstanceOfType(result.VariableName[0], typeof(IdentifierToken));
            Assert.IsNull(result.PreComment);
            Assert.IsNull(result.PostComment);
            Assert.AreEqual(result.AssignedValue.Count, 1);
            Assert.IsInstanceOfType(result.AssignedValue[number], typeof(StringLiteral));
            Assert.AreEqual(result.AssignedValue[number].Data, doubleQuote + @"hello.\\" + doubleQuote);

            result = GetParsedResult<VariableAssignmentParser, VariableAssignment>(@"++this.blockIndexInTexture;");
            result = GetParsedResult<VariableAssignmentParser, VariableAssignment>(@"this.blockIndexInTexture++;");
            result = GetParsedResult<VariableAssignmentParser, VariableAssignment>(@"var1 = (double)((float)par2 + 0.5F);");
        }

        [TestMethod]
        public void FunctionCallParserTest()
        {
            FunctionCallStatement result;
            IList<IAstNode> parameter;
            int number;

            result = GetParsedResult<FunctionCallParser, FunctionCallStatement>(@"this.setBlockBounds(0.5F - var3, 0.0F, 0.5F - var3, 0.5F + var3, var4, 0.5F + var3);");

            Assert.AreEqual(3, result.Method.Count);
            
            number = 0;
            Assert.IsInstanceOfType(result.Method[number], typeof(KeywordToken));
            Assert.AreEqual("this", result.Method[number].Data);
            
            number++;
            Assert.IsInstanceOfType(result.Method[number], typeof(SeparatorToken));
            Assert.AreEqual(".", result.Method[number].Data);

            number++;
            Assert.IsInstanceOfType(result.Method[number], typeof(IdentifierToken));
            Assert.AreEqual("setBlockBounds", result.Method[number].Data);

            Assert.AreEqual(6, result.Parameters.Count);

            parameter = result.Parameters[0];
            Assert.AreEqual(3, parameter.Count);
            Assert.IsInstanceOfType(parameter[0], typeof(ValueStatement));
            Assert.IsInstanceOfType(parameter[1], typeof(Operator));
            Assert.IsInstanceOfType(parameter[2], typeof(ValueStatement));

            parameter = result.Parameters[1];
            Assert.AreEqual(1, parameter.Count);
            Assert.IsInstanceOfType(parameter[0], typeof(ValueStatement));

            parameter = result.Parameters[2];
            Assert.AreEqual(3, parameter.Count);
            Assert.IsInstanceOfType(parameter[0], typeof(ValueStatement));
            Assert.IsInstanceOfType(parameter[1], typeof(Operator));
            Assert.IsInstanceOfType(parameter[2], typeof(ValueStatement));

            parameter = result.Parameters[3];
            Assert.AreEqual(3, parameter.Count);
            Assert.IsInstanceOfType(parameter[0], typeof(ValueStatement));
            Assert.IsInstanceOfType(parameter[1], typeof(Operator));
            Assert.IsInstanceOfType(parameter[2], typeof(ValueStatement));

            parameter = result.Parameters[4];
            Assert.AreEqual(1, parameter.Count);
            Assert.IsInstanceOfType(parameter[0], typeof(ValueStatement));

            parameter = result.Parameters[5];
            Assert.AreEqual(3, parameter.Count);
            Assert.IsInstanceOfType(parameter[0], typeof(ValueStatement));
            Assert.IsInstanceOfType(parameter[1], typeof(Operator));
            Assert.IsInstanceOfType(parameter[2], typeof(ValueStatement));
        }

        [TestMethod]
        public void NestedFunctionCallParserTest()
        {
            FunctionCallStatement result;
            result = GetParsedResult<FunctionCallParser, FunctionCallStatement>(@"this.setDoorRotation(this.getFullMetadata(par1IBlockAccess, par2, par3, par4));");
        }

        [TestMethod]
        public void CastFunctionCallTest()
        {
            FunctionCallStatement result;           
            result = GetParsedResult<FunctionCallParser, FunctionCallStatement>(@"((EntityPlayerMP)par5EntityPlayer).sendContainerToPlayer(par5EntityPlayer.inventoryContainer);");


            var debugOut = result.DebugOut();
            
            
        }

        [TestMethod]
        public void FunctionCallParserTestWithCastParameter()
        {
            FunctionCallStatement result;
            string testData;

            testData = @"par1World.playAuxSFXAtEntity((EntityPlayer)null, 1003, par2, par3, par4, 0);";
            result = GetParsedResult<FunctionCallParser, FunctionCallStatement>(testData);

            testData = @"par1World.newExplosion((Entity)null, (double)((float)par2 + 0.5F), (double)((float)par3 + 0.5F), (double)((float)par4 + 0.5F), 5.0F, true, true);";
            result = GetParsedResult<FunctionCallParser, FunctionCallStatement>(testData);

            testData = @"this.dropBlockAsItem_do(par1World, par2, par3, par4, new ItemStack(var10, 1, this.damageDropped(par5)));";
            result = GetParsedResult<FunctionCallParser, FunctionCallStatement>(testData);

            testData = @"par1World.spawnEntityInWorld(new EntityXPOrb(par1World, (double)par2 + 0.5D, (double)par3 + 0.5D, (double)par4 + 0.5D, var6));";
            result = GetParsedResult<FunctionCallParser, FunctionCallStatement>(testData);
            Assert.AreEqual(3, result.Method.Count);
            Assert.AreEqual(1, result.Parameters.Count);

            testData = @"par1World.newExplosion((Entity)null, (double)((float)par2 + 0.5F), (double)((float)par3 + 0.5F), (double)((float)par4 + 0.5F), 5.0F, true, true);";
            result = GetParsedResult<FunctionCallParser, FunctionCallStatement>(testData);
            Assert.AreEqual(7, result.Parameters.Count);
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
            Assert.AreEqual("0", initializer.AssignedValue[0].Data);
            Assert.AreEqual("int", initializer.InitializedType.Data);
            Assert.AreEqual("var9", initializer.VariableName.Data);

            Assert.AreEqual(1, result.CounterExpressions.Count);
            Assert.AreEqual(2, result.CounterExpressions[0].Count);
            Assert.AreEqual("++", result.CounterExpressions[0][0].Data);
            Assert.AreEqual("var9", result.CounterExpressions[0][1].Data);

            Assert.AreEqual(5, result.Condition.Count);
            Assert.AreEqual("var9", result.Condition[0].Data);
            Assert.IsInstanceOfType(result.Condition[1], typeof(WhiteSpaceInputElement));
            Assert.AreEqual("<", result.Condition[2].Data);
            Assert.IsInstanceOfType(result.Condition[3], typeof(WhiteSpaceInputElement));
            Assert.AreEqual("var8", result.Condition[4].Data);

            Assert.AreEqual(1, result.Body.Count);
            Assert.IsInstanceOfType(result.Body[0], typeof(IfElseStatement));
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

            Assert.AreEqual(5, result.Condition.Count);
            Assert.AreEqual("par5", result.Condition[0].Data);
            Assert.IsInstanceOfType(result.Condition[1], typeof(WhiteSpaceInputElement));
            Assert.AreEqual(">", result.Condition[2].Data);
            Assert.IsInstanceOfType(result.Condition[3], typeof(WhiteSpaceInputElement));
            Assert.AreEqual("0", result.Condition[4].Data);
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
            Assert.AreEqual("this", result.ControlStatement[0].Data);
            Assert.AreEqual(".", result.ControlStatement[1].Data);
            Assert.AreEqual("GetValueOfVar3", result.ControlStatement[2].Data);
            Assert.AreEqual("(", result.ControlStatement[3].Data);
            Assert.AreEqual(")", result.ControlStatement[4].Data);

            Assert.AreEqual(6, result.Body.Count);
            Assert.IsInstanceOfType(result.Body[0], typeof(CaseStatement));
            Assert.IsInstanceOfType(result.Body[1], typeof(ReturnStatement));
            Assert.IsInstanceOfType(result.Body[2], typeof(CaseStatement));
            Assert.IsInstanceOfType(result.Body[3], typeof(ReturnStatement));
            Assert.IsInstanceOfType(result.Body[4], typeof(SwitchDefaultStatement));
            Assert.IsInstanceOfType(result.Body[5], typeof(ReturnStatement));
        }

        [TestMethod]
        public void TryParseBlockFileTest()
        {
            Directory.SetCurrentDirectory(@"D:\Users\Jonathan Lydall\Downloads\mcp\mcp723\src\minecraft\net\minecraft\src\");

            GetParsedResultFromFile("Block.java");
            GetParsedResultFromFile("BlockAnvil.java");
            GetParsedResultFromFile("BlockBeacon.java");
            GetParsedResultFromFile("BlockBed.java");
            GetParsedResultFromFile("BlockBookshelf.java");
            GetParsedResultFromFile("BlockBreakable.java");
            GetParsedResultFromFile("BlockBrewingStand.java");
            GetParsedResultFromFile("BlockButton.java");
            GetParsedResultFromFile("BlockCactus.java");
            GetParsedResultFromFile("BlockCake.java");
            GetParsedResultFromFile("BlockCarrot.java");
            GetParsedResultFromFile("BlockCauldron.java");
            GetParsedResultFromFile("BlockChest.java");
            GetParsedResultFromFile("BlockClay.java");
            GetParsedResultFromFile("BlockCloth.java");
            GetParsedResultFromFile("BlockCocoa.java");
            GetParsedResultFromFile("BlockCommandBlock.java");
            GetParsedResultFromFile("BlockContainer.java");
            GetParsedResultFromFile("BlockCrops.java");
            GetParsedResultFromFile("BlockDeadBush.java");
            GetParsedResultFromFile("BlockDetectorRail.java");
            GetParsedResultFromFile("BlockDirectional.java");
            GetParsedResultFromFile("BlockDirt.java");
            GetParsedResultFromFile("BlockDispenser.java");
            GetParsedResultFromFile("BlockDoor.java");
            GetParsedResultFromFile("BlockDragonEgg.java");
            GetParsedResultFromFile("BlockEnchantmentTable.java");
            GetParsedResultFromFile("BlockEnderChest.java");
            GetParsedResultFromFile("BlockEndPortal.java");
            GetParsedResultFromFile("BlockEndPortalFrame.java");
            GetParsedResultFromFile("BlockEventData.java");
            GetParsedResultFromFile("BlockFarmland.java");
            GetParsedResultFromFile("BlockFence.java");
            GetParsedResultFromFile("BlockFenceGate.java");
            GetParsedResultFromFile("BlockFire.java");
            GetParsedResultFromFile("BlockFlower.java");
            GetParsedResultFromFile("BlockFlowerPot.java");
            GetParsedResultFromFile("BlockFlowing.java");
            GetParsedResultFromFile("BlockFluid.java");
            GetParsedResultFromFile("BlockFurnace.java");
            GetParsedResultFromFile("BlockGlass.java");
            GetParsedResultFromFile("BlockGlowStone.java");
            GetParsedResultFromFile("BlockGrass.java");
            GetParsedResultFromFile("BlockGravel.java");
            GetParsedResultFromFile("BlockHalfSlab.java");
            GetParsedResultFromFile("BlockIce.java");
            GetParsedResultFromFile("BlockJukeBox.java");
            GetParsedResultFromFile("BlockLadder.java");
            GetParsedResultFromFile("BlockLeaves.java");
            GetParsedResultFromFile("BlockLeavesBase.java");
            GetParsedResultFromFile("BlockLever.java");
            GetParsedResultFromFile("BlockLilyPad.java");
            GetParsedResultFromFile("BlockLockedChest.java");
            GetParsedResultFromFile("BlockLog.java");
            GetParsedResultFromFile("BlockMelon.java");
            GetParsedResultFromFile("BlockMobSpawner.java");
            GetParsedResultFromFile("BlockMushroom.java");
            GetParsedResultFromFile("BlockMushroomCap.java");
            GetParsedResultFromFile("BlockMycelium.java");
            GetParsedResultFromFile("BlockNetherrack.java");
            GetParsedResultFromFile("BlockNetherStalk.java");
            GetParsedResultFromFile("BlockNote.java");
            GetParsedResultFromFile("BlockObsidian.java");
            GetParsedResultFromFile("BlockOre.java");
            GetParsedResultFromFile("BlockOreStorage.java");
            GetParsedResultFromFile("BlockPane.java");
            GetParsedResultFromFile("BlockPistonBase.java");
            GetParsedResultFromFile("BlockPistonExtension.java");
            GetParsedResultFromFile("BlockPistonMoving.java");
            GetParsedResultFromFile("BlockPortal.java");
            GetParsedResultFromFile("BlockPotato.java");
            GetParsedResultFromFile("BlockPressurePlate.java");
            GetParsedResultFromFile("BlockPumpkin.java");
            GetParsedResultFromFile("BlockRail.java");
            GetParsedResultFromFile("BlockRedstoneLight.java");
            GetParsedResultFromFile("BlockRedstoneOre.java");
            GetParsedResultFromFile("BlockRedstoneRepeater.java");
            GetParsedResultFromFile("BlockRedstoneTorch.java");
            GetParsedResultFromFile("BlockRedstoneWire.java");
            GetParsedResultFromFile("BlockReed.java");
            GetParsedResultFromFile("BlockSand.java");
            GetParsedResultFromFile("BlockSandStone.java");
            GetParsedResultFromFile("BlockSapling.java");
            GetParsedResultFromFile("BlockSign.java");
            GetParsedResultFromFile("BlockSilverfish.java");
            GetParsedResultFromFile("BlockSkull.java");
            GetParsedResultFromFile("BlockSnow.java");
            GetParsedResultFromFile("BlockSnowBlock.java");
            GetParsedResultFromFile("BlockSoulSand.java");
            GetParsedResultFromFile("BlockSourceImpl.java");
            GetParsedResultFromFile("BlockSponge.java");
            GetParsedResultFromFile("BlockStairs.java");
            GetParsedResultFromFile("BlockStationary.java");
            GetParsedResultFromFile("BlockStem.java");
            GetParsedResultFromFile("BlockStep.java");
            GetParsedResultFromFile("BlockStone.java");
            GetParsedResultFromFile("BlockStoneBrick.java");
            GetParsedResultFromFile("BlockTallGrass.java");
            GetParsedResultFromFile("BlockTNT.java");
            GetParsedResultFromFile("BlockTorch.java");
            GetParsedResultFromFile("BlockTrapDoor.java");
            GetParsedResultFromFile("BlockTripWire.java");
            GetParsedResultFromFile("BlockTripWireSource.java");
            GetParsedResultFromFile("BlockVine.java");
            GetParsedResultFromFile("BlockWall.java");
            GetParsedResultFromFile("BlockWeb.java");
            GetParsedResultFromFile("BlockWood.java");
            GetParsedResultFromFile("BlockWoodSlab.java");
            GetParsedResultFromFile("BlockWorkbench.java");
        }
    }
} 
