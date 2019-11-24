using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Indigo.Core;
using Indigo.Utility;

namespace Indigo.Components
{
	public class Spliner : Component
	{
		/// <summary>
		/// The time remaining before the spline ends or repeats.
		/// </summary>
        public float TimeRemaining { get { return Duration - time; } }
        
        /// <summary>
        /// A value between 0 and 1, where 0 means the spline has not been started and 1 means that it has completed.
        /// </summary>
        public float Completion { get { var c = time / Duration; return c < 0 ? 0 : (c > 1 ? 1 : c); } }
        
        /// <summary>
        /// Whether the spline is currently looping.
        /// </summary>
        public bool Looping { get { return repeatCount != 0; } }
        
        /// <summary>
        /// The object this spline targets.
        /// </summary>
        public object Target { get; private set; }
		
		/// <summary>
		/// Constructor. Does no actual work; call Spline() to set up motion.
		/// </summary>
		public Spliner()
		{
			Splines = new List<Spline>();
		}
		/// <summary>
		/// Set the motion for this spline.
		/// </summary>
		/// <param name="target">The object the motion should target. Must be non-null and a reference type.</param>
		/// <param name="duration">How long the motion should take, in seconds.</param>
		/// <param name="points">The points that the motion should spline between.</param>
		/// <returns>A reference to this, for specifying additional parameters.</returns>
		public Spliner Spline<TTarget, TPoint>(TTarget target, float duration, IEnumerable<TPoint> points) where TTarget : class
		{
			return Spline(target, duration, points.ToArray());
		}
		
		/// <summary>
		/// Set the motion for this spline.
		/// </summary>
		/// <param name="target">The object the motion should target. Must be non-null and a reference type.</param>
		/// <param name="duration">How long the motion should take, in seconds.</param>
		/// <param name="points">The points that the motion should spline between.</param>
		/// <returns>A reference to this, for specifying additional parameters.</returns>
		public Spliner Spline<TTarget, TPoint>(TTarget target, float duration, params TPoint[] points) where TTarget : class
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			
			var targetType = target.GetType();
			if (targetType.IsValueType)
				throw new ArgumentException("Target may not be a struct");
			
			Target = target;
			Duration = duration;
			
			var path = new List<float[]>();		//	float[] here is one object's properties
			var cross = new List<float[]>();    //	float[] here is one property's value at each point

            GetDefinitions(target, points[0], out accessor, out var pointDef);
            buffer = new float[accessor.Length];
			
			for (int i = 0; i < points.Length; i++)
			{
				path.Add(new float[pointDef.Length]);
				for (int j = 0; j < pointDef.Length; j++)
					path[i][j] = pointDef[j].GetValue(points[i]);
			}
			
			for (int i = 0; i < pointDef.Length; i++)
			{
				var splinePoints = new float[path.Count];
				cross.Add(splinePoints);
				for (int j = 0; j < path.Count; j++)
					splinePoints[j] = path[j][i];
			}
			
			Splines.Clear();
			for (int i = 0; i < cross.Count; i++)
				Splines.Add(new Spline(cross[i]));
			
			return this;
		}
		
		public override void Update()
		{
			base.Update();
			Update(App.GameTime.DeltaSeconds);
		}
		
		private void Update(float elapsed)
		{	
			if (Paused || canceled)
				return;
			
			if (Delay > 0)
			{
				Delay -= elapsed;
				if (Delay > 0)
					return;
			}
			
			if (time == 0 && timesRepeated == 0 && begin != null)
				begin();
			
			time += elapsed;
			float setTimeTo = time;
			float t = time / Duration;
			bool doComplete = false;
			
			if (time >= Duration)
			{
				if (repeatCount != 0)
				{
					setTimeTo = 0;
					Delay = repeatDelay;
					timesRepeated++;
					
					if (repeatCount > 0)
						--repeatCount;
					
					if (repeatCount < 0)
						doComplete = true;
				}
				else
				{
					time = Duration;
					t = 1;
                    doComplete = true;
				}
			}
			
			if (ease != null)
				t = ease(t);
			
			PathPoint(t, Splines, buffer);
			for (int i = 0; i < buffer.Length; i++)
				accessor[i].SetValue(Target, buffer[i]);
			
			time = setTimeTo;
			
			//	If the timer is zero here, we just restarted.
			//	If reflect mode is on, flip start to end
			if (time == 0 && reflect)
				Reverse();

            update?.Invoke();

            if (doComplete && complete != null)
				complete();
		}

		/// <summary>
		/// Set the easing function.
		/// </summary>
		/// <param name="ease">The Easer to use.</param>
		/// <returns>A reference to this.</returns>
		public Spliner Ease(Func<float, float> ease)
		{
			this.ease = ease;
			return this;
		}
		
