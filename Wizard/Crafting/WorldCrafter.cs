using Wizard.Draw;
using Wizard.Runes;

namespace Wizard.Crafting
{
	class WorldCrafter : RuneCrafter<World>
	{
		// OMFG I can Craft<Texture>()s too
		/*public Game GameContext { get; private set; }*/

		public override World Craft(Rune rune)
		{
			World world = new World();
			PropManagerCrafter.RegisterNew(Master, world);
			ThinkerCrafter.RegisterNew(Master, world);

			var complx = rune as ComplexRune;
			world.Background.Add(
				Master.Craft<Texture>(
					complx.Read<TokenRune>("BACKDROP")
			));

			var r_props = complx.Read<ComplexRune>("PROPS");
			if (r_props != null)
				world.Props = Master.Craft<PropManager>(r_props);

			return world;
		}

		public static void RegisterNew(RuneMaster master/*, Game game_context*/)
			=> new WorldCrafter(master/*, game_context*/);

		protected WorldCrafter(RuneMaster master/*, Game game_context*/)
			: base(master)
		{
			//GameContext = game_context;
		}
	}
}
