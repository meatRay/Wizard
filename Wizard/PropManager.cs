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
			_MovedDownProps = new List<Prop>();
			_MovedUpProps = new List<Prop>();
			_SpawnedProps = new List<Prop>();
			_RemovedProps = new List<Prop>();
		}

		public void UpdateLists()
		{
			foreach (var prop in _RemovedProps)
				RemoveReal(prop);
			_RemovedProps.Clear();

			foreach (var prop in _SpawnedProps)
				SpawnReal(prop);
			_SpawnedProps.Clear();
		}

		private void SpawnReal(Prop prop)
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

		private void RemoveReal(Prop prop)
		{
			_Props.Remove(prop);
		}

		public void Spawn(Prop prop)
		{
			_SpawnedProps.Add(prop);
		}

		public void Remove(Prop prop)
		{
			_RemovedProps.Add(prop);
			_MovedDownProps.Remove(prop);
			_MovedUpProps.Remove(prop);
		}

		public bool Move(Prop prop, int direction_x, int direction_y)
			=> Move(prop, new Point(direction_x, direction_y));
		public bool Move(Prop prop, Point direction)
		{
			if (direction.Equals(Point.Zero))
				return false;
			var rslt = prop.Position.Add(direction);
			if (rslt.X < _Context.MinBounds.X || rslt.Y < _Context.MinBounds.Y
				|| rslt.X > _Context.MaxBounds.X || rslt.Y > _Context.MaxBounds.Y)
			{
				var scl = new Point((int)Math.Sign(direction.X), (int)Math.Sign(direction.Y));
				return Move(prop, direction.Subtract(scl));
			}
			var targt = prop.Position.Add(direction);
			// Too much Yoda logic...
			Prop bunp = All.Where(
				p => p != prop
				&& (p.Position.Equals(targt)
				|| p.Bounds.Select(b => b.Add(p.Position)).Any(b => b.Equals(targt)))
			).FirstOrDefault();
			if (bunp != null && bunp.BlocksMove && (!bunp.CanMove || !Move(bunp, direction)))
			{
				var scl = new Point((int)Math.Sign(direction.X), (int)Math.Sign(direction.Y));
				return Move(prop, direction.Subtract(scl));
			}
			prop.SetMove(direction);
			if (direction.Y > 0)
				_MovedDownProps.Add(prop);
			else if (direction.Y < 0)
				_MovedUpProps.Add(prop);

			return true;
		}

		public void SortForRender(bool sort_downs)
		{
			if (sort_downs)
			{
				foreach (var prop in _MovedDownProps)
					if (prop.Direction.Y != 0)
					{
						_Props.Remove(prop);
						int at = _Props.FindIndex(p => p.Position.Y > prop.MovingTo.Y);
						if (at != -1)
							_Props.Insert(at, prop);
						else
							_Props.Add(prop);
					}
				_MovedDownProps.Clear();
			}
			else
			{
				foreach (var prop in _MovedUpProps)
					if (prop.Direction.Y != 0)
					{
						_Props.Remove(prop);
						int at = _Props.FindIndex(p => p.Position.Y > prop.MovingTo.Y);
						if (at != -1)
							_Props.Insert(at, prop);
						else
							_Props.Add(prop);
					}
				_MovedUpProps.Clear();
			}
		}

		private World _Context;
		private List<Prop> _Props;
		//Seperation allows more realistic 3D blending
		private List<Prop> _MovedDownProps;
		private List<Prop> _MovedUpProps;
		private List<Prop> _SpawnedProps;
		private List<Prop> _RemovedProps;
	}
}
