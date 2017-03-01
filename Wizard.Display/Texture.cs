using System;
using static SDL2.SDL;
using static SDL2.SDL_image;

namespace Wizard.Draw
{
	public class Texture : IDraw
	{
		[Flags]
		public enum Clips { NONE, XCENTRE, YCENTRE }
		public int Width { get; private set; }
		public int Height { get; private set; }
		internal Texture(IntPtr texture, int width, int height)
		{
			_Texture = texture;
			Width = width;
			Height = height;
		}
		public Clips centre = Clips.NONE;

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
			SDL_Rect rect = new SDL_Rect { x = -render.CameraX, y = -render.CameraY, w = Width, h = Height };
			SDL_RenderCopy(render.Context, _Texture, (IntPtr)null, ref rect);
		}
		public void Draw(Render render, int x, int y, int tile_size)
		{
			Draw(render, x, y, tile_size, 255, 255, 255);
		}
		public void Draw(Render render, int x, int y, int tile_size, byte r, byte g, byte b)
		{
			SDL_SetTextureColorMod(_Texture, r, g, b);
			if (centre.HasFlag(Clips.XCENTRE))
				x -= (Width - tile_size) / 2;
			if (centre.HasFlag(Clips.XCENTRE))
				y -= (Height - tile_size) / 2;
			else
				y -= (Height - tile_size);

			x -= render.CameraX;
			y -= render.CameraY;

			SDL_Rect rect = new SDL_Rect
			{
				x = x,
				y = y,
				w = Width,
				h = Height
			};
			SDL_RenderCopy(render.Context, _Texture, (IntPtr)null, ref rect);
		}
		IntPtr _Texture;
	}
}
