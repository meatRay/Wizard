// Generated by TinyPG v1.3 available at www.codeproject.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Wizard.Runes.TinyPG
{
    #region ParseTree
    [Serializable]
    public class ParseErrors : List<ParseError>
    {
    }

    [Serializable]
    public class ParseError
    {
        private string file;
        private string message;
        private int code;
        private int line;
        private int col;
        private int pos;
        private int length;

        public string File { get { return file; } }
        public int Code { get { return code; } }
        public int Line { get { return line; } }
        public int Column { get { return col; } }
        public int Position { get { return pos; } }
        public int Length { get { return length; } }
        public string Message { get { return message; } }

        // just for the sake of serialization
        public ParseError()
        {
        }

        public ParseError(string message, int code, ParseNode node) : this(message, code, node.Token)
        {
        }

        public ParseError(string message, int code, Token token) : this(message, code, token.File, token.Line, token.Column, token.StartPos, token.Length)
        {
        }

        public ParseError(string message, int code) : this(message, code, string.Empty, 0, 0, 0, 0)
        {
        }

        public ParseError(string message, int code, string file, int line, int col, int pos, int length)
        {
            this.file = file;
            this.message = message;
            this.code = code;
            this.line = line;
            this.col = col;
            this.pos = pos;
            this.length = length;
        }
    }

    // rootlevel of the node tree
    [Serializable]
    public partial class ParseTree : ParseNode
    {
        public ParseErrors Errors;

        public List<Token> Skipped;

        public ParseTree() : base(new Token(), "ParseTree")
        {
            Token.Type = TokenType.Start;
            Token.Text = "Root";
            Errors = new ParseErrors();
        }

        public string PrintTree()
        {
            StringBuilder sb = new StringBuilder();
            int indent = 0;
            PrintNode(sb, this, indent);
            return sb.ToString();
        }

        private void PrintNode(StringBuilder sb, ParseNode node, int indent)
        {
            
            string space = "".PadLeft(indent, ' ');

            sb.Append(space);
            sb.AppendLine(node.Text);

            foreach (ParseNode n in node.Nodes)
                PrintNode(sb, n, indent + 2);
        }
        
        /// <summary>
        /// this is the entry point for executing and evaluating the parse tree.
        /// </summary>
        /// <param name="paramlist">additional optional input parameters</param>
        /// <returns>the output of the evaluation function</returns>
        public object Eval(params object[] paramlist)
        {
            return Nodes[0].Eval(this, paramlist);
        }
    }

    [Serializable]
    [XmlInclude(typeof(ParseTree))]
    public partial class ParseNode
    {
        protected string text;
        protected List<ParseNode> nodes;
        
        public List<ParseNode> Nodes { get {return nodes;} }
        
        [XmlIgnore] // avoid circular references when serializing
        public ParseNode Parent;
        public Token Token; // the token/rule

        [XmlIgnore] // skip redundant text (is part of Token)
        public string Text { // text to display in parse tree 
            get { return text;} 
            set { text = value; }
        } 

        public virtual ParseNode CreateNode(Token token, string text)
        {
            ParseNode node = new ParseNode(token, text);
            node.Parent = this;
            return node;
        }

        protected ParseNode(Token token, string text)
        {
            this.Token = token;
            this.text = text;
            this.nodes = new List<ParseNode>();
        }

        protected object GetValue(ParseTree tree, TokenType type, int index)
        {
            return GetValue(tree, type, ref index);
        }

        protected object GetValue(ParseTree tree, TokenType type, ref int index)
        {
            object o = null;
            if (index < 0) return o;

            // left to right
            foreach (ParseNode node in nodes)
            {
                if (node.Token.Type == type)
                {
                    index--;
                    if (index < 0)
                    {
                        o = node.Eval(tree);
                        break;
                    }
                }
            }
            return o;
        }

        /// <summary>
        /// this implements the evaluation functionality, cannot be used directly
        /// </summary>
        /// <param name="tree">the parsetree itself</param>
        /// <param name="paramlist">optional input parameters</param>
        /// <returns>a partial result of the evaluation</returns>
        internal object Eval(ParseTree tree, params object[] paramlist)
        {
            object Value = null;

            switch (Token.Type)
            {
                case TokenType.Start:
                    Value = EvalStart(tree, paramlist);
                    break;
                case TokenType.Meta:
                    Value = EvalMeta(tree, paramlist);
                    break;
                case TokenType.Item:
                    Value = EvalItem(tree, paramlist);
                    break;
                case TokenType.Value:
                    Value = EvalValue(tree, paramlist);
                    break;
                case TokenType.Atom:
                    Value = EvalAtom(tree, paramlist);
                    break;
                case TokenType.Group:
                    Value = EvalGroup(tree, paramlist);
                    break;

                default:
                    Value = Token.Text;
                    break;
            }
            return Value;
        }

        protected virtual object EvalStart(ParseTree tree, params object[] paramlist)
        {
			return new ComplexRune("Rune",
					Nodes.Where(n => n.Token.Type == TokenType.Item)
					.Select(i => i.Eval(tree, paramlist) as Rune)
					.ToArray());
		}

        protected virtual object EvalMeta(ParseTree tree, params object[] paramlist)
        {
            foreach (var node in Nodes)
                node.Eval(tree, paramlist);
            return null;
        }

        protected virtual object EvalItem(ParseTree tree, params object[] paramlist)
        {
			string r_name = Nodes.First(n => n.Token.Type == TokenType.WORD).Token.Text;
			var val = Nodes.First(n => n.Token.Type == TokenType.Value).Nodes.First();
			if (val.Token.Type == TokenType.Atom)
			{
				object token;
				var atom = val.Nodes.First();
				if (atom.Token.Type == TokenType.NUMBER)
					token = double.Parse(atom.Token.Text);
				else
					token = atom.Token.Text.Substring(1, atom.Token.Text.Length - 2);
				return new TokenRune(r_name, token);
			}
			else if (val.Token.Type == TokenType.Group)
			{
				return new ComplexRune(r_name,
					val.Nodes.Where(n => n.Token.Type == TokenType.Item)
					.Select(i => i.Eval(tree, paramlist) as Rune)
					.ToArray());
			}
			else
				return new Rune(r_name);
		}

        protected virtual object EvalValue(ParseTree tree, params object[] paramlist)
        {
            foreach (var node in Nodes)
                node.Eval(tree, paramlist);
            return null;
        }

        protected virtual object EvalAtom(ParseTree tree, params object[] paramlist)
        {
            foreach (var node in Nodes)
                node.Eval(tree, paramlist);
            return null;
        }

        protected virtual object EvalGroup(ParseTree tree, params object[] paramlist)
        {
            foreach (var node in Nodes)
                node.Eval(tree, paramlist);
            return null;
        }


    }
    
    #endregion ParseTree
}
