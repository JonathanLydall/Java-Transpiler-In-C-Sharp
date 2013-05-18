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
        public static void GenerateTypeScriptReferences(IList<string> files, string projectFile, string sourceFolder)
        {
            var projectRootElement = XElement.Load(projectFile);
            var itemGroupElement = projectRootElement.Descendants().FirstOrDefault(d => d.Name.LocalName == "ItemGroup" && d.Elements().Any(e => e.Name.LocalName == "TypeScriptCompile"));
            RemoveStaleReferences(itemGroupElement, sourceFolder);
            AddReferences(files, itemGroupElement, sourceFolder);

            if (File.ReadAllText(projectFile) != projectRootElement.ToString())
            {
                File.WriteAllText(projectFile, projectRootElement.ToString());
            }
        }

        private static void AddReferences(IList<string> files, XElement itemGroupElement, string sourceFolder)
        {
            var xmlNamespace = itemGroupElement.GetDefaultNamespace();

            foreach (var file in files)
            {
                XElement dependentUponElement;
                dependentUponElement = new XElement(xmlNamespace + "DependentUpon");
                dependentUponElement.SetValue(string.Format(@"{0}.ts", file));

                XElement contentElement;
                contentElement = new XElement(xmlNamespace + "Content");
                contentElement.SetAttributeValue("Include", string.Format(@"{0}\{1}.js", sourceFolder, file));
                contentElement.Add(dependentUponElement);

                XElement typeScriptCompileElement;
                typeScriptCompileElement = new XElement(xmlNamespace + "TypeScriptCompile");
                typeScriptCompileElement.SetAttributeValue("Include", string.Format(@"{0}\{1}.ts", sourceFolder, file));

                itemGroupElement.Add(typeScriptCompileElement);
                itemGroupElement.Add(contentElement);
            }
        }

        private static void RemoveStaleReferences(XElement itemGroupElement, string sourceFolder)
        {
            itemGroupElement
                .Descendants()
                .Where(descendant => 
                    descendant.Attribute("Include") != null &&
                    descendant.Attribute("Include").Value.StartsWith(string.Format(@"{0}\", sourceFolder)))
                .Remove();
        }
    }
}
