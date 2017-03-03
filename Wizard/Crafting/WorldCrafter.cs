using Wizard.Runes;

namespace Wizard.Crafting
{
	class WorldCrafter : RuneCrafter<World>
	{
		public Game GameContext { get; private set; }

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

		public static void RegisterNew(RuneMaster master, Game game_context)
			=> new WorldCrafter(master, game_context);

		protected WorldCrafter(RuneMaster master, Game game_context)
			: base(master)
		{
			GameContext = game_context;
		}
	}
}
