using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wizard.Runes;

namespace Wizard.Crafting
{
	class PropManagerCrafter : RuneCrafter<PropManager>
	{
		public World WorldContext { get; private set; }

		public override PropManager Craft(Rune rune)
		{
			var manager = new PropManager(WorldContext);

			var complx = rune as ComplexRune;

			foreach (var r_prop in complx.ReadAll<ComplexRune>("PROP"))
				manager.Spawn(Master.Craft<Prop>(r_prop));
			foreach (var r_prop in complx.ReadAll<ComplexRune>("THINKER"))
				manager.Spawn(Master.Craft<Thinker>(r_prop));
			foreach (var r_prop in complx.ReadAll<ComplexRune>("WANDERER"))
				manager.Spawn(Master.Craft<Wanderer>(r_prop));

			manager.UpdateLists();

			return manager;
		}

		public static void RegisterNew(RuneMaster master, World world_context)
		{
			new PropManagerCrafter(master, world_context);
		}

		protected PropManagerCrafter(RuneMaster master, World world_context)
			: base(master)
		{
			WorldContext = world_context;
		}
	}
}
