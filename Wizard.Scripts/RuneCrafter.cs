using System;
using System.Collections.Generic;
using Wizard.Runes.Core;

namespace Wizard.Runes
{
	public class RuneMaster
	{
		public IEnumerable<Crafter> Implementations => ImplementList;

		public RuneMaster()
		{
			ImplementList = new List<Crafter>();
		}

		public T Craft<T>(Rune rune)
		{
			RuneCrafter<T> crft = null;
			foreach( var impl in Implementations )
			{
				crft = impl as RuneCrafter<T>;
				if (crft != null)
					return crft.Craft(rune);
			}
			throw new Exception($"No implemention for crafting {typeof(T).Name} loaded");
		}

		internal List<Crafter> ImplementList;
	}

	public abstract class RuneCrafter<T> : Crafter
	{
		protected RuneCrafter(RuneMaster master)
			:base(master)
		{ }

		public abstract T Craft(Rune rune);
	}
}
