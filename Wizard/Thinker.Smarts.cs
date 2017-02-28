using System;
using System.Collections.Generic;
using System.Linq;
using Wizard;
using Wizard.Draw;

namespace Wizard.Helpers
{
	public class Creature : Thinker
	{
		public string TextureName { get; protected set; }

		public override void Spawn(World world_context)
		{
			Texture = Texture.CreateTexture(Game.Active.Display.DrawContext, "img/"+TextureName);
			base.Spawn(world_context);
		}
	}

	public static class ThinkerExtentions
    {

		public static IEnumerable<Prop> VisibleTo(this PropManager manager, Thinker thinker)
			=> manager.All.Where(p => p.Position.Subtract(thinker.Position).Length() < thinker.Sight);
		public static T ClosestVisible<T>(this PropManager manager, Thinker to) where T : Prop
			=> manager.VisibleTo(to)
				.Select(p => p as T)
				.Where(p => p != null)
				.OrderBy(t => t.Position.Subtract(to.Position).Length())
				.FirstOrDefault();
    }
}
