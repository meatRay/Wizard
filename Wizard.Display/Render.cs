using System;
using static SDL2.SDL;

namespace Wizard.Draw
{
	public class Render : IDisposable
	{
		internal IntPtr Context { get; private set; }

		public static Render CreateRender(IntPtr window_context)
		{
			var rndr = SDL_CreateRenderer(window_context, 0, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
			return new Render(rndr);
		}

		private Render(IntPtr render_context)
		{
			Context = render_context;
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
				}
				SDL_DestroyRenderer(Context);

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		~Render()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
