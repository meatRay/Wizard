using Wizard.Draw;
using Wizard.Runes;

namespace Wizard.Crafting
{
	class TextureCrafter : RuneCrafter<Texture>
	{
		public Game GameContext { get; private set; }

		public override Texture Craft(Rune rune)
			=> GameContext.LoadTexture((rune as TokenRune).Read<string>());

		public static void RegisterNew(RuneMaster master, Game game_context)
			=> new TextureCrafter(master, game_context);

		protected TextureCrafter(RuneMaster master, Game game_context)
			: base(master)
		{
			GameContext = game_context;
		}
	}
}
