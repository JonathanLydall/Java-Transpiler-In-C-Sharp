using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.AstGenerator.Types
{
    public class ClassType : AstNode
    {
        private bool _accessModifierPublic = false;
        
        public bool ModifierAbstract = false;

        public bool ModifierStatic = false;

        public bool ModifierFinal = false;

        public bool ModifierStrictfp = false;

        public bool AccessModifierPrivate = true;

        public bool AccessModifierProtected = false;

        public bool AccessModifierPublic
        {
            get
            {
                return _accessModifierPublic;
            }

            set
            {
                if (value)
                {
                    AccessModifierPrivate = false;
                }
                _accessModifierPublic = value;
            }
        }

        public string Name = "";

        public string Extends = null;

        public IList<string> Implements = new List<string>();

        public IList<IAstNode> Fields = new List<IAstNode>(); // TODO: Change from IAstNode to something more specific

        public IList<IAstNode> Methods = new List<IAstNode>(); // TODO: Change from IAstNode to something more specific

        public IList<IAstNode> Constructors = new List<IAstNode>(); // TODO: Change from IAstNode to something more specific

        public IAstNode Initializer = null; // TODO: Change from IAstNode to something more specific

        public IAstNode StaticInitializer = null; // TODO: Change from IAstNode to something more specific
        
        public IList<IAstNode> Body = new List<IAstNode>();

        public override string DebugOut()
        {
            var accessModifiers = 
                (AccessModifierPublic ? "public" : "private") +
                (AccessModifierProtected ? " protected" : string.Empty);

            var modifiers = string.Empty;
            modifiers += ModifierAbstract ? "abstract " : string.Empty;
            modifiers += ModifierStatic ? "static " : string.Empty;
            modifiers += ModifierFinal ? "final " : string.Empty;
            modifiers += ModifierStrictfp ? "strictfp " : string.Empty;

            var implements = Implements.Count == 0 ? string.Empty : " implements " + Implements.Aggregate((x, y) => x + ", " + y);

            var extends = string.IsNullOrEmpty(Extends) ? string.Empty : " extends " + Extends;

            return string.Format("{0} {1}class {2}{3}{4} {{...", accessModifiers, modifiers, Name, extends, implements);
        }
    }
}
