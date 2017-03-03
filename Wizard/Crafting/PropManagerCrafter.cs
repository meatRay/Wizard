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
