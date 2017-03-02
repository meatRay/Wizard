using System;
using System.Collections.Generic;
using System.Linq;
using Wizard.Runes.TinyPG;

namespace Wizard.Runes
{
	public class Rune
	{
		public string Word { get; private set; }

		public Rune(string word)
		{
			Word = word;
		}
		
		public static Rune[] CreateFrom( string input, string file_name )
		{
			RuneParser = new Parser(new Scanner());
			ParseTree tree = new RuneTree();
			tree = RuneParser.Parse(input, file_name, tree);
			return tree.Eval(null) as Rune[];
		}

		private static Parser RuneParser;
	}
	public class TokenRune : Rune
	{
		public TokenRune(string r_name, object token)
			: base(r_name)
		{
			_token = token;
		}
		public T Read<T>()
			=> (T)_token;

		private object _token;
	}
	public class ComplexRune : Rune
	{
		public Rune[] SubRunes { get; private set; }
		public ComplexRune(string word, Rune[] subrunes)
			: base(word)
		{
			SubRunes = subrunes;
		}

		public Rune Read(string rune_name)
			=> SubRunes.Where(r => r.Word.Equals(rune_name, StringComparison.OrdinalIgnoreCase))
				.FirstOrDefault();
		public IEnumerable<Rune> ReadAll(string rune_name)
			=> SubRunes.Where(r => r.Word.Equals(rune_name, StringComparison.OrdinalIgnoreCase));

		public R Read<R>(string rune_name) where R : Rune
			=> Read(rune_name) as R;
		public IEnumerable<R> ReadAll<R>(string rune_name) where R : Rune
			=> ReadAll(rune_name).Cast<R>();
	}
}
