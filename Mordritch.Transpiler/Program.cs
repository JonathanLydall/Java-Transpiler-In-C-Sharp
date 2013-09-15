using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mordritch.Transpiler.Java.Tokenizer;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Compilers.TypeScript;
using Mordritch.Transpiler.Utilities;
using Mordritch.Transpiler.src.Compilers.TypeScript;
using Mordritch.Transpiler.Compilers;

namespace Mordritch.Transpiler
{
    class Program
    {
        private static IDictionary<string, IList<IAstNode>> _sourceFiles = new Dictionary<string, IList<IAstNode>>();
        
        const string basePath = @"D:\Users\Jonathan Lydall\Downloads\mcp\mcp742\src\minecraft_server\net\minecraft\src\";
        //static string basePath = @"C:\Users\jonathan.lydall.ZA\Desktop\New folder (2)\mcp\mcp742\src\minecraft_server\net\minecraft\src\";

        const string destinationPath = @"D:\Users\Jonathan Lydall\Documents\visual studio 2012\Projects\McSim\McSim\";

        const string destinationSubFolder = "minecraft";

        static string[] _definitionList = new string[] {
                "World",
                "Chunk",
                //"NBTTagShort",
                //"NBTTagLong",
                //"NBTTagList",
                //"NBTTagIntArray",
                //"NBTTagInt",
                //"NBTTagFloat",
                //"NBTTagEnd",
                //"NBTTagDouble",
                ////"NBTTagCompound",
                //"NBTTagByteArray",
                //"NBTTagByte",
                //"NBTTagString",
                //"NBTBase",
            };

        static string[] _transpileSearchPatterns = new string[] {
                //"Inventory*.java",
                //"Block*.java",
                //"Material*.java",
                //"TileEntity*.java",
                //"CreativeTab*.java"
            };

        static string[] _parseOnlyFiles = new string[]
            {
                "Block",
                "BlockContainer",

                "Vec3",

                "EntityCreature",
                "EntityGolem",
                "EntityIronGolem",
                "EntitySnowman",

                "EntityFallingSand",
                "TileEntityComparator",

                "ChunkCoordinates",
                "ChunkPosition",
            };

        static string[] _transpileIndividualFiles = new string[]
            {
                "BlockBasePressurePlate",
                "BlockBookshelf",
                "BlockBreakable",
                "BlockButton",
                "BlockButtonStone",
                "BlockButtonWood",
                "BlockCake",
                "BlockClay",
                "BlockCloth",
                "BlockComparator",
                "BlockDirectional",
                "BlockDirt",
                "BlockDoor",
                "BlockFence",
                "BlockFenceGate",
                "BlockFire",
                "BlockGlass",
                "BlockGlowStone",
                "BlockGrass",
                "BlockGravel",
                "BlockHalfSlab",
                "BlockLeaves",
                "BlockLeavesBase",
                "BlockLever",
                "BlockLog",
                "BlockNetherrack",
                "BlockObsidian",
                "BlockOre",
                "BlockOreStorage",
                "BlockPistonBase",
                "BlockPistonExtension",
                "BlockPistonMoving",
                "BlockPressurePlate",
                "BlockPressurePlateWeighted",
                "BlockPumpkin",
                "BlockRedstoneLogic",
                "BlockRedstoneOre",
                "BlockRedstoneRepeater",
                "BlockRedstoneTorch",
                "BlockRedstoneWire",
                "BlockSand",
                "BlockSandStone",
                "BlockSign",
                "BlockSnow",
                "BlockSnowBlock",
                "BlockSoulSand",
                "BlockSponge",
                "BlockStairs",
                "BlockStep",
                "BlockStone",
                "BlockStoneBrick",
                "BlockTorch",
                "BlockWall",
                "BlockWood",

                "Material",
                "MaterialLiquid",
                "MaterialLogic",
                "MaterialPortal",
                "MaterialTransparent",
                "MaterialWeb",

                "MapColor",

                "StepSound",
                "StepSoundAnvil",
                "StepSoundSand",
                "StepSoundStone",

                "RedstoneUpdateInfo",

                "AxisAlignedBB",
                "AABBLocalPool",
                "AABBPool",

                "Vec3Pool",
                "MovingObjectPosition",

                "Facing",

                "Direction",


                //"Entity", // Made manually

                //"MathHelper", // Made manually
                //"TileEntity", // Made manually
                //"TileEntitySign", // Made manually
                //"TileEntityPiston", // Made manually

                //"NBTBase", // Made manually
                //"NBTTagByte", // Made manually
                //"NBTTagByteArray", // Made manually
                //"NBTTagCompound", // Made manually
                //"NBTTagDouble", // Made manually
                //"NBTTagEnd", // Made manually
                //"NBTTagFloat", // Made manually
                //"NBTTagInt", // Made manually
                //"NBTTagIntArray", // Made manually
                //"NBTTagList", // Made manually
                //"NBTTagLong", // Made manually
                //"NBTTagShort", // Made manually
                //"NBTTagString", // Made manually


                //"AxisAlignedBB",
                //"Container",
                //"EntityBoat",
                //"EntityFallingSand",
                //"EntityFireworkRocket",
                //"EntityItem",
                //"EntityMinecart",
                //"EntityMinecartContainer",
                //"EntityMinecartChest",
                //"EntityMinecartHopper",
                //"EntityMinecartEmpty",
                //"EntityMinecartFurnace",
                //"EntityMinecartMobSpawner",
                //"EntityMinecartTNT",
                //"ExtendedBlockStorage",
                //"GameRules",
                //"GameRuleValue",
                //"MathHelper",
                //"MapColor",
                //"MobSpawnerBaseLogic",
                //"NextTickListEntry",
                //"NibbleArray",
                //"PositionImpl",
                //"RedstoneUpdateInfo",
                //"Slot",
                //"WeightedRandomMinecart",
                //"WorldGenBigTree",
                //"WorldGenForest",
                //"WorldGenHugeTrees",
                //"WorldGenerator",
                //"WorldGenTaiga2",
                //"WorldGenTrees",

                //"ItemStack",
                //"WorldServer"
            };

