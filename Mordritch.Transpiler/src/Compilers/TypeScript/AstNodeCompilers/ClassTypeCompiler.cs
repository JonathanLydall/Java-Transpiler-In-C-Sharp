using Mordritch.Transpiler.Java.AstGenerator.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class ClassTypeCompiler
    {
        private ICompiler _compiler;

        private ClassType _classType;

        public ClassTypeCompiler(ICompiler compiler, ClassType classType)
        {
            _compiler = compiler;
            _classType = classType;
        }

        public void Compile()
        {
            var accessModifiers = _classType.AccessModifierPublic ? "export" : string.Empty;

            var modifiers = _classType.ModifierStatic
                ? "static "
                : string.Empty;

            var implements = _classType.Implements.Count == 0
                ? string.Empty
                : " implements " + _classType.Implements.Aggregate((x, y) => x + ", " + y);

            var extends = string.IsNullOrEmpty(_classType.Extends) ? string.Empty : " extends " + _classType.Extends;

            var name = _classType.Name;

            _compiler.AddLine(string.Format("{0} {1}class {2}{3}{4} {{", accessModifiers, modifiers, name, extends, implements));
            _compiler.IncreaseIndentation();
            {
                _compiler.CompileBody(_classType.Body);
            }
            _compiler.DecreaseIndentation();
            _compiler.AddLine("}");
            _compiler.AddBlankLine();

        }
    }
}
