using Indigo.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Indigo.Components
{
	//	Based on code from here: http://indiehouse.tumblr.com/post/45325908888/custom-coroutines-in-c
	
	/// <summary>
	/// Simple coroutine manager.
	/// </summary>
	public class CoroutineHost : Component
	{
		/// <summary>
		/// Causes the calling coroutine to wait for a given number of seconds.
		/// </summary>
		/// <param name="seconds">The amount of seconds to wait.</param>
		/// <returns>IEnumerator</returns>
		public static IEnumerator WaitForSeconds(float seconds)
		{
            var watch = Stopwatch.StartNew();
            while (watch.Elapsed.TotalSeconds < seconds)
                yield return null;
		}
		
		/// <summary>Causes the calling coroutine to wait for a given number of frames.
		/// <para>If you want to wait a single frame, it's better to use <code>yield return null;</code></para>
		/// </summary>
		/// <param name="frames">The number of frames to wait.</param>
		/// <returns>IEnumerator</returns>
		public static IEnumerator WaitForFrames(int frames)
		{
			int elapsed = 0;
            while (elapsed < frames)
            {
                elapsed++;
                yield return 0;
            }
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public CoroutineHost()
		{
			routines = new List<IEnumerator>();
			messages = new List<Enum>();
		}
		
		/// <summary>
		/// Start a new coroutine.
		/// </summary>
		/// <param name="routine">An IEnumerator (created by calling a function which yields).</param>
		/// <returns>The coroutine that was started, used to stop it running.</returns>
		public IEnumerator Start(IEnumerator routine)
		{
			if (routines.Contains(routine))
				throw new Exception("Routine added while it was already running!");
			
			routines.Add(routine);
			return routine;
		}
		
		/// <summary>
		/// Stop the given coroutine.
		/// </summary>
		/// <param name="routine">The coroutine to stop.</param>
		/// <returns>Whether the stopped coroutine was contained in this host.</returns>
		public bool Stop(IEnumerator routine)
		{
			if (routines.Remove(routine))
			{
				//	maybe do processing here?
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Stop all coroutines in the host.
		/// </summary>
		public void StopAll()
		{
			routines.Clear();
		}
		
		public override void Update()
		{
			base.Update();
			
			for (int i = 0; i < routines.Count; ++i)
			{
				Current = routines[i];
				if (routines[i].Current is IEnumerator)
				{
					if (MoveNext((IEnumerator) routines[i].Current))
						continue;
				}
				
				if (!routines[i].MoveNext())
				{
					routines.RemoveAt(i--);
				}
			}
			
			Current = null;
			
			messages.Clear();
		}
		
		private bool MoveNext(IEnumerator routine)
		{
			if (routine.Current is IEnumerator)
			{
				if (MoveNext((IEnumerator) routine.Current))
					return true;
			}
			
			return routine.MoveNext();
		}
		
		/// <summary>
		/// How many coroutines are currently running.
		/// </summary>
		public int Count { get { return routines.Count; } }
		
		/// <summary>
		/// Whether any coroutines are currently running.
		/// </summary>
		public bool Running { get { return Count > 0; } }
		
		/// <summary>
		/// The currently executing routine.
		/// </summary>
		public IEnumerator Current { get; private set; }
		
		private List<IEnumerator> routines;
		private List<Enum> messages;
	}
}