        static void Main(string[] args)
        {
            Utils.LoggingEnabled = false;
            OtherTypes.BasePath = basePath;

            ParseAll();
            //DumpUsedTypes(_sourceFiles.First().Value);

            //TranspileSingleFile("WorldServer");

            CompileAllClasses();
            //GenerateAllDefinitions();
            
            //OtherTypes.DumpList();

            Utils.Pause();
        }

        static void TranspileSingleFile(string file)
        {
            Console.WriteLine("Transpiling {0}...", file);
            Transpile(file);
        }

        static void ParseAll()
        {
            var fileList = GetCompilerFileList();

            fileList = fileList.Union(_parseOnlyFiles).ToList();

            foreach (var file in fileList)
            {
                Console.WriteLine("Parsing {0}...", file);
                var parsedData = GetParsedData(string.Format("{0}{1}.java", basePath, file));
                _sourceFiles.Add(file, parsedData);
            }

            foreach (var file in _definitionList)
            {
                Console.WriteLine("Parsing {0}...", file);
                var parsedData = GetParsedData(string.Format("{0}{1}.java", basePath, file));
                _sourceFiles.Add(file, parsedData);
            }
        }

        static void CompileAllClasses()
        {
            var fileList = GetCompilerFileList();

            UpdateProjectFile(fileList);

            foreach (var file in fileList)
            {
                Console.WriteLine("Transpiling {0}...", file);
                Transpile(file);
            }
        }

        static void GenerateAllDefinitions()
        {
            foreach (var file in _definitionList)
            {
                Console.WriteLine("Generating Type Definition for {0}...", file);
                GenerateDefinition(file);
            }
        }

        static List<string> GetCompilerFileList()
        {
            var files = new List<string>();
            foreach (var pattern in _transpileSearchPatterns)
            {
                files = files.Union(Directory.GetFiles(basePath, pattern)).ToList();
            }

            var fileList = files
                .Select(x => GetFileName(x))
                .ToList();

            fileList = fileList.Union(_transpileIndividualFiles).ToList();

            OtherTypes.ToBeCompiledList = fileList;

            return fileList;
        }

        static string GetFileName(string file)
        {
            var fileInfo = new FileInfo(file);
            return fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
        }

        static void Transpile(string file)
        {
            var compiled = TypeScriptCompiler.Compile(_sourceFiles, file);
            var destinationFile = string.Format(@"{0}\{1}\{2}", destinationPath, destinationSubFolder, file);

            File.WriteAllText(destinationFile + ".ts", compiled);
            File.Create(destinationFile + ".js");
        }

        static void GenerateDefinition(string file)
        {
            var compiled = TypeScriptCompiler.GenerateDefinition(_sourceFiles, file);
            var destinationFile = string.Format(@"{0}\minecraft.d\{1}.d", destinationPath, file);

            File.WriteAllText(destinationFile + ".ts", compiled);
            File.Create(destinationFile + ".js");
        }

        static IList<IAstNode> GetParsedData(string sourceFile)
        {
            var tokenizer = new Tokenizer(sourceFile);
            var inputElements = tokenizer.GetInputElements();

            var fileInfo = new FileInfo(sourceFile);
            var astGen = new AstGenerator();
            var parsedData = astGen.Parse(inputElements, fileInfo.Name);

            return parsedData;
        }

        static void UpdateProjectFile(IList<string> list)
        {
            var folder = "minecraft";
            var project = @"D:\users\jonathan lydall\documents\visual studio 2012\Projects\McSim\McSim\McSim.csproj";

            TypeScriptProject.GenerateTypeScriptReferences(list, project, folder);
        }

        static void DumpUsedTypes(IList<IAstNode> astNodes)
        {
            foreach (var node in astNodes)
            {
                foreach (var type in node.GetUsedTypes())
                {
                    Console.WriteLine("    {0}", type);
                }
            }
        }

        //private static void dumpTokens(IList<Java.Tokenizer.InputElements.InputElementTypes.IInputElement> inputElements)
        //{
        //    var counter = 0;
        //    foreach (var inputElement in inputElements)
        //    {
        //        Console.WriteLine((inputElement.GetInputElementType() + ":").PadRight(43) + inputElement.Data.Replace(Environment.NewLine,  Environment.NewLine + "".PadRight(43)) + " " + inputElement.Position);
        //        if (counter++ > 9000)
        //        {
        //            break;
        //        }
        //    }
        //}
    }
}
