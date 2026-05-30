/*
 * File: Treedump.cs
 * Author: Noah Hensley
 * Course: ETEC4401
 * 
 * AI Usage: Claude (Anthropic) was used to generate XML documentation comments
 * for this file on the following dates:
 *      January 25, 2026
 */

using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides utilities for visualizing object hierarchies in multiple output formats.
/// </summary>
/// <remarks>
/// Supports three output formats:
/// <list type="bullet">
/// <item><description>Text tree: Human-readable tree structure with Unicode box-drawing characters</description></item>
/// <item><description>JSON tree: Structured JSON representation of the object hierarchy</description></item>
/// <item><description>DOT tree: GraphViz DOT format for graph visualization</description></item>
/// </list>
/// Uses reflection to traverse object structures including user-defined types, collections, arrays, and tuples.
/// </remarks>
public class Treedump {

    /// <summary>
    /// Internal node representation for tree structure.
    /// </summary>
    private class Node{
        /// <summary>
        /// Label text for the node.
        /// </summary>
        public string label;
        /// <summary>
        /// Unique identifier for the node.
        /// </summary>
        public string unique;
        /// <summary>
        /// Whether the node represents a null value.
        /// </summary>
        public bool isNull;
        /// <summary>
        /// Child nodes.
        /// </summary>
        public List<Node> children = new();
        private static int counter=0;
        
        /// <summary>
        /// Creates a new node with the specified label.
        /// </summary>
        /// <param name="lbl">The label text for the node.</param>
        /// <param name="isNull">Whether this node represents a null value.</param>
        public Node(string lbl, bool isNull){
            this.label=lbl;
            this.unique = $"n{counter++}";
            this.isNull=isNull;
        }
    }

    /// <summary>
    /// Outputs a text-based tree representation of an object hierarchy.
    /// </summary>
    /// <param name="obj">The object to visualize.</param>
    /// <param name="outs">The text writer to output to.</param>
    /// <remarks>
    /// Produces a human-readable tree using Unicode box-drawing characters (│, ├, └).
    /// </remarks>
    public static void textTree(Object obj, TextWriter outs){
        reflect(outs,obj, null, new List<bool>());
    }

    /// <summary>
    /// Outputs a JSON representation of an object hierarchy.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="outs">The text writer to output to.</param>
    /// <remarks>
    /// Creates a nested JSON structure representing the object tree with indented formatting.
    /// </remarks>
    public static void jsonTree(Object obj, TextWriter outs){
        var sw = new StringWriter();
        var root = reflect(sw,obj, null, new List<bool>());
        var opts = new System.Text.Json.JsonSerializerOptions();
        opts.IncludeFields=true;
        opts.WriteIndented=true;
        opts.MaxDepth=1000000;
        string J = System.Text.Json.JsonSerializer.Serialize(root,opts);
        outs.WriteLine(J);
    }

    /// <summary>
    /// Outputs a GraphViz DOT format representation of an object hierarchy.
    /// </summary>
    /// <param name="obj">The object to visualize.</param>
    /// <param name="outs">The text writer to output to.</param>
    /// <remarks>
    /// Produces DOT format output suitable for rendering with GraphViz tools.
    /// Null nodes are styled with grey fill.
    /// </remarks>
    public static void dotTree(Object obj, TextWriter outs){
        var sw = new StringWriter();
        var root = reflect(sw,obj, null, new List<bool>());
        outs.WriteLine("graph d {");
        outs.WriteLine("node [ shape=rectangle, fontfamily=helvetica]");
        walkPreOrder(root, (Node n) => {
            outs.Write($"{n.unique} [label=\"{escape(n.label)}\"");
            if( n.isNull )
                outs.Write(" style=filled fillcolor=grey");
            outs.WriteLine("];");
        });
        walkPreOrder(root, (Node n) => {
            foreach(var c in n.children){
                outs.WriteLine($"{n.unique} -- {c.unique};");
            }
        });
        outs.WriteLine("}");
    }
    
    /// <summary>
    /// Escapes special characters for DOT format output.
    /// </summary>
    /// <param name="s">The string to escape.</param>
    /// <returns>The escaped string with newlines and quotes escaped.</returns>
    private static string escape(string s){
        string o="";
        foreach(var c in s){
            switch(c){
                case '\n':
                    o+="\\n";
                    break;
                case '\"':
                    o += "\\\"";
                    break;
                default:
                    o += c;
                    break;
            }
        }
        return o;
    }
    
    /// <summary>
    /// Recursively walks a node tree and applies an action to each node.
    /// </summary>
    /// <param name="n">The node to start from.</param>
    /// <param name="f">The action to apply to each node.</param>
    private static void walkPreOrder(Node n, Action<Node> f){
        f(n);
        foreach(var c in n.children){
            walkPreOrder(c,f);
        }
    }

