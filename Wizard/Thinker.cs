using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
	public class Thinker : Prop
	{
		public double Sight = 10;
		public bool Alive { get; private set; } = true;

		public List<Point> Path = new List<Point>();

		public void Kill()
		{
			Alive = false;
			BlocksMove = false;
			CanMove = false;
			if (!Animating)
				MovingTo = Position;
			g = 50;
			b = 50;
			r = 100;
		}

		public Thinker()
			: this(0, 0)
		{ }

		public Thinker(int x, int y, Point[] bounds = null)
			: base(new Point(x, y), bounds)
		{ }

		public Thinker(Point position, Point[] bounds = null)
			: base(position, bounds)
		{ }

		public virtual void Think(World context)
		{ }

		public override void Tick()
		{
			if (Alive)
			{
				base.Tick();
				if ( Path.Count > 0 && !Animating)
				{
					_Context.Props.Move(this, Path.First());
					Path.RemoveAt(0);
				}
				Think(_Context);
			}
		}

		public virtual void Spawn(World world_context)
		{
			Spawn();
			_Context = world_context;
		}

		private World _Context;
	}
}
