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
using Mordritch.Transpiler.src.Utilities;
using Mordritch.Transpiler.src.Compilers;
using Mordritch.Transpiler.src;
using Mordritch.Transpiler.Contracts;
using System.Diagnostics;

namespace Mordritch.Transpiler
{
    class Program
    {
        private static IDictionary<string, IList<IAstNode>> _sourceFiles = new Dictionary<string, IList<IAstNode>>();

        public const string NEEDS_EXTENDING_EXTENSION = "_NeedsExtending";

        private static string _javaSourceFilesPath;
        private static string _singleClassToCompile;
        private static string _javaClassMetadataFilesPath;
        private static string _projectFile;
        private static string _projectTranspiledSubfolder;
        private static string _projectTranspiledButExtendedSubfolder;
        
        private static bool _pauseOnExit = false;

        static void Main(string[] args)
        {
            Debugger.Launch();
            CommandLineParser.AddOption("javaSourceFilesPath", "Folder containing Java source files.", x => _javaSourceFilesPath = x, true);
            CommandLineParser.AddOption("singleClassToCompile", "Only compile this one class, although all other files will be parsed.", x =>
            {
                _singleClassToCompile = x;
            });
            CommandLineParser.AddOption("javaClassMetadataFilesPath", "Folder containing XML files describing which Java classes, methods and fields to compile, ignore or extend.", x => _javaClassMetadataFilesPath = x, true);
            CommandLineParser.AddOption("projectFile", "Path the Visual Studio project file which compiles TypeScript files.", x => _projectFile = x, true);
            CommandLineParser.AddOption("projectTranspiledSubfolder", "Subfolder in the Visual Studio project root in which Transpiled files are placed.", x => _projectTranspiledSubfolder = x, true);
            CommandLineParser.AddOption("projectTranspiledButExtendedSubfolder", "Subfolder in the Visual Studio project root in which Transpiled files are placed.", x => _projectTranspiledButExtendedSubfolder = x, true);

            CommandLineParser.AddFlagOption("pauseOnExit", "Shows 'Press any key to continue.' before the program exits, allowing you to review any output.", () => _pauseOnExit = true);

            try
            {
                CommandLineParser.Parse(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Utils.ConditionalPause(_pauseOnExit);
                return;
            }

            KnownInterfaces.GatherKnownInterfaces(_javaSourceFilesPath);
            JavaClassMetadata.Load(_javaClassMetadataFilesPath);

            Utils.LoggingEnabled = false;
            OtherTypes.BasePath = _javaSourceFilesPath;

            ParseAll();

            if (string.IsNullOrEmpty(_singleClassToCompile))
            {
                TranspileAll();
                TranspileClassesNeedingExtending();
                //GenerateAllDefinitions();
            }
            else
            {
                TranspileSingleFile(_singleClassToCompile);
            }

            //DumpUsedTypes(_sourceFiles.First().Value);
            //OtherTypes.DumpList();

            Utils.ConditionalPause(_pauseOnExit);
        }

        static void TranspileSingleFile(string file)
        {
            if (JavaClassMetadata.GetClass(file).NeedsExtending())
            {
                Console.WriteLine("Transpiling {0} for extending...", file);
                Transpile(file, _projectTranspiledButExtendedSubfolder, true);
            }
            else
            {
                Console.WriteLine("Transpiling {0}...", file);
                Transpile(file, _projectTranspiledSubfolder, false);
            }
        }

        static void ParseAll()
        {
            var fileList = JavaClassMetadata.GetFilesToParse().Select(x => x.Name);
            
            foreach (var file in fileList)
            {
                Console.WriteLine("Parsing {0}...", file);
                var parsedData = GetParsedData(string.Format(@"{0}\{1}.java", _javaSourceFilesPath, file));
                _sourceFiles.Add(file, parsedData);
            }
        }

        static void DeleteAllFilesInFolder(string folder)
        {
            if (!Directory.Exists(folder))
            {
                return;
            }

            var di = new DirectoryInfo(folder);
            foreach (var fi in di.GetFiles())
            {
                fi.Delete();
            }
        }

        static void TranspileAll()
        {
            var fileList = JavaClassMetadata.GetFilesToTranspile()
                .Select(x => x.Name)
                .ToList();

            UpdateProjectFile(fileList, _projectTranspiledSubfolder, false);
            DeleteAllFilesInFolder(string.Format(@"{0}\{1}", Path.GetDirectoryName(_projectFile), _projectTranspiledSubfolder));

            foreach (var file in fileList)
            {
                Console.WriteLine("Transpiling {0}...", file);
                Transpile(file, _projectTranspiledSubfolder, false);
            }
        }

        static void TranspileClassesNeedingExtending()
        {
            var fileList = JavaClassMetadata.GetFilesNeedingExtending()
                .Select(x => x.Name)
                .ToList();

            UpdateProjectFile(fileList, _projectTranspiledButExtendedSubfolder, true);
            DeleteAllFilesInFolder(string.Format(@"{0}\{1}", Path.GetDirectoryName(_projectFile), _projectTranspiledButExtendedSubfolder));
            
            foreach (var file in fileList)
            {
                Console.WriteLine("Transpiling {0} for extending...", file);
                Transpile(file, _projectTranspiledButExtendedSubfolder, true);
            }
        }
        
        static void Transpile(string file, string subFolder, bool needsExtending)
        {
            var compiled = TypeScriptCompiler.Compile(_sourceFiles, file);
            var projectPath = Path.GetDirectoryName(_projectFile);
            var destinationFolder = string.Format(@"{0}\{1}", projectPath, subFolder);
            var destinationFile = string.Format(@"{0}\{1}", destinationFolder, file);

            if (needsExtending)
            {
                destinationFile = string.Format(@"{0}{1}", destinationFile, NEEDS_EXTENDING_EXTENSION);
            }

            Directory.CreateDirectory(destinationFolder);
            File.WriteAllText(destinationFile + ".ts", compiled);
            //using (File.Create(destinationFile + ".js")) { } // Ensure dispose gets called to remove the lock
        }

        static void GenerateDefinition(string file)
        {
            var compiled = TypeScriptCompiler.GenerateDefinition(_sourceFiles, file);
            var projectPath = Path.GetDirectoryName(_projectFile);
            var destinationFolder = string.Format(@"{0}\minecraft.d", projectPath);
            var destinationFile = string.Format(@"{0}\{1}.d", destinationFolder, file);

            Directory.CreateDirectory(destinationFolder);
            File.WriteAllText(destinationFile + ".ts", compiled);
            //using (File.Create(destinationFile + ".js")) { } // Ensure dispose gets called to remove the lock
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

        static void UpdateProjectFile(IList<string> list, string subFolderName, bool needsExtending)
        {
            TypeScriptProject.GenerateTypeScriptReferences(list, _projectFile, subFolderName, needsExtending);
            TypeScriptReferences.Generate(list, _projectFile, subFolderName, needsExtending);
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
