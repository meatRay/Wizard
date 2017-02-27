using static SDL2.SDL;

namespace Wizard.Draw
{
	public class Block : IDraw
	{
		public void Draw(Render render, double delta_time)
		{
			Draw(render, 0, 0, 100, 100);
		}
		public void Draw(Render render, int x, int y, int w, int h)
		{
			SDL_Rect rect = new SDL_Rect { x = x, y = y, w = w, h = h };
			SDL_SetRenderDrawColor(render.Context, 255, 0, 0, 255);
			SDL_RenderFillRect(render.Context, ref rect);
		}
	}
}
