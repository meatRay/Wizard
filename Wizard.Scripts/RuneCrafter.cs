using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wizard.Runes.Core;

namespace Wizard.Runes
{
	public class RuneMaster
	{
		// loaded runes...
		public List<Crafter> Implementations;

		public RuneMaster()
		{
			Implementations = new List<Crafter>();
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
	}

	public abstract class RuneCrafter<T> : Crafter
	{
		public abstract T Craft(Rune rune);
	}
}
