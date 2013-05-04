using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Mordritch.Transpiler.Java.Tokenizer.InputElements;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.InputElementTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.LiteralTypes;
using Mordritch.Transpiler.Java.Tokenizer.InputElements.TokenTypes;

namespace Mordritch.Transpiler.Java.Tokenizer
{
    public class Tokenizer
    {
        private string[] _data;

        private int _line;

        private int _column;

        private string _sourceName;

        public Tokenizer(string source)
        {
            var fileInfo = new FileInfo(source);
            _sourceName = fileInfo.Name;
            
            var fileContents = new StreamReader(source).ReadToEnd().ToCharArray();
            fileContents = ParseEscapedUnicodeSequences(fileContents);
            _data = SplitIntoLines(fileContents);
        }

        public Tokenizer(string source, string sourceName)
        {
            _sourceName = sourceName;

            var fileContents = ParseEscapedUnicodeSequences(source.ToCharArray());
            _data = SplitIntoLines(fileContents);
        }

        public 

        static string[] SplitIntoLines(char[] data)
        {
            return SplitIntoLines(new string(data));
        }

        static string[] SplitIntoLines(string dataString)
        {
            do
            {
                dataString = dataString.Replace("\r\n", "\r");
            }
            while (dataString.Contains("\r\n"));
            return dataString.Split(Convert.ToChar("\r"));
        }
        
        static bool IsHexCharacter(char ch)
        {
            return
                char.IsNumber(ch) ||
                char.ToLower(ch) == Convert.ToChar("a") ||
                char.ToLower(ch) == Convert.ToChar("b") ||
                char.ToLower(ch) == Convert.ToChar("c") ||
                char.ToLower(ch) == Convert.ToChar("d") ||
                char.ToLower(ch) == Convert.ToChar("e") ||
                char.ToLower(ch) == Convert.ToChar("f");
        }

