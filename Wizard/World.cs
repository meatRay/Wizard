using System;
using System.Collections.Generic;
using System.Linq;
using Wizard.Draw;

namespace Wizard
{
	class DM
	{
		public static int TileSize = 32;
		public static double TickTime = 1.0;
	}

	//Lifting the one from System.Drawing means importing the entire dll..
	//Investigate System.Numerics?
	public struct Point : IEquatable<Point>
	{
		public int X { get; private set; }
		public int Y { get; private set; }

		public Point(int x, int y)
		{
			X = x;
			Y = y;
		}

		public bool Equals(Point other)
			=> X == other.X && Y == other.Y;
		public Point Add(Point other)
			=> new Point(X + other.X, Y + other.Y);
		public Point Subtract(Point other)
			=> new Point(X - other.X, Y - other.Y);
		public double Length()
			=> Math.Sqrt((X * X) + (Y * Y));

		public static Point Zero { get; } = new Point(0, 0);
	}

	public class World : IDraw, IDisposable
	{
		public List<IDraw> Background;
		public PropManager Props { get; private set; }
		public Display Display;

		public World(Display display)
		{
			Background = new List<IDraw>();
			Props = new PropManager(this);
			Display = display;

		}

		public void Run()
		{
			Display.Initialize(d => Update(d), (r, d) => Draw(r, d));
			Display.Run();
		}

		public static void Main()
		{
			using (var world = new World(Display.CreateDisplay()))
			{
				var me = new Prop(new Point(0, 0)) { Texture = Texture.CreateTexture(world.Display.DrawContext, "img/sprite.png") };
				world.Props.Spawn(me);
				world.player = me;
				var tbl = Texture.CreateTexture(world.Display.DrawContext, "img/table_huge.png");
				var prop = new Prop(new Point(3, 2), new Point[] { new Point(1, 0) }) { Texture = tbl };
				world.Props.Spawn(prop);
				prop = new Prop(new Point(3, 6), new Point[] { new Point(1, 0) }) { Texture = tbl };
				world.Props.Spawn(prop);
				prop = new Prop(new Point(5, 2), new Point[] { new Point(1, 0) }) { Texture = tbl };
				world.Props.Spawn(prop);
				prop = new Prop(new Point(5, 6), new Point[] { new Point(1, 0) }) { Texture = tbl };
				world.Props.Spawn(prop);
				prop = new Prop(new Point(2, 4), new Point[] { new Point(1, 0) }) { Texture = tbl };
				world.Props.Spawn(prop);
				prop = new Prop(new Point(6, 4), new Point[] { new Point(1, 0) }) { Texture = tbl };
				world.Props.Spawn(prop);
				prop = new Wanderer(new Point(4, 4)) { Texture = Texture.CreateTexture(world.Display.DrawContext, "img/goblin.png") };
				world.Props.Spawn(prop);

				world.Run();
			}
		}

		public void Draw(Render render, double delta_time)
		{
			foreach (var tile in Background)
				tile.Draw(render, delta_time);
			foreach (var prop in Props.All/*.OrderBy(p => p.Position.Y)*/)
				prop.Draw(render, delta_time);
		}
		public Prop player;
		public void Tick()
		{
			foreach (var prop in Props.All)
				prop.Tick();
			int x = 0, y = 0;
			if (ontick.HasFlag(pinput.right))
				x = 1;
			else if (ontick.HasFlag(pinput.left))
				x = -1;
			if (ontick.HasFlag(pinput.up))
				y = -1;
			else if (ontick.HasFlag(pinput.down))
				y = 1;
			if (player != null)
				Props.Move(player, x, y);
			ontick = pinput.none;
			Props.SortForRender();
		}

		[Flags]
		enum pinput { none, right, left, up = 4, down = 8 }
		pinput ontick;

		private double _TickingTime = 0.0;
		public void Update(double delta_time)
		{
			_TickingTime += delta_time;
			if (_TickingTime >= DM.TickTime)
			{
				_TickingTime -= DM.TickTime;
				Tick();
			}

			if (Display.KeyDown("d"))
				ontick = pinput.right;
			else if (Display.KeyDown("a"))
				ontick = pinput.left;
			if (Display.KeyDown("w"))
				ontick |= pinput.up;
			else if (Display.KeyDown("s"))
				ontick |= pinput.down;
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
					Display.Dispose();
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~World() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}
		#endregion
	}
}
