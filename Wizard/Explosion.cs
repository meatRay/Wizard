﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
	class Explosion : Thinker
	{
		public double Force { get; private set; }
		public int Lifetime { get; private set; }
		public int LifeRemaining { get; private set; }

		public Explosion(int x, int y, double force, int lifetime, int force_at)
			: this(new Point(x, y), force, lifetime, force_at)
		{ }

		public Explosion(Point position, double force, int lifetime, int force_at)
			:base(position, null)
		{
			Force = force;
			Lifetime = lifetime;
			_forceat = force_at;
		}

		public override void Spawn()
		{
			base.Spawn();
			LifeRemaining = Lifetime;
			_forceremain = _forceat;
		}

		public override void Think(World context)
		{
			base.Think(context);
			if (--LifeRemaining <= 0)
				context.Props.Remove(this);
			if (--_forceremain == 0)
			{
				foreach( var prop in context.Props.All.Where(p => p.Position.Subtract(Position).Length() <= Force))
				{
					var dir = prop.Position.Subtract(Position);
					var t_ply = Force - dir.Length() + 1 - prop.Mass;
					if (t_ply <= 0)
						continue;
					var go = new Point((int)(dir.X*t_ply), (int)(dir.Y*t_ply));
					context.Props.Move(prop, go);
				}
			}
		}

		private int _forceat;
		private int _forceremain;
	}
}