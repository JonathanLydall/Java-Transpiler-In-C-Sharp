using Mordritch.Transpiler.Contracts;
using Mordritch.Transpiler.src.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.src
{
    static class JavaClassMetadata
    {
        private static IList<JavaClass> _javaClasses = new List<JavaClass>();
        private class FabricatedMethodDetail : MethodDetail { }
        private class FabricatedFieldDetail : FieldDetail { }
        
        public static void Load(string path)
        {
            var xmlDirectoryInfo = new DirectoryInfo(path);
            var xmlFileInfos = xmlDirectoryInfo.GetFiles("*.xml");
            var xmlFileNames = xmlFileInfos.Select(x => Path.GetFileNameWithoutExtension(x.Name));

            _javaClasses = xmlFileNames
                .Select(x => string.Format(@"{0}\{1}.xml", path, x))
                .Select(x => SerializationHelper.Deserialize<JavaClass>(File.ReadAllText(x)))
                .ToList();
        }

        private static bool ShouldParse(this JavaClass javaClass)
        {
            return
                javaClass.Action == JavaClassCompileAction.ParseOnly ||
                javaClass.Action == JavaClassCompileAction.Compile;
        }

        private static bool ShouldCompile(this JavaClass javaClass)
        {
            return
                javaClass.Action == JavaClassCompileAction.Compile &&
                javaClass.DefaultMethodAction != MethodAction.Extend &&
                javaClass
                    .Methods
                    .All(y => y.Action != MethodAction.Extend);
        }

        public static bool NeedsExtending(this JavaClass javaClass)
        {
            return
                javaClass.Action == JavaClassCompileAction.Compile &&
                (
                   javaClass.DefaultMethodAction == MethodAction.Extend ||
                   javaClass.Methods.Any(y => y.Action == MethodAction.Extend)
                );
        }

        public static IList<JavaClass> GetFilesToParse()
        {
            return _javaClasses.Where(ShouldParse).ToList();
        }

        public static IList<JavaClass> GetFilesToTranspile()
        {
            return _javaClasses.Where(ShouldCompile).ToList();
        }

        public static IList<JavaClass> GetFilesNeedingExtending()
        {
            return _javaClasses.Where(NeedsExtending).ToList();
        }

        public static JavaClass GetClass(string className)
        {
            return _javaClasses.FirstOrDefault(x => x.Name == className);
        }

        public static MethodDetail GetMethod(this JavaClass javaClass, string methodName)
        {
            if (javaClass.Methods.All(x => x.Name != methodName))
            {
                javaClass.Methods.Add(new FabricatedMethodDetail
                {
                    Name = methodName,
                    Action = javaClass.DefaultMethodAction,
                    Comments = string.Format("Default method action for class '{0}'.", javaClass.Name),
                });
            }
            
            return javaClass.Methods.First(x => x.Name == methodName);
        }

        public static FieldDetail GetField(this JavaClass javaClass, string fieldName)
        {
            if (javaClass.ExcludedFields.All(x => x.Name != fieldName))
            {
                javaClass.ExcludedFields.Add(new FabricatedFieldDetail
                {
                    Name = fieldName,
                    Action = FieldAction.Compile
                });
            }

            return javaClass.ExcludedFields.First(x => x.Name == fieldName);
        }

        public static bool NeedsExtending(this MethodDetail methodDetail)
        {
            return methodDetail.Action == MethodAction.Extend;
        }

        public static bool NeedsExclusion(this MethodDetail methodDetail, IEnumerable<string> classInheritanceStack)
        {
            if (methodDetail.Action == MethodAction.Exclude || methodDetail.Action == MethodAction.ExcludeAndInDerivedClasses)
            {
                return true;
            }

            return classInheritanceStack
                .Select(x => JavaClassMetadata.GetClass(x))
                .Where(x => x != null)
                .Where(x => x.Methods.Any(y => y.Name == methodDetail.Name))
                .Select(x => x.Methods.FirstOrDefault(y => y.Name == methodDetail.Name))
                .Any(x => x.Action == MethodAction.ExcludeAndInDerivedClasses);
        }

        public static string GetExclusionComment(this MethodDetail methodDetail, IEnumerable<string> classInheritanceStack)
        {
            if (methodDetail.Action == MethodAction.Exclude || methodDetail.Action == MethodAction.ExcludeAndInDerivedClasses)
            {
                return methodDetail.Comments;
            }

            var parentClass = classInheritanceStack
                .Select(x => JavaClassMetadata.GetClass(x))
                .Where(x => x != null)
                .First(x => x.Methods.Any(y => y.Name == methodDetail.Name && y.Action == MethodAction.ExcludeAndInDerivedClasses));

            return string.Format("({0}) {1}", parentClass.Name, parentClass.Methods.First(x => x.Name == methodDetail.Name).Comments);
        }

        public static bool NeedsBodyOnlyExclusion(this MethodDetail methodDetail)
        {
            return methodDetail.Action == MethodAction.ExcludeBodyOnly;
        }

        public static bool NeedsExclusion(this FieldDetail fieldDetail)
        {
            return fieldDetail.Action == FieldAction.Exclude;
        }

        public static IList<string> GetDependantMethods(this MethodDetail methodDetail)
        {
            var javaClass = _javaClasses.First(x => x.Methods.Contains(methodDetail));
            
            var dependantMethods = javaClass.Methods
                .Where(x =>
                    x.DependantOn != null &&
                    x.DependantOn.Any(y => y == methodDetail.Name))
                .ToList();

            if (dependantMethods.Count == 0)
            {
                return null;
            }

            return dependantMethods
                .Select(x => x.Name)
                .ToList();
        }

        public static string GetComment(this MethodDetail methodDetail)
        {
            return methodDetail.Comments;
        }

        public static string GetComment(this MethodDetail methodDetail, IList<string> classInheritanceStack)
        {
            return classInheritanceStack
                .Select(x => JavaClassMetadata.GetClass(x))
                .Select(x => x.Methods.FirstOrDefault(y => y.Name == methodDetail.Name))
                .Where(x => x != null)
                .Select(x => x.GetComment())
                .Where(x => !string.IsNullOrEmpty(x))
                .FirstOrDefault();
        }

        public static string GetComment(this FieldDetail fieldDetail)
        {
            return fieldDetail.Comments;
        }
    }
}
