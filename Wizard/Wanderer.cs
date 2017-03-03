using System;

namespace Wizard
{
	class Wanderer : Thinker
	{
		public Wanderer()
			: this(0, 0)
		{ }

		public Wanderer(int x, int y, Point[] bounds = null)
			: base(new Point(x, y), bounds)
		{ }

		public Wanderer(Point position, Point[] bounds = null)
			: base(position, bounds)
		{ }

		public override void Think(World context)
		{
			if (Game.Active.Dice.Next(4) == 0)
			{
				var dir = new Point(Game.Active.Dice.Next(-1, 2), Game.Active.Dice.Next(-1, 2));
				context.Props.Move(this, dir);
			}
		}
	}
}
