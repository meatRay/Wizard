using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wizard.Runes;

namespace Wizard.Crafting
{
	class WorldCrafter : RuneCrafter<World>
	{
		public Game GameContext { get; private set; }

		public WorldCrafter(Game game_context)
		{
			GameContext = game_context;
		}
		public override World Craft(Rune rune)
		{
			World world = new World();

			var complx = rune as ComplexRune;

			world.Background.Add(
				GameContext.LoadTexture(
					complx.Read<TokenRune>("BACKDROP").Read<string>()
			));

			return world;
		}
	}
}
