// Generated by TinyPG v1.3 available at www.codeproject.com

using System;
using System.Collections.Generic;

// Disable unused variable warnings which
// can happen during the parser generation.
#pragma warning disable 168

namespace Wizard.Runes.TinyPG
{
    #region Parser

    public partial class Parser 
    {
        private Scanner scanner;
        private ParseTree tree;
        
        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
        }

         public ParseTree Parse(string input)
        {
            return Parse(input, "", new ParseTree());
        }

        public ParseTree Parse(string input, string fileName)
        {
            return Parse(input, fileName, new ParseTree());
        }

        public ParseTree Parse(string input, string fileName, ParseTree tree)
        {
            scanner.Init(input, fileName);

            this.tree = tree;
            ParseStart(tree);
            tree.Skipped = scanner.Skipped;

            return tree;
        }

        private void ParseStart(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Start), "Start");
            parent.Nodes.Add(node);

            found = false;
            do {
                tok = scanner.LookAhead(TokenType.WORD, TokenType.META);
                switch (tok.Type)
                {
                    case TokenType.WORD:
                        ParseItem(node);
                        break;
                    case TokenType.META:
                        ParseMeta(node);
                        break;
                    default:
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected WORD or META.", 0x0002, tok));
                        break;
                }
                if(!found) {
                    tok = scanner.LookAhead(TokenType.WORD, TokenType.META);
                found = true;
                } else {
                    tok = scanner.LookAhead(TokenType.WORD, TokenType.META);
                }
            } while (tok.Type == TokenType.WORD
                || tok.Type == TokenType.META);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseMeta(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Meta), "Meta");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.META);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.META) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.META.ToString(), 0x1001, tok));
                return;
            }

            
            tok = scanner.Scan(TokenType.WORD);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.WORD) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.WORD.ToString(), 0x1001, tok));
                return;
            }

            
            ParseValue(node);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseItem(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Item), "Item");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.WORD);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.WORD) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.WORD.ToString(), 0x1001, tok));
                return;
            }

            
            ParseValue(node);

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseValue(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Value), "Value");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.WORD, TokenType.QUOTE, TokenType.NUMBER, TokenType.SOPEN, TokenType.SCLOSE);
            switch (tok.Type)
            {
                case TokenType.WORD:
                case TokenType.QUOTE:
                case TokenType.NUMBER:
                    ParseAtom(node);
                    break;
                case TokenType.SOPEN:
                    ParseGroup(node);
                    break;
                case TokenType.SCLOSE:
                    tok = scanner.Scan(TokenType.SCLOSE);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.SCLOSE) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SCLOSE.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected WORD, QUOTE, NUMBER, SOPEN, or SCLOSE.", 0x0002, tok));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseAtom(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Atom), "Atom");
            parent.Nodes.Add(node);

            tok = scanner.LookAhead(TokenType.WORD, TokenType.QUOTE, TokenType.NUMBER);
            switch (tok.Type)
            {
                case TokenType.WORD:
                    tok = scanner.Scan(TokenType.WORD);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.WORD) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.WORD.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.QUOTE:
                    tok = scanner.Scan(TokenType.QUOTE);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.QUOTE) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.QUOTE.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                case TokenType.NUMBER:
                    tok = scanner.Scan(TokenType.NUMBER);
                    n = node.CreateNode(tok, tok.ToString() );
                    node.Token.UpdateRange(tok);
                    node.Nodes.Add(n);
                    if (tok.Type != TokenType.NUMBER) {
                        tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.NUMBER.ToString(), 0x1001, tok));
                        return;
                    }
                    break;
                default:
                    tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected WORD, QUOTE, or NUMBER.", 0x0002, tok));
                    break;
            }

            parent.Token.UpdateRange(node.Token);
        }

        private void ParseGroup(ParseNode parent)
        {
            Token tok;
            ParseNode n;
            bool found;
            ParseNode node = parent.CreateNode(scanner.GetToken(TokenType.Group), "Group");
            parent.Nodes.Add(node);


            
            tok = scanner.Scan(TokenType.SOPEN);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.SOPEN) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SOPEN.ToString(), 0x1001, tok));
                return;
            }

            
            found = false;
            do {
                ParseItem(node);
                if(!found) {
                    tok = scanner.LookAhead(TokenType.WORD);
                found = true;
                } else {
                    tok = scanner.LookAhead(TokenType.SCLOSE, TokenType.WORD);
                }
            } while (tok.Type == TokenType.WORD);

            
            tok = scanner.Scan(TokenType.SCLOSE);
            n = node.CreateNode(tok, tok.ToString() );
            node.Token.UpdateRange(tok);
            node.Nodes.Add(n);
            if (tok.Type != TokenType.SCLOSE) {
                tree.Errors.Add(new ParseError("Unexpected token '" + tok.Text.Replace("\n", "") + "' found. Expected " + TokenType.SCLOSE.ToString(), 0x1001, tok));
                return;
            }

            parent.Token.UpdateRange(node.Token);
        }


    }

    #endregion Parser
}
