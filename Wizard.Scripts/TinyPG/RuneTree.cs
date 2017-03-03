﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard.Runes.TinyPG
{
	class RuneTree : ParseTree
	{
		public Rune[] RuneEval()
		{
			foreach( var node in Nodes)
		}

		protected object EvalStart(ParseTree tree, params object[] paramlist)
		{
			return Nodes.Where(n => n.Token.Type == TokenType.Item)
					.Select(i => i.Eval(tree, paramlist) as Rune)
					.ToArray();
		}
		protected override object EvalItem(ParseTree tree, params object[] paramlist)
		{
			string r_name = Nodes.First(n => n.Token.Type == TokenType.WORD).Text;
			var val = Nodes.First(n => n.Token.Type == TokenType.Value).Nodes.First();
			if (val.Token.Type == TokenType.Atom)
			{
				object token;
				var atom = val.Nodes.First();
				if (atom.Token.Type == TokenType.NUMBER)
					token = double.Parse(atom.Text);
				else
					token = atom.Text.Substring(1, atom.Text.Length - 2);
				return new TokenRune(r_name, token);
			}
			else if (val.Token.Type == TokenType.Group)
			{
				new ComplexRune(r_name,
					val.Nodes.Where(n => n.Token.Type == TokenType.Item)
					.Select(i => i.Eval(tree, paramlist) as Rune)
					.ToArray());
			}
			else
				return new Rune(r_name);
			return null;
		}
	}
}