        static char[] ParseEscapedUnicodeSequences(char[] data)
        {
            char backslash = Convert.ToChar(@"\");
            char u = Convert.ToChar("u");
            var maxLen = data.Length;

            var newData = new List<char>();

            for (var i = 0; i < maxLen; i++)
            {
                var currentChar = data[i];
                var previousCharacter = i == 0 ? '\u0000' : data[i - 1];
                var forwardChar1 = i + 1 < maxLen ? data[i + 1] : '\u0000';
                var forwardChar2 = i + 2 < maxLen ? data[i + 2] : '\u0000';
                var forwardChar3 = i + 3 < maxLen ? data[i + 3] : '\u0000';
                var forwardChar4 = i + 4 < maxLen ? data[i + 4] : '\u0000';
                var forwardChar5 = i + 5 < maxLen ? data[i + 5] : '\u0000';

                if (currentChar == backslash &&
                    previousCharacter != backslash &&
                    forwardChar1 == u &&
                    IsHexCharacter(forwardChar2) &&
                    IsHexCharacter(forwardChar3) &&
                    IsHexCharacter(forwardChar4) &&
                    IsHexCharacter(forwardChar5))
                {
                    newData.Add(
                        Convert.ToChar(
                            string.Format(@"\u{0}{1}{2}{3}",
                                Convert.ToString(forwardChar2),
                                Convert.ToString(forwardChar3),
                                Convert.ToString(forwardChar4),
                                Convert.ToString(forwardChar5))));
                    i += 5;
                }
                else
                {
                    newData.Add(currentChar);
                }
            }

            return newData.ToArray();
        }

        private bool Eof
        {
            get { return _line >= _data.Length || (_line == _data.Length -1 && _column >= _data[_line].Length); }
        }

        private bool Eol
        {
            get { return !Eof && _column >= _data[_line].Length; }
        }

        private void MoveToNextChar(int count = 1)
        {
            for (var i = 1; i <= count; i++)
            {
                _column++;

                while (Eol)
                {
                    _line += 1;
                    _column = 0;
                }
            }
        }

        private void MoveToPreviousChar(int count = 1)
        {
            for (var i = 1; i <= count; i++)
            {
                _column--;

                while (_column < 0 || _data[_line].Length == 0)
                {
                    _line = _line == 0 ? 0 : _line - 1;
                    _column = _data[_line].Length - 1;
                }
            }
        }

        private char ReadCharAtCurrentPosition()
        {
            return Convert.ToChar(_data[_line].Substring(_column, 1));
        }

        private char ReadChar(int offset = 0)
        {
            for (var i = 0; i < offset; i++)
            {
                MoveToNextChar();
            }

            var returnChar = Eof ? '\u0000' : ReadCharAtCurrentPosition();

            for (var i = 0; i < offset; i++)
            {
                MoveToPreviousChar();
            }

            return returnChar;
        }

        public IList<IInputElement> GetInputElements()
        {
            var inputElements = new List<IInputElement>();

            while (!Eof)
            {
                var line = _line;
                var column = _column;
                
                if (InputElementClassifier.IsStartForWhiteSpace(ReadChar()))
                {
                    var inputElement = GetWhiteSpace();
                    inputElement.Line = line;
                    inputElement.Column = column;
                    inputElement.Source = _sourceName;
                    inputElements.Add(inputElement);
                    continue;
                }

                if (InputElementClassifier.IsStartForComment(ReadChar(0), ReadChar(1)))
                {
                    var inputElement = GetComment();
                    inputElement.Line = line;
                    inputElement.Column = column;
                    inputElement.Source = _sourceName;
                    inputElements.Add(inputElement);
                    continue;
                }

                if (InputElementClassifier.IsStartForToken(ReadChar()))
                {
                    var inputElement = GetToken();
                    inputElement.Line = line;
                    inputElement.Column = column;
                    inputElement.Source = _sourceName;
                    inputElements.Add(inputElement);
                    continue;
                }

                throw new Exception("Unknown character type.");
            }

            return inputElements;
        }

        private WhiteSpaceInputElement GetWhiteSpace()
        {
            while (!Eof && char.IsWhiteSpace(ReadChar()))
            {
                MoveToNextChar();
            }

            return new WhiteSpaceInputElement();
        }

        private CommentInputElement GetComment()
        {
            var commentInputElement = new CommentInputElement();
            
            commentInputElement.IsMultilineComment = ReadChar(1) == Convert.ToChar("*");
            MoveToNextChar();

            if (commentInputElement.IsMultilineComment)
            {
                var line = _line;
                MoveToNextChar(2);
                while (!Eof && !(ReadChar() == Convert.ToChar("*") && ReadChar(1) == Convert.ToChar("/")))
                {
                    if (_line != line)
                    {
                        commentInputElement.Data += Environment.NewLine;
                        line = _line;
                    }
                    commentInputElement.Data += new String(ReadChar(), 1);
                    MoveToNextChar();
                }
                MoveToNextChar(2);


                var splitByLine = SplitIntoLines(commentInputElement.Data);

                for (var i = 0; i < splitByLine.Length; i++)
                {
                    splitByLine[i] = splitByLine[i].Trim();
                    while (splitByLine[i].StartsWith("*"))
                    {
                        splitByLine[i] = splitByLine[i].Substring(1);
                        splitByLine[i] = splitByLine[i].Trim();
                    }
                }

                var lineList = splitByLine.ToList();

                while (lineList.First() == string.Empty)
                {
                    lineList.Remove(lineList.First());
                }

                while (lineList.Last() == string.Empty)
                {
                    lineList.Remove(lineList.Last());
                }

                commentInputElement.Data = lineList.Aggregate((workingString, next) => workingString + Environment.NewLine + next);
            }
            else
            {
                MoveToNextChar(2);
                while (_column != 0)
                {
                    commentInputElement.Data += new String(ReadChar(), 1);
                    MoveToNextChar();
                }
            }

            return commentInputElement;
        }

        private IToken GetToken()
        {
            var ch = ReadChar();
            
            if (InputElementClassifier.IsJavaIdentifierPart(ch))
            {
                return GetCharacterSequenceToken();
            }

            if (InputElementClassifier.IsOperatorTokenStart(ch))
            {
                var operatorToken = new OperatorToken();
                operatorToken.Data = GetOperatorToken();
                return operatorToken;
            }

            if (InputElementClassifier.IsSeperatorTokenStart(ch))
            {
                var seperatorToken = new SeperatorToken();
                seperatorToken.Data = new String(ch, 1);
                MoveToNextChar();
                return seperatorToken;
            }

            if (InputElementClassifier.IsCharacterLiteralStart(ch))
            {
                return GetCharacterLiteral();
            }

            if (InputElementClassifier.IsStringLiteralStart(ch))
            {
                return GetStringLiteral();
            }

            throw new Exception("Error");
        }

        private StringLiteral GetStringLiteral()
        {
            var stringLiteral = new StringLiteral();
            var doubleQuotesChar = Convert.ToChar("\"");
            var backslashChar = Convert.ToChar("\\");
            stringLiteral.Data = "\"";

            MoveToNextChar();
            while (ReadChar() != doubleQuotesChar)
            {
                var ch = ReadChar();
                stringLiteral.Data += new String(ch, 1);
                MoveToNextChar();
                if (ch == backslashChar)
                {
                    ch = ReadChar();
                    stringLiteral.Data += new String(ch, 1);
                    MoveToNextChar();
                }
            }
            MoveToNextChar();

            stringLiteral.Data += "\"";
            return stringLiteral;
        }

        private CharacterLiteral GetCharacterLiteral()
        {
            var characterLiteral = new CharacterLiteral();
            var singleQuoteChar = Convert.ToChar("'");
            characterLiteral.Data = "";

            MoveToNextChar();
            while (ReadChar() != singleQuoteChar)
            {
                var ch = ReadChar();
                characterLiteral.Data += new String(ch, 1);
                MoveToNextChar();
            }
            MoveToNextChar();

            return characterLiteral;
        }

        private IToken GetCharacterSequenceToken()
        {
            var tokenContents = "";

            if (Char.IsNumber(ReadChar()))
            {
                return GetNumericLiteral();
            }

            while (!Eof && InputElementClassifier.IsJavaIdentifierPart(ReadChar()))
            {
                tokenContents += new String(ReadChar(), 1);
                MoveToNextChar();
            }

            if (InputElementClassifier.Keywords.Any(k => k == tokenContents))
            {
                var keywordToken = new KeywordToken();
                keywordToken.Data = tokenContents;
                return keywordToken;
            }

            if (InputElementClassifier.NullLiteral == tokenContents)
            {
                var nullLiteral = new NullLiteral();
                nullLiteral.Data = tokenContents;
                return nullLiteral;
            }

            if (InputElementClassifier.BooleanLiterals.Any(b => b == tokenContents))
            {
                var booleanLiteral = new BooleanLiteral();
                booleanLiteral.Data = tokenContents;
                return booleanLiteral;
            }

            var identifierToken = new IdentifierToken();
            identifierToken.Data = tokenContents;
            return identifierToken;
        }

        private ILiteralToken GetNumericLiteral()
        {
            var tokenContents = "";
            var previousCh = '\u0000';
            var ch = ReadChar();
            var eChar1 = Convert.ToChar("e");
            var eChar2 = Convert.ToChar("E");
            var fullStopChar = Convert.ToChar(".");
            var plusChar = Convert.ToChar("+");
            var minusChar = Convert.ToChar("-");


            while (ch == fullStopChar || InputElementClassifier.IsJavaIdentifierPart(ch))
            {
                tokenContents += new String(ch, 1);
                previousCh = ch;
                MoveToNextChar();
                ch = ReadChar();
                if ((previousCh == eChar1 || previousCh == eChar2) && (ch == plusChar || ch == minusChar))
                {
                    tokenContents += new String(ch, 1);
                    previousCh = ch;
                    MoveToNextChar();
                    ch = ReadChar();
                }
            }

            if (tokenContents.Contains(".") ||
                tokenContents.Contains("e") ||
                tokenContents.Contains("E") ||
                tokenContents.Contains("f") ||
                tokenContents.Contains("F") ||
                tokenContents.Contains("l") ||
                tokenContents.Contains("L"))
            {
                // TODO: Improve as per spec: http://docs.oracle.com/javase/specs/jls/se7/html/jls-3.html#jls-3.10.2
                var floatingPointLiteral = new FloatingPointLiteral();
                floatingPointLiteral.Data = tokenContents;
                return floatingPointLiteral;
            }

            var integerLiteral = new IntegerLiteral();
            integerLiteral.Data = tokenContents;
            return integerLiteral;
        }

        private string GetOperatorToken()
        {
            var returnString = "";

            while (!Eof && InputElementClassifier.Operators.Any(o => o.Length == 1 && Convert.ToChar(o) == ReadChar()))
            {
                returnString += new String(ReadChar(), 1);
                MoveToNextChar();
            }
            
            return returnString;
        }
    }
}
