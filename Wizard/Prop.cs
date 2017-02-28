using Wizard.Draw;

namespace Wizard
{
	public class Prop : IDraw
	{
		private const double MoveScale = 1.2;

		public Point Position;
		public Point MovingTo;
		public Point[] Bounds;
		public Texture Texture;
		public bool CanMove = false;
		public bool BlocksMove = true;
		public Point Direction = Point.Zero;


		public Prop(int x, int y, Point[] bounds = null)
			: this(new Point(x, y), bounds)
		{ }

		public Prop(Point position, Point[] bounds = null)
		{
			Position = position;
			MovingTo = position;
			Bounds = bounds ?? new Point[0];
			r = 255;
			b = 255;
			g = 255;
		}

		public void SetMove(Point direction)
		{
			Direction = direction;
			_MoveAnim = 0.0;
			MovingTo = Position.Add(direction);
			Animating = true;
			_Moving = true;
		}

		public virtual void Spawn()
		{
			MovingTo = Position;
		}

		public virtual void Tick()
		{
			Animating = false;
			_MoveAnim = 0.0;
			if (_Moving)
			{
				Position = MovingTo;
				_Moving = false;
			}
			else
				MovingTo = Position;
		}

		private bool _Moving;
		private double _MoveAnim;
		protected bool Animating { get; private set; }
		protected byte r, g, b;

		public void Draw(Render render, double delta_time)
		{
			int x = Position.X * DM.TileSize;
			int y = Position.Y * DM.TileSize;
			if (Animating)
				// Times don't quite match up.. will need to make more clever or impl a timer!
				_MoveAnim += delta_time * (1.0 / DM.TickTime) * MoveScale;
			if (_MoveAnim > 1.0)
				_MoveAnim = 1.0;
			x += (int)((MovingTo.X - Position.X) * DM.TileSize * _MoveAnim);
			y += (int)((MovingTo.Y - Position.Y) * DM.TileSize * _MoveAnim);

			if (Animating && _MoveAnim >= 1.0)
				Animating = false;
			Texture.Draw(render, x, y, DM.TileSize, r, g, b);
		}
	}
}
