using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mordritch.Transpiler.Java.Common
{
    // http://docs.oracle.com/javase/7/docs/api/java/lang/package-summary.html
    public static class JavaLangPackages
    {
        // Interfaces:
        public static string Appendable = "Appendable";
        public static string AutoCloseable = "AutoCloseable";
        public static string CharSequence = "CharSequence";
        public static string Cloneable = "Cloneable";
        public static string Comparable = "Comparable";
        public static string Iterable = "Iterable";
        public static string Readable = "Readable";
        public static string Runnable = "Runnable";
        //public static string Thread.UncaughtExceptionHandler = "Thread.UncaughtExceptionHandler";

        // Classes:
        public static string Boolean = "Boolean";
        public static string Byte = "Byte";
        public static string Character = "Character";
        //public static string Character.Subset = "Character.Subset";
        //public static string Character.UnicodeBlock = "Character.UnicodeBlock";
        public static string Class = "Class";
        public static string ClassLoader = "ClassLoader";
        public static string ClassValue = "ClassValue";
        public static string Compiler = "Compiler";
        public static string Double = "Double";
        public static string Enum = "Enum";
        public static string Float = "Float";
        public static string InheritableThreadLocal = "InheritableThreadLocal";
        public static string Integer = "Integer";
        public static string Long = "Long";
        //public static string Math = "Math"; //Going to try get away with using JavaScript's
        public static string Number = "Number";
        public static string Object = "Object";
        public static string Package = "Package";
        public static string Process = "Process";
        public static string ProcessBuilder = "ProcessBuilder";
        //public static string ProcessBuilder.Redirect = "ProcessBuilder.Redirect";
        public static string Runtime = "Runtime";
        public static string RuntimePermission = "RuntimePermission";
        public static string SecurityManager = "SecurityManager";
        public static string Short = "Short";
        public static string StackTraceElement = "StackTraceElement";
        public static string StrictMath = "StrictMath";
        public static string String = "String";
        public static string StringBuffer = "StringBuffer";
        public static string StringBuilder = "StringBuilder";
        public static string System = "System";
        public static string Thread = "Thread";
        public static string ThreadGroup = "ThreadGroup";
        public static string ThreadLocal = "ThreadLocal";
        public static string Throwable = "Throwable";
        public static string Void = "Void";

        // Enums:
        //public static string Character.UnicodeScript = "Character.UnicodeScript";
        //public static string ProcessBuilder.Redirect.Type = "ProcessBuilder.Redirect.Type";
        //public static string Thread.State = "Thread.State";

        // Exceptions:
        //public static string ArithmeticException = "ArithmeticException";
        //public static string ArrayIndexOutOfBoundsException = "ArrayIndexOutOfBoundsException";
        //public static string ArrayStoreException = "ArrayStoreException";
        //public static string ClassCastException = "ClassCastException";
        //public static string ClassNotFoundException = "ClassNotFoundException";
        //public static string CloneNotSupportedException = "CloneNotSupportedException";
        //public static string EnumConstantNotPresentException = "EnumConstantNotPresentException";
        //public static string Exception = "Exception";
        //public static string IllegalAccessException = "IllegalAccessException";
        //public static string IllegalArgumentException = "IllegalArgumentException";
        //public static string IllegalMonitorStateException = "IllegalMonitorStateException";
        //public static string IllegalStateException = "IllegalStateException";
        //public static string IllegalThreadStateException = "IllegalThreadStateException";
        //public static string IndexOutOfBoundsException = "IndexOutOfBoundsException";
        //public static string InstantiationException = "InstantiationException";
        //public static string InterruptedException = "InterruptedException";
        //public static string NegativeArraySizeException = "NegativeArraySizeException";
        //public static string NoSuchFieldException = "NoSuchFieldException";
        //public static string NoSuchMethodException = "NoSuchMethodException";
        //public static string NullPointerException = "NullPointerException";
        //public static string NumberFormatException = "NumberFormatException";
        //public static string ReflectiveOperationException = "ReflectiveOperationException";
        //public static string RuntimeException = "RuntimeException";
        //public static string SecurityException = "SecurityException";
        //public static string StringIndexOutOfBoundsException = "StringIndexOutOfBoundsException";
        //public static string TypeNotPresentException = "TypeNotPresentException";
        //public static string UnsupportedOperationException = "UnsupportedOperationException";

        // Errors:
        //public static string AbstractMethodError = "AbstractMethodError";
        //public static string AssertionError = "AssertionError";
        //public static string BootstrapMethodError = "BootstrapMethodError";
        //public static string ClassCircularityError = "ClassCircularityError";
        //public static string ClassFormatError = "ClassFormatError";
        //public static string Error = "Error";
        //public static string ExceptionInInitializerError = "ExceptionInInitializerError";
        //public static string IllegalAccessError = "IllegalAccessError";
        //public static string IncompatibleClassChangeError = "IncompatibleClassChangeError";
        //public static string InstantiationError = "InstantiationError";
        //public static string InternalError = "InternalError";
        //public static string LinkageError = "LinkageError";
        //public static string NoClassDefFoundError = "NoClassDefFoundError";
        //public static string NoSuchFieldError = "NoSuchFieldError";
        //public static string NoSuchMethodError = "NoSuchMethodError";
        //public static string OutOfMemoryError = "OutOfMemoryError";
        //public static string StackOverflowError = "StackOverflowError";
        //public static string ThreadDeath = "ThreadDeath";
        //public static string UnknownError = "UnknownError";
        //public static string UnsatisfiedLinkError = "UnsatisfiedLinkError";
        //public static string UnsupportedClassVersionError = "UnsupportedClassVersionError";
        //public static string VerifyError = "VerifyError";
        //public static string VirtualMachineError = "VirtualMachineError";

        public static string[] Interfaces =
        {
            Appendable,
            AutoCloseable,
            CharSequence,
            Cloneable,
            Comparable,
            Iterable,
            Readable,
            Runnable,
            //Thread.UncaughtExceptionHandler,
        };

        public static string[] Classes =
        {
            Boolean,
            Byte,
            Character,
            //Character.Subset,  "Character.Subset";
            //Character.UnicodeBlock,  "Character.UnicodeBlock";
            Class,
            ClassLoader,
            ClassValue,
            Compiler,
            Double,
            Enum,
            Float,
            InheritableThreadLocal,
            Integer,
            Long,
            //Math,
            Number,
            Object,
            Package,
            Process,
            ProcessBuilder,
            //ProcessBuilder.Redirect,
            Runtime,
            RuntimePermission,
            SecurityManager,
            Short,
            StackTraceElement,
            StrictMath,
            String,
            StringBuffer,
            StringBuilder,
            System,
            Thread,
            ThreadGroup,
            ThreadLocal,
            Throwable,
            Void,
        };

        public static string[] Enums =
        {
            //Character.UnicodeScript,  "Character.UnicodeScript";
            //ProcessBuilder.Redirect.Type,  "ProcessBuilder.Redirect.Type";
            //Thread.State,  "Thread.State";
        };

        public static string[] Exceptions = {
            //ArithmeticException,
            //ArrayIndexOutOfBoundsException,
            //ArrayStoreException,
            //ClassCastException,
            //ClassNotFoundException,
            //CloneNotSupportedException,
            //EnumConstantNotPresentException,
            //Exception,
            //IllegalAccessException,
            //IllegalArgumentException,
            //IllegalMonitorStateException,
            //IllegalStateException,
            //IllegalThreadStateException,
            //IndexOutOfBoundsException,
            //InstantiationException,
            //InterruptedException,
            //NegativeArraySizeException,
            //NoSuchFieldException,
            //NoSuchMethodException,
            //NullPointerException,
            //NumberFormatException,
            //ReflectiveOperationException,
            //RuntimeException,
            //SecurityException,
            //StringIndexOutOfBoundsException,
            //TypeNotPresentException,
            //UnsupportedOperationException,
        };

        public static string[] Errors = {
            //AbstractMethodError,
            //AssertionError,
            //BootstrapMethodError,
            //ClassCircularityError,
            //ClassFormatError,
            //Error,
            //ExceptionInInitializerError,
            //IllegalAccessError,
            //IncompatibleClassChangeError,
            //InstantiationError,
            //InternalError,
            //LinkageError,
            //NoClassDefFoundError,
            //NoSuchFieldError,
            //NoSuchMethodError,
            //OutOfMemoryError,
            //StackOverflowError,
            //ThreadDeath,
            //UnknownError,
            //UnsatisfiedLinkError,
            //UnsupportedClassVersionError,
            //VerifyError,
            //VirtualMachineError,
        };

        public static bool Contains(string type)
        {
            return
                Interfaces.Contains(type) ||
                Classes.Contains(type) ||
                Enums.Contains(type) ||
                Exceptions.Contains(type) ||
                Errors.Contains(type);
        }
    }
}
