using System.Collections.Generic;
using System.Linq;
using Wizard.Draw;

namespace Wizard.Helpers
{
	public class Creature : Thinker
	{
		public string TextureName { get; private set; }

		public override void Spawn(World world_context)
		{
			Texture = Texture
			base.Spawn(world_context);
		}
	}
    public static class ThinkerExtentions
    {
		/// <summary>
		/// QUICK RECT
		/// </summary>
		/// <param name="me"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public static IEnumerable<Prop> PropsInSight(this Thinker me, World context)
			=> context.Props.All.Where(p => p.Position.Subtract(me.Position).Length() < me.Sight);
    }
}
