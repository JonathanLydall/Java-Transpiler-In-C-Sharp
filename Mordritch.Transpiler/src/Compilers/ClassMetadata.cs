using Mordritch.Transpiler.Contracts;
using Mordritch.Transpiler.src.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.src.Compilers
{
    public static class ClassMetadata
    {
        private static IList<JavaClass> _classes = new List<JavaClass>();

        private const string PATH_TO_XML_FILES = @"D:\Users\Jonathan Lydall\Documents\Visual Studio 2012\Projects\Transpiler\Mordritch.Transpiler\Resources\NeedsExtending";

        static ClassMetadata()
        {
            var directoryInfo = new DirectoryInfo(PATH_TO_XML_FILES);
            var xmlFiles = directoryInfo.GetFiles("*.xml");
            
            foreach (var xmlFile in xmlFiles)
            {
                try
                {
                    _classes.Add(SerializationHelper.Deserialize<JavaClass>(File.ReadAllText(xmlFile.FullName)));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not deserialize file {0}:\n{1}", xmlFile.Name, e.Message);
                }
            }
        }

        public static bool ClassNeedsExtending(string className)
        {
            return _classes.Any(c => c.Name == className && c.Methods.Any(m => m.Action == MethodAction.Extend));
        }

        public static bool MethodNeedsExtending(string className, string methodName)
        {
            return _classes.Any(c => c.Name == className && c.Methods.Any(m => m.Name == methodName && m.Action == MethodAction.Extend));
        }

        public static bool MethodNeedsExclusion(string className, string methodName)
        {
            return _classes.Any(c => c.Name == className && c.Methods.Any(m => m.Name == methodName && m.Action == MethodAction.Exclude));
        }

        public static bool MethodNeedsExclusion(IList<string> classInhertanceStack, string methodName)
        {
            return classInhertanceStack.Any(className => MethodNeedsExclusion(className, methodName));
        }

        public static bool MethodNeedsBodyOnlyExclusion(string className, string methodName)
        {
            return _classes.Any(c => c.Name == className && c.Methods.Any(m => m.Name == methodName && m.Action == MethodAction.ExcludeBodyOnly));
        }

        public static bool FieldNeedsExcluding(string className, string fieldName)
        {
            return _classes.Any(c => c.Name == className && c.ExcludedFields.Any(f => f.Name == fieldName));
        }

        public static IList<string> GetClassesNeedingExtending()
        {
            return _classes.Where(c => c.Methods.Any(m => m.Action == MethodAction.Extend)).Select(x => x.Name).ToList();
        }

        public static IList<string> GetMethodDependancies(string className, string methodName)
        {
            var classMatchingName = _classes.FirstOrDefault(x => x.Name == className);

            if (classMatchingName == null)
            {
                return null;
            }

            var dependantMethods = classMatchingName.Methods
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

        public static MethodDetail GetMethodDetails(string className, string methodName)
        {
            var classDetails = _classes.FirstOrDefault(x => x.Name == className);
            if (classDetails == null)
            {
                return null;
            }

            if (classDetails.Methods == null)
            {
                return null;
            }

            return classDetails.Methods.FirstOrDefault(x => x.Name == methodName);
        }

        public static string GetMethodComment(string className, string methodName)
        {
            var methodDetails = GetMethodDetails(className, methodName);
            if (methodDetails == null)
            {
                return null;
            }

            return methodDetails.Comments;
        }

        public static string GetMethodComment(IList<string> classInheritanceStack, string methodName)
        {
            return classInheritanceStack
                .Where(x => GetMethodComment(x, methodName) != null)
                .Select(x => GetMethodComment(x, methodName))
                .FirstOrDefault();
        }

        public static string GetFieldComment(string className, string fieldName)
        {
            var classDetails = _classes.FirstOrDefault(x => x.Name == className);
            if (classDetails == null)
            {
                return null;
            }

            if (classDetails.ExcludedFields == null)
            {
                return null;
            }

            var fieldDetails = classDetails.ExcludedFields.FirstOrDefault(x => x.Name == fieldName);
            if (fieldDetails == null)
            {
                return null;
            }

            return fieldDetails.Comments;
        }
    }
}
