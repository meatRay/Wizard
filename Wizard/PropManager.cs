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
			// Too much Yoda logic...
			Prop bunp = All.Where(
				p => p != prop
				&& (p.Position.Equals(targt)
				|| p.Bounds.Select(b => b.Add(p.Position)).Any(b => b.Equals(targt)))
			).FirstOrDefault();
			if (bunp != null && bunp.BlocksMove && (!bunp.CanMove || Move(bunp, direction)) )
				return false;
			prop.SetMove(direction);
			_MovedProps.Add(prop);

			return true;
		}

		public void SortForRender()
		{
			foreach (var prop in _MovedProps)
			{
				if (prop.Direction.Y != 0)
				{
					_Props.Remove(prop);
					int at = _Props.FindIndex(p => p.Position.Y > prop.MovingTo.Y);
					if (at != -1)
						_Props.Insert(at, prop);
					else
						_Props.Add(prop);
				}
			}
			_MovedProps.Clear();
		}

		private World _Context;
		private List<Prop> _Props;
		private List<Prop> _MovedProps;
	}
}
