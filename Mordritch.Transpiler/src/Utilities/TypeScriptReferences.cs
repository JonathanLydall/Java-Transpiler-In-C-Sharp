using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mordritch.Transpiler.src.Utilities
{
    static class TypeScriptReferences
    {
        public static void Generate(IList<string> files, string projectFile, string sourceFolder, bool needsExtending)
        {
            var referencesFilePath = string.Format(@"{0}\_references.ts", Path.GetDirectoryName(projectFile));
            CleanOldFiles(files, referencesFilePath, sourceFolder, needsExtending);

        }

        private static void CleanOldFiles(IList<string> files, string referencesFile, string sourceFolder, bool needsExtending)
        {
            if (needsExtending)
            {
                files = files.Select(x => string.Format("{0}{1}", x, Program.NEEDS_EXTENDING_EXTENSION)).ToList();
            }

            var lines = File.ReadAllLines(referencesFile).ToList();
            var stringBuilder = new StringBuilder();

            var linesToRemove = lines
                .Where(x => IsAReferenceInFolderButNotSubfolder(x, sourceFolder) && files.All(y => y != GetFileName(x)))
                .ToList();

            foreach (var line in linesToRemove)
            {
                lines.Remove(line);
            }

            var linesToAdd = files
                .Where(x => lines.All(y => GetFileName(y) != x))
                .Select(x => string.Format("/// <reference path=\"{0}/{1}.ts\" />", sourceFolder.Replace('\\', '/'), x))
                .ToList();
            
            lines.AddRange(linesToAdd);

            if (linesToRemove.Any() || linesToAdd.Any()) {
                File.WriteAllLines(referencesFile, lines);
            }
        }

        private static string GetFileName(string line)
        {
            XElement xElement = null;

            if (!line.Trim().StartsWith("///"))
            {
                return null;
            }

            try
            {
                xElement = XElement.Parse(line.Trim().Substring(3));
            }
            catch
            {
                return null;
            }

            if (xElement.Name != "reference")
            {
                return null;
            }

            if (xElement.Attribute("path") == null)
            {
                return null;
            }

            return Path.GetFileNameWithoutExtension(xElement.Attribute("path").Value);
        }

        private static bool IsAReferenceInFolderButNotSubfolder(string line, string sourceFolder)
        {
            XElement xElement = null;
            
            if (!line.Trim().StartsWith("///"))
            {
                return false;
            }
            
            try
            {
                xElement = XElement.Parse(line.Trim().Substring(3));
            }
            catch
            {
                return false;
            }

            if (xElement.Name != "reference")
            {
                return false;
            }

            if (xElement.Attribute("path") == null)
            {
                return false;
            }

            var path = xElement.Attribute("path").Value.Replace('/', '\\');
            var subFolder = string.Format(@"{0}\", sourceFolder);

            if (!(path.StartsWith(subFolder)))
            {
                return false;
            }

            return !path.Substring(subFolder.Length).Contains(@"\");
        }
    }
}
