using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wizard.Draw;
using Wizard.Runes;

namespace Wizard
{
	public static class Directions
	{
		private static Point[] _dirs { get; } = new Point[8]
		{
			new Point(0, -1),new Point(1, -1),new Point(1, 0),new Point(1, 1),
			new Point(0, 1),new Point(-1, 1),new Point(-1, 0),new Point(-1, -1),
		};

		public static Point North { get; } = _dirs[0];
		public static Point NorthEast { get; } = _dirs[1];
		public static Point East { get; } = _dirs[2];
		public static Point SouthEast { get; } = _dirs[3];
		public static Point South { get; } = _dirs[4];
		public static Point SouthWest { get; } = _dirs[5];
		public static Point West { get; } = _dirs[6];
		public static Point NorthWest { get; } = _dirs[7];

		public enum Angle { NONE, CW45 = 1, CW90 = 2, CCW45 = -1, CCW90 = -2 }

		public static Point Rotate(Point direction, Angle angle)
		{
			// Forgit me father for I have sinned.
			// I should import a real math lib
			var at = Array.FindIndex(_dirs, d => direction.Equals(d));
			int togo = (int)angle;
			if (at + togo < 0)
				at = 8 - (0 - (at + togo));
			else if (at + togo > 7)
				at = ((at + togo) - 8);
			else
				at += togo;
			return _dirs[at];
		}
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

	public class World : IDraw
	{
		public List<Texture> Background;
		public PropManager Props { get; internal set; }
		public Point MinBounds { get; private set; }
		public Point MaxBounds { get; private set; }

		//public Display Display;

		public World()
		{
			Background = new List<Texture>();
			Props = new PropManager(this);
		}

		public static void Main()
		{
			using (Game.Active = new Game())
			{
				var rune = Rune.CreateFrom( File.ReadAllText("data/world.rne"), "data/world.rne" );
				var world = Game.Active.Content.Craft<World>( rune.Read("WORLD") );

				//var world = new World();
				Game.Active.LoadedWorld = world;
				world.MinBounds = new Point(1, 1);
				world.MaxBounds = new Point((int)(10 / Game.Active.Display.DrawContext.RenderScale), (int)(7 / Game.Active.Display.DrawContext.RenderScale));

				world.player = world.Props.All.Select(p => p as Thinker).First(p => p != null);

				Game.Active.Run();
			}
		}

		public void Draw(Render render, double delta_time)
		{
			foreach (var tile in Background)
				tile.Draw(render, delta_time);
			foreach (var prop in Props.All/*.OrderBy(p => p.Position.Y)*/)
				prop.Draw(render, delta_time);
		}
		public Thinker player;
		public void Tick()
		{
			Props.SortForRender(true);
			foreach (var prop in Props.All)
				prop.Tick();

			Props.UpdateLists();

			Props.SortForRender(false);
		}

		public static World GenField()
		{
			World world = new World();
			world.Background.Add(Game.Active.LoadTexture("img/backdrop.png"));
			var tex = Game.Active.LoadTexture("img/bush.png");
			world.Props.Spawn(new Prop(7, 8) { Texture = tex, CanMove = false });
			world.Props.Spawn(new Prop(19, 8) { Texture = tex, CanMove = false });
			world.Props.Spawn(new Prop(6, 16) { Texture = tex, CanMove = false });
			world.Props.Spawn(new Prop(18, 18) { Texture = tex, CanMove = false });

			return world;
		}

		[Flags]
		enum pinput { none, right, left, up = 4, down = 8 }
		pinput ontick;

		private double _TickingTime = 0.0;
		public void Update(double delta_time)
		{
			_TickingTime += delta_time;
			if (_TickingTime >= Game.Active.TickTime)
			{
				_TickingTime -= Game.Active.TickTime;
				Tick();
			}

			/*if (Display.KeyDown("d"))
				ontick = pinput.right;
			else if (Display.KeyDown("a"))
				ontick = pinput.left;
			if (Display.KeyDown("w"))
				ontick |= pinput.up;
			else if (Display.KeyDown("s"))
				ontick |= pinput.down;*/
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
					//Display.Dispose();
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