		/// <summary>
		/// Set a function to call when motion begins (useful when using delays). Can be called multiple times for compound callbacks.
		/// </summary>
		/// <param name="callback">The function that will be called when motion starts, after the delay.</param>
		/// <returns>A reference to this.</returns>
		public Spliner OnBegin(Action callback)
		{
			if (begin == null) begin = callback;
			else begin += callback;
			return this;
		}
		
		/// <summary>
		/// Set a function to call when motion finishes. Can be called multiple times for compound callbacks.
		/// If the spliner repeats infinitely, this will be called each time; otherwise it will only run when the spliner is finished repeating.
		/// </summary>
		/// <param name="callback">The function that will be called on motion completion.</param>
		/// <returns>A reference to this.</returns>
		public Spliner OnComplete(Action callback)
		{
			if (complete == null) complete = callback;
			else complete += callback;
			return this;
		}
		
		/// <summary>
		/// Set a function to call as the spliner updates. Can be called multiple times for compound callbacks.
		/// </summary>
		/// <param name="callback">The function to use.</param>
		/// <returns>A reference to this.</returns>
		public Spliner OnUpdate(Action callback)
		{
			if (update == null) update = callback;
			else update += callback;
			return this;
		}
		
		/// <summary>
		/// Enable repeating.
		/// </summary>
		/// <param name="times">Number of times to repeat. Leave blank or pass a negative number to repeat infinitely.</param>
		/// <returns>A reference to this.</returns>
		public Spliner Repeat(int times = -1)
		{
			repeatCount = times;
			return this;
		}
		
		/// <summary>
		/// Set a delay for when motion repeats.
		/// </summary>
		/// <param name="delay">How long to wait before repeating.</param>
		/// <returns>A reference to this.</returns>
		public Spliner RepeatDelay(float delay)
		{
			repeatDelay = delay;
			return this;
		}
		
		/// <summary>
		/// Sets motion to reverse every other time it repeats. Repeating must be enabled for this to have any effect.
		/// </summary>
		/// <returns>A reference to this.</returns>
		public Spliner Reflect()
		{
			reflect = true;
			return this;
		}
		
		/// <summary>
		/// Reverses the spline for each property being animated.
		/// </summary>
		/// <returns>A reference to this.</returns>
		public Spliner Reverse()
		{
			foreach (var spline in Splines)
				spline.Reverse();
			
			return this;
		}
		
		/// <summary>
		/// Stop motion without calling complete callbacks.
		/// </summary>
		public void Cancel()
		{
			canceled = true;
		}
		
		/// <summary>
		/// Assign target its final values and stop.
		/// </summary>
		public void CancelAndComplete()
		{
			time = Duration;
			update = null;
			Update(0);
			canceled = true;
		}
		
		/// <summary>
		/// Pause spliner. Motion will not update and delay won't tick down.
		/// </summary>
		public void Pause()
		{
    		Paused = true;
		}
		
		/// <summary>
		/// Toggle whether the motion is paused.
		/// </summary>
		public void PauseToggle()
		{
			Paused = !Paused;
		}
		
		/// <summary>
		/// Resumes motion from a paused state.
		/// </summary>
		public void Resume()
		{
			Paused = false;
		}
		
        private static void PathPoint(float t, List<Spline> splines, float[] result)
        {
			for (int i = 0; i < result.Length; i++)
				result[i] = splines[i].Point(t);
        }
        
		private static void GetDefinitions(object target, object point, out FastAccessor<float>[] targetDef, out FastAccessor<float>[] pointDef)
		{
			var pointType = point.GetType();
			var targetType = target.GetType();
			
			var pointResult = new List<FastAccessor<float>>();
			var targetResult = new List<FastAccessor<float>>();
			var members = pointType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
			
			foreach (var member in members)
			{
				if (member.MemberType == MemberTypes.Property || member.MemberType == MemberTypes.Field)
				{
					MemberInfo targetMember = null;
					var targetMembers = targetType.GetMember(member.Name, BindingFlags.Public | BindingFlags.Instance);
					for (int i = 0; i < targetMembers.Length; i++)
					{
						var type = targetMembers[i].MemberType;
						if (type == MemberTypes.Field || type == MemberTypes.Property)
						{
							targetMember = targetMembers[i];
							break;
						}
					}
					
					if (targetMember != null)
					{
						pointResult.Add(new FastAccessor<float>(member));
						targetResult.Add(new FastAccessor<float>(targetMember));
					}
				}
			}
			
			targetDef = targetResult.ToArray();
			pointDef = pointResult.ToArray();
		}
		
		private FastAccessor<float>[] accessor;
		private List<Spline> Splines;
		private float[] buffer;
		
#region Callbacks
		private Func<float, float> ease;
        private Action begin, update, complete;
#endregion

#region Timing
		public bool Paused { get; private set; }
		private bool canceled;
        private float Delay, repeatDelay;
        private float Duration;

        private float time;
#endregion

		private bool reflect;
        private int repeatCount, timesRepeated;
	}
}
