using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Mordritch.Transpiler.Contracts;

namespace Mordritch.Transpiler.src.Utilities
{
    public class ClassesNeedingExtending_DELETING
    {
        private IList<JavaClass> classes = new List<JavaClass>();

        public ClassesNeedingExtending_DELETING(string pathToXmlFiles)
        {
            var directoryInfo = new DirectoryInfo(pathToXmlFiles);
            var xmlFiles = directoryInfo.GetFiles("*.xml");

            foreach (var xmlFile in xmlFiles)
            {
                try
                {
                    classes.Add(SerializationHelper.Deserialize<JavaClass>(File.ReadAllText(xmlFile.FullName)));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not deserialize file {0}:\n{1}", xmlFile.Name, e.Message);
                }
            }
        }

        public string[] GetClassNames()
        {
            return classes.Select(x => x.Name).ToArray();
        }

        public bool ClassNeedsExtending(string className)
        {
            return classes.Any(x => x.Name == className);
        }

        public bool MethodNeedsExtending(string className, string methodName)
        {
            return
                classes.FirstOrDefault(x => x.Name == className) != null &&
                classes.FirstOrDefault(x => x.Name == className)
                    .Methods.Any(x => x.Name == methodName && x.Action == MethodAction.Extend);
        }

        public IList<string> GetDependantMethods(string className, string methodName)
        {
            var classesMatchingName = classes.FirstOrDefault(x => x.Name != className);

            if (classesMatchingName == null)
            {
                return null;
            }

            var dependantMethods = classesMatchingName.Methods
                .Where(x =>
                    x.DependantOn != null &&
                    x.DependantOn.Any(y => y == methodName))
                .ToList();

            if (dependantMethods.Count == 0)
            {
                return null;
            }

            return dependantMethods
                .Select(x => x.Name)
                .ToList();
        }

        public MethodDetail GetMethodDetails(string className, string methodName)
        {
            return classes.First(x => x.Name == className).Methods.First(x => x.Name == methodName);
        }
    }
}
