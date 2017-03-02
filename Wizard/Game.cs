using System;
using System.Collections.Generic;
using Wizard.Crafting;
using Wizard.Draw;
using Wizard.Runes;

namespace Wizard
{
	public class Game : IDisposable
	{
		public static Game Active;
		public Display Display { get; private set; }
		public RuneMaster Content { get; private set; }

		public Game()
		{
			Textures = new Dictionary<string, Texture>();
			Display = Display.CreateDisplay("Wizard.Draw", 100, 100, 768, 576);

			Content = new RuneMaster();
			Content.Implementations.Add(new WorldCrafter(this));
		}

		private void Update(double delta_time)
		{
			LoadedWorld.Update(delta_time);
			bombtimr += delta_time;

			var pos = Display.MousePosition();
			int tl_x = (int)Math.Floor((pos.Item1 + Display.DrawContext.CameraX) / (TileSize * Display.DrawContext.RenderScale));
			int tl_y = (int)Math.Floor((pos.Item2 + Display.DrawContext.CameraY) / (TileSize * Display.DrawContext.RenderScale));
			mouse.Position = new Point(tl_x, tl_y);

			if (Display.KeyDown("w"))
				--Display.DrawContext.CameraY;
			else if (Display.KeyDown("s"))
				++Display.DrawContext.CameraY;
			if (Display.KeyDown("d"))
				++Display.DrawContext.CameraX;
			else if (Display.KeyDown("a"))
				--Display.DrawContext.CameraX;

		}
		Prop mouse;
		double bombtimr = 0.0;

		private void Click(int mouse_button, bool is_down)
		{
			var pos = Display.MousePosition();
			int tl_x = (int)Math.Floor((pos.Item1 + Display.DrawContext.CameraX) / (TileSize * Display.DrawContext.RenderScale)) - LoadedWorld.player.Position.X;
			int tl_y = (int)Math.Floor((pos.Item2 + Display.DrawContext.CameraY) / (TileSize * Display.DrawContext.RenderScale)) - LoadedWorld.player.Position.Y;
			if (mouse_button == 1 && is_down)
			{
				Point tl = new Point(tl_x, tl_y).Add(LoadedWorld.player.Position);
				LoadedWorld.player.Path.Clear();
				Point togo = LoadedWorld.player.Position;
				while (!togo.Equals(tl))
				{
					Point dif = tl.Subtract(togo);
					dif = new Point(Math.Sign(dif.X), Math.Sign(dif.Y));
					LoadedWorld.player.Path.Add(dif);
					togo = togo.Add(dif);
				}
			}
			if (mouse_button == 3 && is_down)
			{
				if (bombtimr < 1.0)
					return;
				bombtimr = 0.0;

				var bomb = new Explosion(LoadedWorld.player.Position, 4.0, 6, 4) { Texture = Game.Active.LoadTexture("img/fireball.png") };
				LoadedWorld.Props.Spawn(bomb);
				bomb.SetMove(new Point(tl_x, tl_y));
			}
		}

		private void Draw(Render render, double delta_time)
		{
			LoadedWorld.Draw(render, delta_time);
			foreach (var effect in Effects)
				effect.Draw(render, delta_time);
		}

		public void Run()
		{
			if (LoadedWorld == null)
				throw new Exception();
			mouse = new Prop(0, 0) { Texture = LoadTexture("img/cursor.png") };
			Effects.Add(mouse);
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
		public List<IDraw> Effects = new List<IDraw>();
		public int TileSize = 64;
		public double TickTime = 0.25;
		public Random Dice = new Random();
	}
}
