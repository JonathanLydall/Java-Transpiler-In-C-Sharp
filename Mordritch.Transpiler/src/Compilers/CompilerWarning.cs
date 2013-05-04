using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Compilers
{
    public class CompilerWarning
    {
        private int _line;

        private int _column;

        private string _description;
        
        public int Line
        {
            get
            {
                return _line;
            }
        }

        public int Column
        {
            get
            {
                return _column;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
        }

        public CompilerWarning(int line, int column, string description)
        {
            _line = line;
            _column = column;
            _description = description;
        }
    }
}
