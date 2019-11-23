using System;
using Microsoft.Xna.Framework;

namespace Indigo.Components
{
	/// <summary>
	/// Simple class that allows a lambda function to run every frame.
	/// </summary>
	public class Actor : Component
	{
		/// <summary>
		/// <para>Constructor.</para>
		/// </summary>
		/// <param name="action">The function to run each frame.</param>
		public Actor(Action action)
		{
			Action = action;
			if (Action == null)
				throw new ArgumentNullException("action");
		}
		
		public override void Update()
		{
			base.Update();
			Action();
		}
		
		private Action Action;
	}
}
