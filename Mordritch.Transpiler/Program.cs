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
        const string basePath = @"D:\Users\Jonathan Lydall\Downloads\mcp\mcp742\src\minecraft_server\net\minecraft\src\";
        //static string basePath = @"C:\Users\jonathan.lydall.ZA\Desktop\New folder (2)\mcp\mcp742\src\minecraft_server\net\minecraft\src\";

        const string destinationPath = @"D:\Users\Jonathan Lydall\Documents\visual studio 2012\Projects\McSim\McSim\";

        const string destinationSubFolder = "minecraft";

        //static string fileName = basePath + @"BlockDoor.java";
        //static string fileName = basePath + @"Block.java";
        //static string fileName = basePath + @"Direction.java";
        //static string fileName = basePath + @"World.java";
        //static string fileName = basePath + @"BlockLeaves.java";
        //static string fileName = basePath + @"EntityLiving.java";
        
        static void Main(string[] args)
        {
            Utils.LoggingEnabled = false;
            OtherTypes.BasePath = basePath;

            //Transpile(basePath + "Block.java", destinationPath + @"\minecraft\Block.ts");
            
            CompileAll();
            //GenerateAllDefinitions();
            
            //OtherTypes.DumpList();

            //var blockfiles = Directory.GetFiles(basePath, "*.java");
            //foreach (var file in blockfiles)
            //{
            //    if (
            //        file.Contains("WorldGenDungeons") ||
            //        file.Contains("Enum") ||
            //        file.Contains("NBT") ||
            //        file.Contains("SpawnerAnimals") ||
            //        file.Contains("IntCache") ||
            //        file.Contains("TcpMasterThread"))
            //    {
            //        continue;
            //    }
            //    Console.WriteLine("Processing {0}...", file);
            //    GetParsedData(file);
            //}

            Utils.Pause();
        }

        static void GenerateAllDefinitions()
        {
            var files = new[] {
                "World",
                "Chunk"
            };

            foreach (var file in files)
            {
                GenerateDefinition(basePath + @"\" + file + ".java", destinationPath + @"\minecraft.d\" + file + ".d.ts");
            }

        }

        static void CompileAll()
        {
            var fileSearchPatterns = new[] {
                "Block*.java",
                "Material*.java",
                "TileEntity*.java",
                "CreativeTab*.java",
                "Entity*.java"
            };

            var miscellaneousFiles = new[]
            {
                "MapColor",
                "Direction",
                "ExtendedBlockStorage",
                "NibbleArray",
                "MathHelper",
            };

            var files = new List<string>();
            foreach (var pattern in fileSearchPatterns)
            {
                files = files.Union(Directory.GetFiles(basePath, pattern)).ToList();
            }

            var fileList = files
                .Select(x => GetFileName(x))
                .ToList();

            fileList = fileList.Union(miscellaneousFiles).ToList();

            OtherTypes.ToBeCompiledList = fileList;

            UpdateProjectFile(fileList);
            GenerateIncludeFile(fileList);

            foreach (var file in fileList)
            {
                Console.WriteLine("Processing {0}...", file);
                Transpile(
                    string.Format("{0}{1}.java", basePath, file),
                    string.Format(@"{0}\{1}\{2}.ts", destinationPath, destinationSubFolder, file));

                File.Create(string.Format(@"{0}\{1}\{2}.js", destinationPath, destinationSubFolder, file));
            }
        }

        static string GetFileName(string file)
        {
            var fileInfo = new FileInfo(file);
            return fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
        }

        static void Transpile(string sourceFile, string destinationFile)
        {
            var parsedData = GetParsedData(sourceFile);
            var compiled = TypeScriptCompiler.Compile(parsedData);

            compiled =
                @"/// <reference path=""..\Includes.ts"" />" +
                Environment.NewLine +
                Environment.NewLine +
                compiled;

            File.WriteAllText(destinationFile, compiled);
        }

        static void GenerateDefinition(string sourceFile, string destinationFile)
        {
            var parsedData = GetParsedData(sourceFile);
            var compiled = TypeScriptCompiler.GenerateDefinition(parsedData);

            compiled =
                @"/// <reference path=""..\Includes.ts"" />" +
                Environment.NewLine +
                Environment.NewLine +
                compiled;
            File.WriteAllText(destinationFile, compiled);
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

        static void GenerateIncludeFile(IList<string> list)
        {
            var file = string.Format(@"{0}\Includes.ts", destinationPath);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(@"/// <reference path=""manualIncludes\_ManualIncludes.ts"" />");
            stringBuilder.AppendLine(@"/// <reference path=""IncludesInterfaces.ts"" />");

            foreach (var entry in list)
            {
                stringBuilder.AppendLine(string.Format(@"/// <reference path=""minecraft\{0}.ts"" />", entry));
            }

            File.WriteAllText(file, stringBuilder.ToString());
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
