using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mordritch.Transpiler.Contracts
{
    public enum MethodAction
    {
        [Description("Compile")]
        Compile,

        [Description("Exclude")]
        Exclude,

        [Description("Exclude (And in Derived Classes)")]
        ExcludeAndInDerivedClasses,

        [Description("Exclude (Body Only)")]
        ExcludeBodyOnly,

        [Description("Extend")]
        Extend,

        [Description("Generate Signature")]
        GenerateSignature,
    }

    public enum FieldAction
    {
        [Description("Exclude")]
        Exclude = 0,

        [Description("Compile")]
        Compile = 1,
    }

    public enum JavaClassCompileAction
    {
        [Description("Compile")]
        Compile,

        [Description("Ignore")]
        Ignore,

        [Description("Parse Only")]
        ParseOnly
    }

    public class JavaClass
    {
        public string Name { get; set; }

        public string Comments { get; set; }

        public JavaClassCompileAction Action { get; set; }

        public MethodAction DefaultMethodAction { get; set; }

        public FieldAction DefaultFieldAction { get; set; }

        public List<MethodDetail> Methods { get; set; }

        public List<FieldDetail> Fields { get; set; }
    }

    public class MethodDetail
    {
        public string Name { get; set; }

        public string Comments { get; set; }

        public MethodAction Action { get; set; }

        public List<string> DependantOn { get; set; }
    }

    public class FieldDetail
    {
        public string Name { get; set; }

        public string Comments { get; set; }

        public FieldAction Action { get; set; }
    }
}
