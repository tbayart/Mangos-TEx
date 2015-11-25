using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Framework.Helpers
{
    public static class DebugHelper
    {
        /// <summary>
        /// A helper method which provides enhanced string representation 
        /// from any exception specially for debugging purpose
        /// </summary>
        public static string DumpToString(this Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            if (ex.InnerException != null)
            {
                //called recursively for nested exceptions (inner, outer, etc)
                sb.Append(DumpToString(ex.InnerException));
                sb.Append(Environment.NewLine);
                sb.Append("(Outer Exception)");
                sb.Append(Environment.NewLine);

            }

            sb.Append("Exception Type:        ");
            try
            {
                sb.Append(ex.GetType().FullName);
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
            }

            sb.Append(Environment.NewLine);

            sb.Append("Exception Message:        ");
            try
            {
                sb.Append(ex.Message);
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
            }

            sb.Append(Environment.NewLine);

            sb.Append("Exception Source:        ");
            try
            {
                sb.Append(ex.Source);
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
            }

            sb.Append(Environment.NewLine);

            sb.Append("Exception Target Site:   ");
            try
            {
                if (ex.TargetSite != null)
                    sb.Append(ex.TargetSite.Name);
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
            }

            if (ex.Data.Count > 0)
            {
                sb.Append(Environment.NewLine);

                sb.Append("Exception Data:   ");
                try
                {
                    if (ex.TargetSite != null)
                        sb.Append(DumpBuilder.Dump(ex.Data));
                }
                catch (Exception e)
                {
                    sb.Append(e.Message);
                }
            }

            sb.Append(Environment.NewLine);

            //Append EnhancedStackTrace
            try
            {
                sb.Append(EnhancedStackTrace(ex));
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
            }

            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        // '' <summary>
        // '' enhanced stack trace generator
        // '' </summary>
        private static string EnhancedStackTrace(StackTrace st, string skipClassName)
        {
            int intFrame;
            // Warning!!! Optional parameters not supported
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append("---- Stack Trace ----");
            sb.Append(Environment.NewLine);
            for (intFrame = 0; intFrame < st.FrameCount; intFrame++)
            {
                StackFrame sf = st.GetFrame(intFrame);
                MemberInfo mi = sf.GetMethod();
                if (string.IsNullOrEmpty(skipClassName)
                    || mi.DeclaringType.Name.IndexOf(skipClassName) == -1)
                {
                    sb.Append(StackFrameToString(sf));
                }

            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        // '' <summary>
        // '' enhanced stack trace generator, using existing exception as start point
        // '' </summary>
        private static string EnhancedStackTrace(this Exception ex)
        {
            return EnhancedStackTrace(new StackTrace(ex, true), string.Empty);
        }

        // '' <summary>
        // '' enhanced stack trace generator, using current execution as start point
        // '' </summary>
        private static string EnhancedStackTrace()
        {
            return EnhancedStackTrace(new StackTrace(true), "UnhandledException");
        }

        // '' <summary>
        // '' turns a single stack frame object into an informative string
        // '' </summary>
        private static string StackFrameToString(StackFrame sf)
        {
            StringBuilder sb = new StringBuilder();
            MemberInfo mi = sf.GetMethod();
            // -- build method name
            sb.Append("   ");
            sb.Append(mi.DeclaringType.Namespace);
            sb.Append(".");
            sb.Append(mi.DeclaringType.Name);
            sb.Append(".");
            sb.Append(mi.Name);
            // -- build method params
            sb.Append("(");
            int intParam = 0;
            foreach (ParameterInfo param in sf.GetMethod().GetParameters())
            {
                intParam++;
                if (intParam > 1)
                {
                    sb.Append(", ");
                }
                sb.Append(param.Name);
                sb.Append(" As ");
                sb.Append(param.ParameterType.Name);
            }

            sb.Append(")");
            sb.Append(Environment.NewLine);
            // -- if source code is available, append location info
            sb.Append("       ");
            if (string.IsNullOrEmpty(sf.GetFileName()) || sf.GetFileName().Length == 0)
            {
                sb.Append("(unknown file)");
                sb.Append(": N ");
                sb.Append(string.Format("{0:#00000}", sf.GetNativeOffset()));
            }
            else
            {
                sb.Append(System.IO.Path.GetFileName(sf.GetFileName()));
                sb.Append(": line ");
                sb.Append(string.Format("{0:#0000}", sf.GetFileLineNumber()));
                sb.Append(", col ");
                sb.Append(string.Format("{0:#00}", sf.GetFileColumnNumber()));
                // -- if IL is available, append IL location info
                if (sf.GetILOffset() != StackFrame.OFFSET_UNKNOWN)
                {
                    sb.Append(", IL ");
                    sb.Append(string.Format("{0:#0000}", sf.GetILOffset()));
                }
            }
            sb.Append(Environment.NewLine);

            return sb.ToString();
        }

        #region PreserveExceptionStackTrace
        /// <summary>
        /// Used to preserve stack traces on rethrow
        /// </summary>
        private static readonly MethodInfo PreserveException =
            typeof(Exception).GetMethod("InternalPreserveStackTrace",
                                        BindingFlags.Instance | BindingFlags.NonPublic);

        /// <summary>
        /// Calls the Exception's internal method to preserve its stack trace prior to rethrow
        /// </summary>
        /// <remarks>
        /// See http://dotnetjunkies.com/WebLog/chris.taylor/archive/2004/03/03/8353.aspx for more info.
        /// </remarks>
        /// <param name="e"></param>
        public static void PreserveExceptionStackTrace(this Exception e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            PreserveException.Invoke(e, null);
        }
        #endregion PreserveExceptionStackTrace
    }

    /// <summary>
    /// Class that helps in dumping.
    /// </summary>
    public sealed class DumpBuilder
    {
        #region Fields
        private List<string> _lines = new List<string>();
        private readonly int _indentLevel = 0;
        private readonly bool _deep = false;
        #endregion Fields

        #region Ctor
        public DumpBuilder()
            : this(0, false, null)
        {
        }

        /// <param name="indentLevel">The indent level.</param>
        public DumpBuilder(int indentLevel)
            : this(indentLevel, false, null)
        {
        }

        /// <param name="indentLevel">The indent level.</param>
        /// <param name="deep">if set to <c>true</c> [deep].</param>
        public DumpBuilder(int indentLevel, bool deep)
            : this(indentLevel, deep, null)
        {
        }

        /// <param name="indentLevel">The indent level.</param>
        /// <param name="deep">if set to <c>true</c> [deep].</param>
        /// <param name="typeToDump">The type to dump.</param>
        public DumpBuilder(int indentLevel, bool deep, Type typeToDump)
        {
            _indentLevel = indentLevel;
            _deep = deep;

            if (typeToDump != null)
            {
                string s = string.Format(@"Dumping for '{0}':",
                                         typeToDump.FullName);

                _lines.Add(s);
            }
        }
        #endregion Ctor

        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is deep.
        /// </summary>
        /// <value><c>true</c> if this instance is deep; otherwise, <c>false</c>.</value>
        public bool IsDeep
        {
            get { return _deep; }
        }
        #endregion Properties

        #region Public methods
        /// <summary>
        /// Add a line to dump.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddLine(string name, object value)
        {
            _lines.Add(MakeStringToAdd(name, value));
        }

        /// <summary>
        /// Add a line to dump.
        /// </summary>
        /// <param name="text">The text.</param>
        public void AddLine(string text)
        {
            _lines.Add(text);
        }

        /// <summary>
        /// Insert a line to dump.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void InsertLine(int index, string name, object value)
        {
            _lines.Insert(index, MakeStringToAdd(name, value));
        }

        /// <summary>
        /// Insert a line to dump.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="text">The text.</param>
        public void InsertLine(int index, string text)
        {
            _lines.Insert(index, text);
        }

        /// <summary>
        /// Get the dumped content.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string line in _lines)
            {
                if (string.IsNullOrEmpty(line) == false)
                {
                    sb.AppendLine(DoIndent(line.TrimEnd()));
                }
            }

            return sb.ToString().TrimEnd();
        }
        #endregion Public methods

        #region Static methods for dumping predefined items
        /// <summary>
        /// Dumps an object.
        /// </summary>
        /// <param name="x">The object to dump.</param>
        /// <returns>Returns the string with the dump.</returns>
        /// <remarks>Thanks to J. Dunlap for the code.</remarks>
        public static string Dump(object x)
        {
            return Dump(x, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy, 5);
        }

        /// <summary>
        /// Dumps an object.
        /// </summary>
        /// <param name="x">The object to dump.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="maxDepth">The max depth.</param>
        /// <returns>Returns the string with the dump.</returns>
        /// <remarks>Thanks to J. Dunlap for the code.</remarks>
        public static string Dump(object x, BindingFlags flags, int maxDepth)
        {
            StringBuilder sb = new StringBuilder();
            Reflect(sb, x, flags, maxDepth);

            return sb.ToString();
        }
        #endregion Static methods for dumping predefined items

        #region Reflection output
        /// <summary>
        /// Reflects the specified sb.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="maxDepth">The max depth.</param>
        private static void Reflect(StringBuilder sb, object obj, BindingFlags flags, int maxDepth)
        {
            Reflect(sb, new GraphRef(null, obj, null), 0, flags, maxDepth);
        }

        /// <summary>
        /// Reflects the specified sb.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="obj">The obj.</param>
        /// <param name="indent">The indent.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="maxDepth">The max depth.</param>
        private static void Reflect(StringBuilder sb, GraphRef obj, int indent, BindingFlags flags, int maxDepth)
        {
            //Ensure that we are not following a circular reference path
            if ((obj.Value is ValueType) == false)
            {
                GraphRef parentRef = obj.Parent;
                while (parentRef != null)
                {
                    if (parentRef.Value == obj.Value)
                        return;
                    parentRef = parentRef.Parent;
                }
            }

            sb.Append('\t', indent);

            //Output property name if applicable
            if (!String.IsNullOrEmpty(obj.PropName))
            {
                sb.Append(obj.PropName);
                sb.Append("=");
            }

            int childIndent = indent + 1;

            //If value is null, output "null"
            if (obj.Value == null)
            {
                sb.Append("null");
            }
            //If value is a string, output value with quotes around it
            else if (obj.Value is string)
            {
                sb.Append("\"" + Escape((string)obj.Value) + "\"");
            }
            //If value is a char, output value with single quotes around it
            else if (obj.Value is char)
            {
                sb.Append("\'" + Escape(new String((char)obj.Value, 1)) + "\'");
            }
            //If value is a XmlNode, output xml value with quotes around it
            else if (obj.Value is System.Xml.XmlNode)
            {
                sb.AppendLine("\"");
                sb.AppendLine(((System.Xml.XmlNode)obj.Value).OuterXml);
                sb.AppendLine("\"");
            }
            //If value is an IEnumerable, output all the items
            else if (obj.Value is IEnumerable)
            {
                sb.AppendLine();
                sb.Append('\t', indent);
                sb.Append("[");
                sb.AppendLine();

                IEnumerator enumerator = ((IEnumerable)obj.Value).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Reflect(sb, new GraphRef(obj, enumerator.Current, null), childIndent, flags, maxDepth);
                    sb.AppendLine(",");
                }

                sb.Append('\t', indent);
                sb.Append("]");
            }

            //If it's a Type object, we don't want to endlessly follow long trains of 
            //interconnected type info objects
            else if (obj.Value is Type)
            {
                sb.Append("Type: ");
                sb.Append(((Type)obj.Value).FullName);
            }
            //...and similarly for MemberInfo objects
            else if (obj.Value is MemberInfo)
            {
                sb.Append(obj.Value.GetType().Name);
                sb.Append(": ");
                sb.Append(((MemberInfo)obj.Value).Name);
            }
            //If value is not of a basic datatype
            else if (Convert.GetTypeCode(obj.Value) == TypeCode.Object)
            {
                Type type = obj.Value.GetType();
                sb.Append(type.Name); //might want to use type.FullName instead
                if (indent <= maxDepth)
                {
                    sb.AppendLine();
                    sb.Append('\t', indent);
                    sb.AppendLine("{");

                    //Get all the fileds in the object's type
                    FieldInfo[] fields = type.GetFields(flags);
                    //Enumerate all the properties and output their values
                    for (int i = 0; i < fields.Length; i++)
                    {
                        FieldInfo fi = fields[i];
                        try
                        {
                            Reflect(sb, new GraphRef(obj, fi.GetValue(obj.Value), fi.Name), childIndent, flags, maxDepth);
                        }
                        catch (Exception e)
                        {
                            sb.Append("<Error getting field value (" + e.GetType().Name + ") " + fi.Name + ">");
                        }
                        if (i < fields.Length - 1)
                            sb.Append(',');
                        sb.AppendLine();
                    }

                    //Get all the properties in the object's type
                    PropertyInfo[] props = type.GetProperties(flags);
                    //Enumerate all the properties and output their values
                    for (int i = 0; i < props.Length; i++)
                    {
                        PropertyInfo pi = props[i];
                        if (pi.GetIndexParameters().Length == 0) //Ignore indexers
                        {
                            try
                            {
                                Reflect(sb, new GraphRef(obj, pi.GetValue(obj.Value, null), pi.Name), childIndent, flags, maxDepth);
                            }
                            catch (Exception e)
                            {
                                sb.Append("<Error getting property value (" + e.GetType().Name + ") " + pi.Name + ">");
                            }
                            if (i < props.Length - 1)
                                sb.Append(',');
                            sb.AppendLine();
                        }
                    }

                    sb.Append('\t', indent);
                    sb.Append("}");
                }
            }
            //If value is of a basic datatype
            else
            {
                sb.Append(obj.Value.ToString());
            }
        }
        #endregion Reflection output

        #region Escaping
        private static readonly Dictionary<string, string> _escapeReplacements =
            new Dictionary<string, string>
                {
                    { "\r", "\\r" },
                    { "\n", "\\n" },
                    { "\t", "\\t" },
                    { "\"", "\\\"" },
                    { "\'", "\\\'" },
                    { "\\", "\\\\" },
                };

        /// <summary>
        /// Escapes characters in a string using the escaping system used 
        /// in C# string literals
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public static string Escape(string input)
        {
            StringBuilder sb = new StringBuilder(input);
            foreach (var replacement in _escapeReplacements)
                sb.Replace(replacement.Key, replacement.Value);
            return sb.ToString();
        }
        #endregion Escaping

        #region Private helper class for reflecting
        /// <summary>
        /// Represents a reference within an object graph.
        /// </summary>
        private class GraphRef
        {
            #region Fields
            private object _value;
            private GraphRef _parent;
            private string _propName;
            #endregion Fields

            #region Properties
            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <value>The value.</value>
            public object Value
            {
                get { return _value; }
            }

            /// <summary>
            /// Gets the parent.
            /// </summary>
            /// <value>The parent.</value>
            public GraphRef Parent
            {
                get { return _parent; }
            }

            /// <summary>
            /// Gets the name of the prop.
            /// </summary>
            /// <value>The name of the prop.</value>
            public string PropName
            {
                get { return _propName; }
            }
            #endregion Properties

            #region Methods
            /// <summary>
            /// Initializes a new instance of the <see cref="GraphRef"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="obj">The obj.</param>
            /// <param name="propName">Name of the prop.</param>
            public GraphRef(GraphRef parent, object obj, string propName)
            {
                _parent = parent;
                _value = obj;
                _propName = propName;
            }
            #endregion Methods
        }
        #endregion Private helper class for reflecting

        #region Private methods
        /// <summary>
        /// Makes the string to add.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static string MakeStringToAdd(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name.Length <= 0)
            {
                throw new ArgumentException("name");
            }

            string result = string.Empty;

            result += string.Format(
                @"{0}: '{1}'",
                name,
                value == null ? @"(null)" : value.ToString());

            return result;
        }

        /// <summary>
        /// Does the indent.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        private string DoIndent(string text)
        {
            StringBuilder result = new StringBuilder();

            result.Append('\t', _indentLevel);
            result.Append(text.TrimEnd());

            return result.ToString();
        }
        #endregion Private methods
    }
}
