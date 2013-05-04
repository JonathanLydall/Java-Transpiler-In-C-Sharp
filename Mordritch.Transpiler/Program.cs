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

namespace Mordritch.Transpiler
{
    class Program
    {
        const string basePath = @"D:\Users\Jonathan Lydall\Downloads\mcp\mcp742\src\minecraft_server\net\minecraft\src\";
        //static string basePath = @"C:\Users\jonathan.lydall.ZA\Desktop\New folder (2)\mcp\mcp742\src\minecraft_server\net\minecraft\src\";

        //static string fileName = basePath + @"BlockDoor.java";
        //static string fileName = basePath + @"Block.java";
        //static string fileName = basePath + @"Direction.java";
        static string fileName = basePath + @"World.java";
        //static string fileName = basePath + @"BlockLeaves.java";
        //static string fileName = basePath + @"EntityLiving.java";
        
        static void Main(string[] args)
        {
            var tokenizer = new Tokenizer(fileName);
            var inputElements = tokenizer.GetInputElements();

            var fileInfo = new FileInfo(fileName);
            var astGen = new AstGenerator();
            var parsedData = astGen.Parse(inputElements, fileInfo.Name);

            Compile(parsedData);

            Utils.Pause();
        }

        private static void Compile(IList<IAstNode> parsedData)
        {
            var compiled = TypeScriptCompiler.Compile(parsedData);
            Console.WriteLine();

            compiled = @"/// <reference path=""interfaces.ts"" />" + Environment.NewLine + Environment.NewLine + compiled;
            //File.WriteAllText(@"C:\Users\jonathan.lydall.ZA\Desktop\New folder (2)\Projects\McSim\McSim\BlockSample.ts", compiled);
            File.WriteAllText(@"d:\users\jonathan lydall\documents\visual studio 2012\Projects\McSim\McSim\BlockSample.ts", compiled);
        }

        private static void dumpTokens(IList<Java.Tokenizer.InputElements.InputElementTypes.IInputElement> inputElements)
        {
            var counter = 0;
            foreach (var inputElement in inputElements)
            {
                Console.WriteLine((inputElement.GetInputElementType() + ":").PadRight(43) + inputElement.Data.Replace(Environment.NewLine,  Environment.NewLine + "".PadRight(43)) + " " + inputElement.Position);
                if (counter++ > 9000)
                {
                    break;
                }
            }
        }
    }
}
