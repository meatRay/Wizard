using System;
using static SDL2.SDL;
using static SDL2.SDL_image;

namespace Wizard.Draw
{
	public class Texture : IDraw
	{
		public int Width { get; private set; }
		public int Height { get; private set; }
		internal Texture(IntPtr texture, int width, int height)
		{
			_Texture = texture;
			Width = width;
			Height = height;
		}

		// This is illegal, you know
		public static Texture CreateTexture(Render render, string path)
		{
			var txtr = IMG_LoadTexture(render.Context, path);
			uint format;
			int acc, w, h;
			SDL_QueryTexture(txtr, out format, out acc, out w, out h);
			return new Texture(txtr, w, h);
		}

		public void Draw(Render render, double delta_time)
		{
			Draw(render, 0, 0, 32);
		}
		public void Draw(Render render, int x, int y, int tile_size)
		{
			SDL_Rect rect = new SDL_Rect { x = x/* - ((Width-tile_size)/2)*/, y = y - (Height - tile_size), w = Width, h = Height };
			SDL_RenderCopy(render.Context, _Texture, (IntPtr)null, ref rect);
		}

		IntPtr _Texture;
	}
}
