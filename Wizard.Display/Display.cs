using System;
using System.Threading;
using SDL2;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;
using System.Runtime.InteropServices;

namespace Wizard.Draw
{	
	public delegate void UpdateJob(double delta_time);
	public delegate void DrawJob(Render render, double delta_time);
	public class Display : IDisposable
	{
		public Render DrawContext { get; private set; }
		public bool Running { get; private set; }

		public UpdateJob Update { get; private set; }
		public DrawJob Draw { get; private set; }

		public void Initialize( UpdateJob update_job, DrawJob draw_job )
		{
			Update = update_job;
			Draw = draw_job;
		}
		public void Run()
		{
			Running = true;
			CoreUpdate();
		}

		private void CoreDraw(double delta_time)
		{
			SDL_SetRenderDrawColor(DrawContext.Context, 255, 255, 255, 255);
			SDL_RenderClear(DrawContext.Context);

			Draw?.Invoke(DrawContext, delta_time);

			SDL_RenderPresent(DrawContext.Context);
		}

		private void CoreUpdate()
		{
			SDL_Event curevent;
			while ( Running )
			{
				while( SDL_PollEvent(out curevent) > 0 )
				{
					DoEvent(ref curevent);
				}
				Update?.Invoke(_CoreDelay.TotalSeconds);
				CoreDraw(_CoreDelay.TotalSeconds);
				Thread.Sleep(_CoreDelay);
			}
		}
		private TimeSpan _CoreDelay = TimeSpan.FromMilliseconds(16.6666);

		private void DoEvent(ref SDL_Event to_do)
		{
			switch( to_do.type )
			{
				case SDL_EventType.SDL_QUIT:
					Quit();
					break;
				default:
					break;
			}
		}
		public bool KeyDown( string key )
		{
			var sc = SDL_GetScancodeFromName(key);
			int a;
			var ptr = SDL_GetKeyboardState(out a);
			ptr += (int)sc;
			var got = Marshal.PtrToStructure<byte>(ptr);
			return got > 0;
		}

		public void Quit() => Running = false;

		public static Display CreateDisplay(bool create_render = true)
		{
			var wndw = SDL_CreateWindow("Testing Window", 100, 100, 800, 600, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
			var dspl = new Display(wndw);
			if (create_render)
				dspl.DrawContext = Render.CreateRender(wndw);
			if (IMG_Init(IMG_InitFlags.IMG_INIT_PNG) == 0)
				throw new Exception("Image Init Exception");
			if (TTF_Init() == -1)
				throw new Exception("Text Init Exception");
			++DisplayCount;
			return dspl;
		}

		private Display( IntPtr window_handle )
		{
			_Window = window_handle;
		}

		private IntPtr _Window;

		private static int DisplayCount = 0;

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					DrawContext.Dispose();
				}
				SDL_DestroyWindow(_Window);
				if (--DisplayCount==0)
				{
					IMG_Quit();
					TTF_Quit();
				}

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~Display() { Dispose(false); }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
