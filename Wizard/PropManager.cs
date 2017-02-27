using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
	public class PropManager
	{
		public IEnumerable<Prop> All => _Props;
		public PropManager(World context)
		{
			_Context = context;
			_Props = new List<Prop>();
			_MovedProps = new List<Prop>();
		}

		public void Spawn(Prop prop)
		{
			var think = prop as Thinker;
			if (think != null)
				think.Spawn(_Context);
			else
				prop.Spawn();
			int at = _Props.FindIndex(p => p.Position.Y > prop.Position.Y);
			if (at != -1)
				_Props.Insert(at, prop);
			else
				_Props.Add(prop);
		}

		public bool Move(Prop prop, int direction_x, int direction_y)
			=> Move(prop, new Point(direction_x, direction_y));
		public bool Move(Prop prop, Point direction)
		{
			if (direction.Equals(Point.Zero))
				return false;
			var targt = prop.Position.Add(direction);
			Prop bunp = All.Where(
				p => p != prop
				&& (p.Position.Equals(targt)
				|| p.Bounds.Select(b => b.Add(p.Position)).Any(b => b.Equals(targt)))
			).FirstOrDefault();
			if (bunp != null && !Move(bunp, direction))
				return false;
			prop.SetMove(direction);

			return true;
		}

		public void SortForRender()
		{
			foreach (var prop in _MovedProps)
			{
				if (prop.MovingTo.Y != 0)
				{
					var t_y = prop.Position.Y + prop.MovingTo.Y;
					_Props.Remove(prop);
					_Props.Insert(_Props.FindIndex(p => p.Position.Y > t_y), prop);
				}
			}
			_MovedProps.Clear();
		}

		private World _Context;
		private List<Prop> _Props;
		private List<Prop> _MovedProps;
	}
}
