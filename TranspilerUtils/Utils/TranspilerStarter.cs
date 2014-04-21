using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerUtils.Utils
{
    public class TranspilerStarter
    {
        private const string PARAMETER_PREFIX = "-";

        public static void SingleClass(string className)
        {
            var parameters = GetRequiredParameters();

            parameters.Add(string.Format("{0}singleClassToCompile", PARAMETER_PREFIX));
            parameters.Add(className);

            StartProcess(parameters);
        }

        public static void AllClasses()
        {
            StartProcess(GetRequiredParameters());
        }

        private static IList<string> GetRequiredParameters()
        {
            var parameters = new List<string>();

            parameters.Add(string.Format("{0}javaSourceFilesPath", PARAMETER_PREFIX));
            parameters.Add(WrapInDoubleQuotes(App.CurrentJavaFilesPath));

            parameters.Add(string.Format("{0}javaClassMetadataFilesPath", PARAMETER_PREFIX));
            parameters.Add(WrapInDoubleQuotes(App.CurrentXmlPath));
            
            parameters.Add(string.Format("{0}projectFile", PARAMETER_PREFIX));
            parameters.Add(WrapInDoubleQuotes(App.CurrentProjectFilePath));
            
            parameters.Add(string.Format("{0}projectTranspiledSubfolder", PARAMETER_PREFIX));
            parameters.Add(WrapInDoubleQuotes(App.CurrentProjectTranspiledSubfolder));
            
            parameters.Add(string.Format("{0}projectTranspiledButExtendedSubfolder", PARAMETER_PREFIX));
            parameters.Add(WrapInDoubleQuotes(App.CurrentProjectTranspiledButExtendedSubfolder));
            
            parameters.Add(string.Format("{0}pauseOnExit", PARAMETER_PREFIX));

            return parameters;
        }

        private static void StartProcess(IList<string> parameters)
        {
            var process = new Process();

            process.StartInfo.FileName = App.CurrentTranspilerPath;
            process.StartInfo.Arguments = parameters.Aggregate((x, y) => x + " " + y);

            process.Start();
        }

        private static string WrapInDoubleQuotes(string input)
        {
            return string.Format("\"{0}\"", input);
        }
    }
}
