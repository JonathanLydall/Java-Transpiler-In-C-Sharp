﻿using Mordritch.Transpiler.Java.AstGenerator;
using Mordritch.Transpiler.Java.AstGenerator.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mordritch.Transpiler.src.Compilers;

namespace Mordritch.Transpiler.Compilers.TypeScript.AstNodeCompilers
{
    public class ReturnStatementCompiler
    {
        private ICompiler _compiler;

        private ReturnStatement _returnStatement;

        public ReturnStatementCompiler(ICompiler compiler, ReturnStatement returnStatement)
        {
            _compiler = compiler;
            _returnStatement = returnStatement;
        }

        public void Compile()
        {
            if (_returnStatement.ReturnValue.Count == 0)
            {
                _compiler.AddLine("return;");
                return;
            }

            var list = _compiler.ProcessToInnerExpressionItemList(_returnStatement.ReturnValue);

            var returnExpressions = list
                .Where(x => x.Processed)
                .Select(x => x.Output)
                .Aggregate((x, y) => x + " " + y);

            _compiler.AddLine(string.Format("return {0};", returnExpressions));
        }
    }
}
