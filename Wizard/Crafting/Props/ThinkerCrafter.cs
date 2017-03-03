using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wizard.Draw;
using Wizard.Runes;

namespace Wizard.Crafting
{
	class ThinkerCrafter : RuneCrafter<Thinker>
	{
		public World WorldContext { get; private set; }
		protected ThinkerCrafter(RuneMaster master, World world_context) 
			: base(master)
		{
			WorldContext = world_context;
		}

		public override Thinker Craft(Rune rune)
		{ 
			var complx = rune as ComplexRune;
			Point p = new Point(
				complx.Read<TokenRune>("X").Read<int>(),
				complx.Read<TokenRune>("Y").Read<int>()
			);
			Point[] bounds = null;
			var r_bnds = complx.Read<ComplexRune>("BOUNDS");
			if (r_bnds != null)
			{
				// More just practicing LINQ-FU than anything useful..
				var r_pnts = r_bnds.SubRunes.Where(
					r => r.Word.Equals("X", StringComparison.OrdinalIgnoreCase)
					|| r.Word.Equals("Y", StringComparison.OrdinalIgnoreCase)
				).Cast<TokenRune>()
					.GroupBy(r => r.Word);
				bounds = r_pnts.First(r => r.Key == "X")
					.Zip(r_pnts.First(r => r.Key == "Y"), (f, s) => new { First = f, Second = s })
					.Select(a => new Point(a.First.Read<int>(), a.Second.Read<int>()))
					.ToArray();
			}

			var prop = new Thinker(p, bounds);

			if (complx.Read("PLAYER") != null)
				WorldContext.player = prop;

			prop.Texture = Master.Craft<Texture>(complx.Read<TokenRune>("TEXTURE"));

			var r_mass = complx.Read<TokenRune>("MASS");
			if (r_mass != null)
				prop.Mass = r_mass.Read<int>();

			return prop;
		}

		public static void RegisterNew(RuneMaster master, World world_context)
			=> new ThinkerCrafter(master, world_context);
	}
}
