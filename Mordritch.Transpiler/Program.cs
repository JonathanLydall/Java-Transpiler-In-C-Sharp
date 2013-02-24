using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mordritch.Transpiler.Java.Tokenizer;
using Mordritch.Transpiler.Java.Common;
using Mordritch.Transpiler.Java.AstGenerator;

namespace Mordritch.Transpiler
{
    class Program
    {
        const string fileName = @"D:\Users\Jonathan Lydall\Downloads\mcp\mcp723\src\minecraft\net\minecraft\src\BlockDoor.java";
        
        static void Main(string[] args)
        {
            var tokenizer = new Tokenizer(fileName);
            var inputElements = tokenizer.GetInputElements();

            var astGen = new AstGenerator();
            var j = astGen.Parse(inputElements);


            //dumpTokens(inputElements);
            
            Utils.Pause();
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
