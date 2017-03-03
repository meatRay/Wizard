using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard.Runes.Core
{
	public class Crafter
	{
		public RuneMaster Master { get; private set; }

		protected Crafter( RuneMaster master )
		{
			Master = master;
			Master.ImplementList.Add(this);
		}
	}
}
