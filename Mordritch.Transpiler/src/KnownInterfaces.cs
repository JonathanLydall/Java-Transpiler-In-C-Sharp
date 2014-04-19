using Mordritch.Transpiler.Java.Tokenizer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler
{
    public class KnownInterfaces
    {
        private static IList<string> _knownInterfaces = new List<string>();

        public static void GatherKnownInterfaces(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            var fileInfos = directoryInfo.GetFiles("*.java");
            var fileNames = fileInfos.Select(x => x.FullName);

            foreach (var fileName in fileNames)
            {
                var fileLines = File.ReadAllLines(fileName);
                var declarationLine = fileLines.FirstOrDefault(x => x.Contains("public interface "));

                if (declarationLine == null)
                {
                    continue;
                }

                var splitLine = declarationLine.Split(' ').ToList();
                var index = splitLine.IndexOf("interface") + 1;
                _knownInterfaces.Add(splitLine[index]);
            }
        }

        public static bool IsKnown(string typeName)
        {
            return _knownInterfaces.Contains(typeName);
        }
    }
}
