using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Mordritch.Transpiler.Utilities
{
    public class TypeScriptProject
    {
        public static void GenerateTypeScriptReferences(IList<string> files, string projectFile, string sourceFolder, bool needsExtending)
        {
            var orginalProjectRootElement = XElement.Load(projectFile);
            var projectRootElement = XElement.Load(projectFile);
            var itemGroupElement = projectRootElement.Descendants().FirstOrDefault(d => d.Name.LocalName == "ItemGroup" && d.Elements().Any(e => e.Name.LocalName == "TypeScriptCompile"));
            var fileRemoved = RemoveStaleReferences(itemGroupElement, sourceFolder, files, needsExtending);
            var fileAdded = AddReferences(files, itemGroupElement, sourceFolder, needsExtending);

            if (!fileRemoved && !fileAdded)
            {
                Console.WriteLine("No changes detected to .proj file.");
                return;
            }

            File.WriteAllText(projectFile, projectRootElement.ToString());
        }

        private static bool AddReferences(IList<string> files, XElement itemGroupElement, string sourceFolder, bool needsExtending)
        {
            var xmlNamespace = itemGroupElement.GetDefaultNamespace();
            var fileAdded = false;
            
            if (needsExtending)
            {
                files = files.Select(x => string.Format("{0}{1}", x, Program.NEEDS_EXTENDING_EXTENSION)).ToList();
            }

            foreach (var file in files)
            {
                var hasJs = true;
                //var hasJs = itemGroupElement
                //        .Descendants()
                //        .Any(descendant =>
                //            descendant.Attribute("Include") != null &&
                //            string.Format(@"{0}\{1}.js", sourceFolder, fileName) == descendant.Attribute("Include").Value);
                
                var hasTs = itemGroupElement
                        .Descendants()
                        .Any(descendant =>
                            descendant.Attribute("Include") != null &&
                            string.Format(@"{0}\{1}.ts", sourceFolder, file) == descendant.Attribute("Include").Value);

                if (hasJs && hasTs)
                {
                    continue;
                }

                fileAdded = true;

                //XElement dependentUponElement;
                //dependentUponElement = new XElement(xmlNamespace + "DependentUpon");
                //dependentUponElement.SetValue(string.Format(@"{0}.ts", file));

                //XElement contentElement;
                //contentElement = new XElement(xmlNamespace + "Content");
                //contentElement.SetAttributeValue("Include", string.Format(@"{0}\{1}.js", sourceFolder, fileName));
                //contentElement.Add(dependentUponElement);

                XElement typeScriptCompileElement;
                typeScriptCompileElement = new XElement(xmlNamespace + "TypeScriptCompile");
                typeScriptCompileElement.SetAttributeValue("Include", string.Format(@"{0}\{1}.ts", sourceFolder, file));

                itemGroupElement.Add(typeScriptCompileElement);
                //itemGroupElement.Add(contentElement);
            }

            return fileAdded;
        }

        private static bool RemoveStaleReferences(XElement itemGroupElement, string sourceFolder, IList<string> files, bool needsExtending)
        {
            if (needsExtending)
            {
                files = files.Select(x => string.Format("{0}{1}", x, Program.NEEDS_EXTENDING_EXTENSION)).ToList();
            }

            var itemsToRemove = itemGroupElement
                .Descendants()
                .Where(descendant =>
                    descendant.Attribute("Include") != null &&
                    IsInFolderButNotSubfolder(descendant.Attribute("Include").Value, string.Format(@"{0}\", sourceFolder)) &&
                    files.All(file => string.Format(@"{0}\{1}.ts", sourceFolder, file) != descendant.Attribute("Include").Value) &&
                    files.All(file => string.Format(@"{0}\{1}.js", sourceFolder, file) != descendant.Attribute("Include").Value))
                .ToList();

            if (itemsToRemove.Count == 0)
            {
                return false;
            }

            itemsToRemove.Remove();
            return true;
        }

        private static bool IsInFolderButNotSubfolder(string path, string subFolder)
        {
            if (!(path.StartsWith(subFolder)))
            {
                return false;
            }

            return !path.Substring(subFolder.Length).Contains(@"\");
        }
    }
}
