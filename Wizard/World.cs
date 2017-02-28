using System;
using System.Collections.Generic;
using System.Linq;
using Wizard.Draw;

namespace Wizard
{
	public class Game : IDisposable
	{
		public static Game Active;
		public Display Display { get; private set; }

		public Game()
		{
			Textures = new Dictionary<string, Texture>();
			Display = Display.CreateDisplay("Wizard.Draw", 100, 100, 800, 576);
		}

		private void Update(double delta_time)
		{
			LoadedWorld.Update(delta_time);
			bombtimr += delta_time;
		}

		double bombtimr = 0.0;

		private void Click(int mouse_button, bool is_down)
		{
			if (bombtimr < 1.0)
				return;
			bombtimr = 0.0;
			var pos = Display.MousePosition();
			int tl_x = (int)Math.Floor(pos.Item1 / (TileSize * RenderScale)) - LoadedWorld.player.Position.X;
			int tl_y = (int)Math.Floor(pos.Item2 / (TileSize * RenderScale)) - LoadedWorld.player.Position.Y;

			var bomb = new Explosion(LoadedWorld.player.Position, 4.0, 6, 4) { Texture = Game.Active.LoadTexture("img/fireball.png") };
			LoadedWorld.Props.Spawn(bomb);
			bomb.SetMove( new Point(tl_x, tl_y) );
		}

		private void Draw(Render render, double delta_time)
		{
			LoadedWorld.Draw(render, delta_time);
		}

		public void Run()
		{
			if (LoadedWorld == null)
				throw new Exception();
			Display.Initialize(Update, Draw);
			Display.OnClick = Click;
			Display.Run();
		}

		public Texture LoadTexture(string name)
		{
			name = name.ToLower();
			if (Textures.ContainsKey(name))
				return Textures[name];
			var tx = Texture.CreateTexture(Display.DrawContext, name);
			Textures.Add(name, tx);
			return tx;
		}

		public void Dispose()
		{
			Display.Dispose();
		}

		public World LoadedWorld;
		public Dictionary<string, Texture> Textures;
		public double RenderScale = 2.0;
		public int TileSize = 32;
		public double TickTime = 0.25;
		public Random Dice = new Random();
	}

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
		public List<IDraw> Background;
		public PropManager Props { get; private set; }
		public Point MinBounds { get; private set; }
		public Point MaxBounds { get; private set; }

		//public Display Display;

		public World()
		{
			Background = new List<IDraw>();
			Props = new PropManager(this);
		}

		public static void Main()
		{
			using (Game.Active = new Game())
			{
				var world = new World();
				Game.Active.LoadedWorld = world;
				world.MinBounds = new Point(0, 0);
				world.MaxBounds = new Point(24, 17);

				var me = new Prop(new Point(4, 3)) { Texture = Game.Active.LoadTexture("img/sprite.png") };
				world.Props.Spawn(me);
				world.player = me;
				var tbl = Game.Active.LoadTexture("img/table_huge.png");
				var prop = new Prop(new Point(3, 2), new Point[] { new Point(1, 0) }) { Texture = tbl, Mass = 2.0 };
				world.Props.Spawn(prop);
				prop = new Prop(new Point(2, 4), new Point[] { new Point(1, 0) }) { Texture = tbl, Mass = 2.0 };
				world.Props.Spawn(prop);

				prop = new Wanderer(new Point(4, 4)) { CanMove = true, Texture = Game.Active.LoadTexture("img/goblin.png") };
				world.Props.Spawn(prop);

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
		public Prop player;
		public void Tick()
		{
			Props.SortForRender(true);
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
