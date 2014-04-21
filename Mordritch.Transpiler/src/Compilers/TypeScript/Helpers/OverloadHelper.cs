using Mordritch.Transpiler.Java.AstGenerator.Declarations;
using Mordritch.Transpiler.Java.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.Helpers
{
    public static class OverloadHelper
    {
        public static string GetDispatcherParameters(IList<MethodDeclaration> methodDeclarations, string argumentPrefix)
        {
            var argumentNumber = 0;

            if (methodDeclarations.Count == 0 || methodDeclarations.Max(x => x.Arguments.Count) == 0)
            {
                return string.Format("{0}{1}?: any", argumentPrefix, argumentNumber);
            }

            return methodDeclarations
                .First(x => x.Arguments.Count == methodDeclarations.Max(y => y.Arguments.Count))
                .Arguments
                .Select(x => string.Format("{0}{1}?: any", argumentPrefix, argumentNumber++))
                .Aggregate((x, y) => x + ", " + y);
        }

        public static void BuildDispatcher(ICompiler compiler, IList<MethodDeclaration> methodDeclarations, string dispatcherMethodName, string returnType, string targetMethodName)
        {
            var constructorNumber = 0;
            var argumentPrefix = "c_par";

            // TODO: Hardcoded as public, however, not sure right now what to do if signatures differ.
            var accessModifier = "public ";
            var isStatic = methodDeclarations.Any(x => x.Modifiers != null && x.Modifiers.Any(y => y.Data == Keywords.Static)) ? "static " : string.Empty;

            compiler.AddLine(string.Format("{0}{1}{2}({3}): {4} {{", accessModifier, isStatic, dispatcherMethodName, OverloadHelper.GetDispatcherParameters(methodDeclarations, argumentPrefix), returnType));
            compiler.IncreaseIndentation();
            {
                foreach (var constructor in methodDeclarations)
                {
                    compiler.AddLine("if (");
                    compiler.IncreaseIndentation();
                    {
                        var parametersAreDefined = GetParametersAreDefined(methodDeclarations, constructor, argumentPrefix);
                        var parametersAreOfRightType = GetParametersAreOfRightType(constructor, argumentPrefix);

                        if (string.IsNullOrEmpty(parametersAreOfRightType))
                        {
                            compiler.AddLine(parametersAreDefined);
                        }
                        else
                        {
                            compiler.AddLine(string.Format("{0} &&", parametersAreDefined));
                            compiler.AddLine(parametersAreOfRightType);
                        }
                    }
                    compiler.DecreaseIndentation();

                    compiler.AddLine(") {");

                    compiler.IncreaseIndentation();
                    {
                        var parNumber = 0;
                        var arguments = constructor.Arguments.Count != 0
                            ? constructor.Arguments
                                .Select(x => string.Format("<{0}{1}>{2}{3}", compiler.GetTypeString(x.Type, "GetConstructorCallCastType"), GetArrayDepth(x.ArrayDepth), argumentPrefix, parNumber++))
                                .Aggregate((x, y) => x + ", " + y)
                            : string.Empty;

                        compiler.AddLine(string.Format("this.{0}{1}({2});", targetMethodName, constructorNumber++, arguments));
                    }
                    compiler.DecreaseIndentation();

                    compiler.AddLine("}");
                    compiler.AddBlankLine();
                }
            }

            if (methodDeclarations.Count > 0)
            {
                compiler.AddLine(@"throw new Error(""Unrecognized paramaters called."")");
            }

            compiler.DecreaseIndentation();
            compiler.AddLine("}");
            compiler.AddBlankLine();
        }

        private static string GetArrayDepth(int depth)
        {
            var returnString = string.Empty;
            for (var i = 0; i < depth; i++)
            {
                returnString += "[]";
            }

            return returnString;
        }

        private static string GetParametersAreOfRightType(MethodDeclaration constructor, string argumentPrefix)
        {
            var isOfRightTypeNumber = 0;
            return constructor.Arguments.Count == 0
                ? string.Empty
                : constructor.Arguments
                    .Select(x => GetTypeCheck(x, isOfRightTypeNumber++, argumentPrefix))
                    .Aggregate((x, y) => x + " && " + y);
        }

        private static string GetTypeCheck(MethodArgument methodArgument, int count, string argumentPrefix)
        {
            if (methodArgument.ArrayDepth > 0)
            {
                return string.Format("({0}{1} instanceof Array)", argumentPrefix, count);
            }

            var type = PrimitiveMapper.IsTypeOfMap(methodArgument.Type.Data);
            if (type != null)
            {
                return string.Format(@"(typeof {0}{1} == ""{2}"")", argumentPrefix, count, type);
            }

            return string.Format(@"(typeof {0}{1} == ""function"" || typeof {0}{1} == ""object"")", argumentPrefix, count);

        }

        private static string GetParametersAreDefined(IList<MethodDeclaration> methodDeclarations, MethodDeclaration constructor, string argumentPrefix)
        {
            var isDefinedNumber = 0;

            return methodDeclarations
                .First(x => x.Arguments.Count == methodDeclarations.Max(y => y.Arguments.Count))
                .Arguments
                .Select(x => string.Format(@"(typeof {0}{1} {2} ""undefined"")", argumentPrefix, isDefinedNumber, isDefinedNumber++ < constructor.Arguments.Count ? "!=" : "=="))
                .Aggregate((x, y) => x + " && " + y);
        }
    }
}
