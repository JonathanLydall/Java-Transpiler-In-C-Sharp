using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TranspilerUtils
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        const string DEFAULT_XML_FILE_PATH = @"D:\Users\Jonathan Lydall\Documents\Visual Studio 2013\Projects\McSim\McSim\Metadata";
        const string DEFAULT_JAVA_FILE_PATH = @"D:\Users\Jonathan Lydall\Downloads\mcp\mcp742\src\minecraft_server\net\minecraft\src";
        const string DEFAULT_TRANSPILER_PATH = @"D:\Users\Jonathan Lydall\Documents\Visual Studio 2012\Projects\Transpiler\Mordritch.Transpiler\bin\Debug\Transpiler.exe";
        const string DEFAULT_PROJECT_FILE_PATH = @"D:\Users\Jonathan Lydall\Documents\visual studio 2013\Projects\McSim\McSim\McSim.csproj";
        const string DEFAULT_PROJECT_TRANSPILED_SUBFOLDER = @"Generated\Minecraft";
        const string DEFAULT_PROJECT_TRANSPILED_BUT_EXTENDED_SUBFOLDER = @"Generated\Minecraft\NeedsExtending";

        public static string CurrentXmlPath = DEFAULT_XML_FILE_PATH;
        public static string CurrentJavaFilesPath = DEFAULT_JAVA_FILE_PATH;
        public static string CurrentTranspilerPath = DEFAULT_TRANSPILER_PATH;
        public static string CurrentProjectFilePath = DEFAULT_PROJECT_FILE_PATH;
        public static string CurrentProjectTranspiledSubfolder = DEFAULT_PROJECT_TRANSPILED_SUBFOLDER;
        public static string CurrentProjectTranspiledButExtendedSubfolder = DEFAULT_PROJECT_TRANSPILED_BUT_EXTENDED_SUBFOLDER;

        public static string JavaFileName(string className)
        {
            return string.Format(@"{0}\{1}.java", CurrentJavaFilesPath, className);
        }

        public static bool JavaFileExists(string className)
        {
            return File.Exists(JavaFileName(className));
        }

        public static string XmlFileName(string className)
        {
            return string.Format(@"{0}\{1}.xml", CurrentXmlPath, className);
        }

        public static bool XmlFileExists(string className)
        {
            return File.Exists(XmlFileName(className));
        }
    }
}
