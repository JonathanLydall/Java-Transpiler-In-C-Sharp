using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace Mordritch.Transpiler.Compilers
{
    public class Excluder_DELETING
    {
        private static IList<Fields> _contents;

        private static IList<Fields> _bodyOnlyContents;

        public struct Fields
        {
            public string ClassName;

            public string MethodName;

            public string Comment;
        }

        public static bool IsExcluding = false;

        public static IList<Fields> Contents
        {
            get
            {
                if (_contents == null)
                {
                    _contents = new List<Fields>();

                    var stream = new FileStream(@"..\..\Resources\Exclusions.csv", FileMode.Open);
                    var parser = new TextFieldParser(stream);
                    parser.TextFieldType = FieldType.Delimited;
                    parser.Delimiters = new[] { "\t" };
                    while (!parser.EndOfData)
                    {
                        var rowContents = parser.ReadFields();
                        _contents.Add(new Fields
                        {
                            ClassName = rowContents[0],
                            MethodName = rowContents[1],
                            Comment = rowContents.Length > 2 ? rowContents[2] : string.Empty
                        });
                    }
                }

                return _contents;
            }

            set
            {
                _contents = value;
            }
        }

        public static IList<Fields> BodyOnlyContents
        {
            get
            {
                if (_bodyOnlyContents == null)
                {
                    _bodyOnlyContents = new List<Fields>();

                    var stream = new FileStream(@"..\..\Resources\ExcludeBodyOnly.csv", FileMode.Open);
                    var parser = new TextFieldParser(stream);
                    parser.TextFieldType = FieldType.Delimited;
                    parser.Delimiters = new[] { "\t" };
                    while (!parser.EndOfData)
                    {
                        var rowContents = parser.ReadFields();
                        _bodyOnlyContents.Add(new Fields
                        {
                            ClassName = rowContents[0],
                            MethodName = rowContents[1],
                            Comment = rowContents[2]
                        });
                    }
                }

                return _bodyOnlyContents;
            }

            set
            {
                _bodyOnlyContents = value;
            }
        }

        public static string ShouldExclude(string className, string methodName)
        {
            var exclusionEntry = Contents
                .Where(x => className.StartsWith(x.ClassName) && x.MethodName == methodName).ToList();

            return exclusionEntry.Count == 0 ? null : exclusionEntry.First().Comment;
        }

        public static string ShouldExcludeBody(string className, string methodName)
        {
            var exclusionEntry = BodyOnlyContents
                .Where(x => className.StartsWith(x.ClassName) && x.MethodName == methodName).ToList();

            return exclusionEntry.Count == 0 ? null : exclusionEntry.First().Comment;
        }
    }
}
