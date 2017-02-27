using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard
{
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
