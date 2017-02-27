using System;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;

namespace Wizard.Draw
{
	public static class Text
	{
		private static IntPtr Font;
		private static SDL_Color Color = new SDL_Color() { r = 0, b = 0, g = 0 };

		// This is illegal, you know
		public static Texture CreateText(Render render, string text)
		{
			if (Font == null || Font == IntPtr.Zero)
				Font = TTF_OpenFont("C:/Windows/Fonts/Arial.ttf", 16);
			var srfc = TTF_RenderUNICODE_Blended(Font, text, Color);
			var txtr = SDL_CreateTextureFromSurface(render.Context, srfc);
			SDL_FreeSurface(srfc);
			uint format;
			int acc, w, h;
			SDL_QueryTexture(txtr, out format, out acc, out w, out h);
			return new Texture(txtr, w, h);
		}
	}
}