    /// <summary>
    /// Prints a string with appropriate tree indentation based on sibling structure.
    /// </summary>
    /// <param name="outs">The text writer to output to.</param>
    /// <param name="hasNextSibling">List tracking whether each ancestor has a next sibling.</param>
    /// <param name="label">The label text to print.</param>
    private static void printWithIndentation(TextWriter outs, List<bool> hasNextSibling, string label){

        for(int i=0;i<hasNextSibling.Count-1;++i){
            if( hasNextSibling[i]){
                outs.Write("│ ");
            } else {
                outs.Write("  ");
            }
        }
        if( hasNextSibling.Count > 0 ){
            if( hasNextSibling[^1] == true )
                outs.Write("├─");
            else
                outs.Write("└─");
        }
        outs.Write(label);
        outs.WriteLine();
    }

    /// <summary>
    /// Checks if an object is an aggregate type (list, array, or enumerable).
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object is a list, array, or enumerable; false otherwise.</returns>
    private static bool isAggregate(Object? obj){
        return isList(obj) || isArray(obj) || isEnumerable(obj);
    }

    /// <summary>
    /// Checks if an object is a generic List.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object is a List&lt;T&gt;; false otherwise.</returns>
    private static bool isList(Object? obj){
        if( obj == null )
            return false;
        var f = obj.GetType();
        if( f.IsGenericType && f.GetGenericTypeDefinition() == typeof(List<>) ){
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Checks if an object is enumerable (excluding strings).
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object implements IEnumerable and is not a string; false otherwise.</returns>
    private static bool isEnumerable(Object? obj){
        if( obj == null )
            return false;
        if( obj as String != null )
            return false;       //don't count strings as enumerable
        return null != obj as IEnumerable;
    } 

    /// <summary>
    /// Checks if an object is a tuple.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object implements ITuple; false otherwise.</returns>
    private static bool isTuple(Object? obj){
        if( obj == null )
            return false;
        var tmp = obj as ITuple;
        return tmp != null;
    }
    
    /// <summary>
    /// Checks if an object is an array.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object is an array type; false otherwise.</returns>
    private static bool isArray(Object? obj){ 
        if( obj == null )
            return false;
        var f = obj.GetType();
        return f.IsArray;
    }

    /// <summary>
    /// Checks if an object is a user-defined type (not a System type or array).
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if the object's namespace is not System or System.*; false otherwise.</returns>
    private static bool isUserDefinedType(Object? obj) {
        if( obj == null )
            return false;
        string? ns = obj.GetType().Namespace;
        if( ns == null )
            return true;
        if( ns == "System")
            return false;
        if( ns.StartsWith("System."))
            return false;
        if( obj.GetType().IsArray )
            return false;
        return true;
    }
 
    /// <summary>
    /// Checks if all public instance fields of an object are primitive types.
    /// </summary>
    /// <param name="obj">The object to check.</param>
    /// <returns>True if all fields are primitive (not aggregates or user-defined types); false otherwise.</returns>
    private static bool allFieldsArePrimitive(Object? obj){
        if( obj == null )
            return true;
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance|BindingFlags.Public);
        foreach(var f in fields){
            Object? fdata = f.GetValue(obj);
            if( fdata != null ){
                if( isAggregate(fdata) || isUserDefinedType(fdata) )
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Gets a string representation of all field values in an object.
    /// </summary>
    /// <param name="obj">The object to get values from.</param>
    /// <returns>A space-separated string of "name=value" pairs with special characters replaced.</returns>
    private static string getValues(Object? obj){
        if( obj == null )
            return "null";
        List<string> tmp = new();
        foreach( var finfo in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public) ){
            string v = $"{finfo.GetValue(obj)}";
            v = v.Replace("\n","↵").Replace("\r","␍").Replace("\t","↦");
            tmp.Add(finfo.Name+"="+v);
        }
        return String.Join(" ",tmp);
    }

    /// <summary>
    /// Reflects over an object hierarchy to build a node tree.
    /// </summary>
    /// <param name="outs">The text writer for text output during reflection.</param>
    /// <param name="obj">The object to reflect over.</param>
    /// <param name="name">The field name of this object (null for root).</param>
    /// <param name="hasNextSibling">List tracking sibling structure for indentation.</param>
    /// <returns>A Node representing the reflected object tree.</returns>
    /// <remarks>
    /// This method uses reflection to traverse object hierarchies, handling:
    /// <list type="bullet">
    /// <item><description>Null values</description></item>
    /// <item><description>Lists, arrays, and enumerables</description></item>
    /// <item><description>Tuples</description></item>
    /// <item><description>User-defined types with fields</description></item>
    /// <item><description>Primitive values</description></item>
    /// </list>
    /// Objects with all primitive fields are displayed inline without recursion.
    /// </remarks>
    private static Node reflect(TextWriter outs,
            Object? obj, 
            string? name,        //may be empty string 
            List<bool> hasNextSibling
    ){

        string longName = "";
        string shortName = "";
        if( name != null && name.Length != 0 ){
            longName += $"{name} : ";
            shortName = $"{name}";
        }

        bool recurse;
        bool isNull=false;
        if( Object.ReferenceEquals(obj,null)){
            longName += "null";
            shortName += " (null)";
            recurse=false;
            isNull=true;
        } else if( isList(obj) ){
            IList L = (IList)obj;
            string plural = ( (L.Count == 1) ? "":"s");
            longName += $"List<{obj.GetType().GenericTypeArguments[0].Name}> with {L.Count} element{plural}";
            recurse=true;
            shortName += $" (List<{obj.GetType().GenericTypeArguments[0].Name}>)";
       } else if( isArray(obj) ){
            Array A = (Array)obj;
            string plural = ( (A.Length == 1) ? "":"s");
            longName += $"{obj.GetType().Name} with {A.Length} element{plural}";  //includes []'s
            shortName += $" ({obj.GetType().Name}[{A.Length}])";
            recurse=true;
        } else if (isEnumerable(obj) ){
            recurse = true;
        } else if( isTuple(obj)){
            recurse=true;
            ITuple? tmp = (ITuple?)obj;
            if( tmp == null )
                throw new Exception();
            List<string> L = new();
            for(int i=0;i<tmp.Length;++i){
                var item = tmp[i];
                if( item == null )
                    L.Add("null");
                else
                    L.Add(item.GetType().Name);
            }
            longName += "Tuple<"+String.Join(",",L)+">";
            shortName += " (Tuple<"+String.Join(",",L)+">)";

        } else if( !isUserDefinedType(obj)  ){
            longName += $"{obj}";
            shortName += $" ({obj})";
            recurse=false;
        } else {
            longName += $"{obj.GetType().Name}";
            shortName += $" ({obj.GetType().Name})";
            recurse=true;
        }


        if( obj != null && isUserDefinedType(obj) && allFieldsArePrimitive(obj)){
            var fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            if( fields.Length > 0 ){
                longName += "{";
                foreach(var f in fields){
                    string v = $"{f.GetValue(obj)}";
                    longName += $" {f.Name}={v}";
                    shortName += $"\\n{f.Name}={escape(v)}";
                }
                longName += " }";
            }
            recurse=false;
        }


        Node n = new Node(shortName,isNull);

        printWithIndentation(outs,hasNextSibling,longName);

        if(!recurse){
            return n;
        }

        if( obj == null ){
            return n;
        } else if( isList(obj) || isArray(obj) || isEnumerable(obj) || isTuple(obj) ){
            List<Object?> items = new();

            if( isList(obj) ){
                IList? L = (IList?)obj;
                if( L == null )
                    throw new Exception();      //checked for null previously
                foreach(var ob in L){
                    items.Add(ob);
                }
            } else if( isArray(obj) ){
                Array? A = (Array?)obj;
                if(A == null )
                    throw new Exception();
                foreach(var ob in A){
                    items.Add(ob);
                }
            } else if( isEnumerable(obj) ){
                IEnumerable? E = (IEnumerable?)obj;
                if( E == null )
                    throw new Exception();
                foreach(var ob in E){
                    items.Add(ob);
                }
            } else if( isTuple(obj) ){
                ITuple? T = (ITuple?)obj;
                if(T == null)
                    throw new Exception();
                for(int i=0;i<T.Length;++i){
                    items.Add(T[i]);
                }
            } else {
                throw new Exception();
            }
            hasNextSibling.Add(true);
            for(int i=0;i<items.Count;++i){
                if( i == items.Count-1)
                    hasNextSibling[^1] = false;
                Node c = reflect(outs,items[i],$"[{i}]",hasNextSibling);
                n.children.Add(c);
            }
            hasNextSibling.RemoveAt(hasNextSibling.Count-1);
        } else if( !isUserDefinedType(obj) ){
            return n;
        } else {
            if( obj == null )
                throw new Exception();
            var ty = obj.GetType();
            if( ty == null )
                throw new Exception();
            var fields = ty.GetFields(BindingFlags.Instance | BindingFlags.Public);
            hasNextSibling.Add(true);
            for(int j=0;j<fields.Length;++j){
                if( j == fields.Length-1 )
                    hasNextSibling[^1]=false;
                Node c = reflect(outs,fields[j].GetValue(obj),fields[j].Name,hasNextSibling);
                n.children.Add(c);
            }
            hasNextSibling.RemoveAt(hasNextSibling.Count-1);
        }
        return n;
    }
}