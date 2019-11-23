
using Indigo.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Indigo.Components
{
	public class StateMachine : Component
	{
		private List<State> states;
		
		private int FrameCount;
		private List<int> FrameCountStack;
		
		private float Timer;
		private List<float> TimerStack;
		
		private Stack<int> StateStack;
		public int CurrentState { get; private set; }
		
		public const int InvalidState = -1;
		
		/// <summary>
		/// How many frames the current state has been active for.
		/// </summary>
		public int StateFrames
		{
			get {
				if (FrameCountStack.Count > 0) return FrameCountStack[FrameCountStack.Count - 1];
				return FrameCount;
			}
			
			set {
				if (FrameCountStack.Count > 0)FrameCountStack[FrameCountStack.Count - 1] = value;
				FrameCount = value;
			}
		}
		
		/// <summary>
		/// How long (in seconds) the current state has been active for.
		/// </summary>
		public float StateTime
		{
			get {
				if (TimerStack.Count > 0) return TimerStack[TimerStack.Count - 1];
				return Timer;
			}
			
			set {
				if (TimerStack.Count > 0) TimerStack[TimerStack.Count - 1] = value;
				Timer = value;
			}
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public StateMachine()
		{
			states = new List<State>();
			CurrentState = InvalidState;
			
			StateStack = new Stack<int>();
			FrameCountStack = new List<int>();
			TimerStack = new List<float>();
		}
		
		public override void Update()
		{
			base.Update();
			
			int currState = InvalidState;
			
			//	if the stack isn't empty, use the top state
			if (StateStack.Count > 0)
			{
				currState = StateStack.Peek();
			}
			else
			{
				currState = CurrentState;
			}
			
			StateFrames++;
            StateTime += App.GameTime.DeltaSeconds;
			
			if (currState == InvalidState)
				return;
			
			if (states.Count == 0)
				return;
			
			if (StateInvalid(currState))
				throw new Exception("Invalid state set in StateMachine.");

            states[currState].Update?.Invoke();
        }
		
		/// <summary>
		/// Add a new state to the state machine.
		/// </summary>
		/// <param name="begin">The function to call when the state begins. Can be null.</param>
		/// <param name="update">The function to call while the state is active. Can be null.</param>
		/// <param name="end">The function to call when the state ends. Can be null.</param>
		/// <returns>The state ID.</returns>
		public int AddState(Action begin, Action update, Action end)
		{
			var state = new State();
			state.Begin = begin;
			state.Update = update;
			state.End = end;
			
			states.Add(state);
			return states.Count - 1;
		}
		
		/// <summary>
		/// Add a new state to the state machine.
		/// </summary>
		/// <param name="update">The function to call while the state updates. Can be null.</param>
		/// <returns>The state ID.</returns>
		public int AddState(Action update)
		{
			return AddState(null, update, null);
		}
		
		/// <summary>
		/// Push a state onto a stack, preserving the existing state or states.
		/// </summary>
		/// <param name="state">The state to push.</param>
		public void PushState(int state)
		{
			if (StateInvalid(state))
				throw new Exception("Invalid state pushed.");
			
			StateStack.Push(state);
			FrameCountStack.Add(0);
			TimerStack.Add(0);
			
			var begin = states[state].Begin;
			if (begin != null)
				begin();
			
			CurrentState = StateStack.Peek();
		}
		
		/// <summary>
		/// Removes the current state and transitions to the next one on the stack.
		/// </summary>
		/// <returns>The ID of the state that was removed, or -1 if the stack was empty.</returns>
		public int PopState()
		{
			if (StateStack.Count == 0)
				return InvalidState;
			
			var state = StateStack.Pop();
			var end = states[state].End;
			if (end != null)
				end();
			
			FrameCountStack.RemoveAt(FrameCountStack.Count - 1);
			TimerStack.RemoveAt(TimerStack.Count - 1);
			
			if (StateStack.Count > 0)
			{
				CurrentState = StateStack.Peek();
			}
			else
			{
				CurrentState = InvalidState;
			}
			
			return state;
		}
		
		/// <summary>
		/// <para>Switches to a different state.</para>
		/// <para>If the stack is not empty, this will cause an error.</para>
		/// </summary>
		/// <param name="state">The state to switch to.</param>
		public void ChangeState(int state)
		{
			if (StateStack.Count > 0)
				throw new Exception("Cannot change state while in stack mode.");
			
			if (CurrentState == state)
				return;
			
			var lastState = CurrentState;
			CurrentState = state;
			
			if (StateInvalid(state))
				throw new Exception("Invalid state set.");
			
			if (!StateInvalid(lastState))
			{
				var end = states[lastState].End;
				if (end != null)
					end();
			}
			
			var begin = states[state].Begin;
			if (begin != null)
				begin();
			
			StateTime = 0;
			StateFrames = 0;
		}
		
		/// <summary>
		/// Reset the current state's timers and call its begin function, if it exists.
		/// </summary>
		public void ResetState()
		{
			StateTime = 0;
			StateFrames = 0;
			
			var begin = states[CurrentState].Begin;
			if (begin != null)
				begin();
		}
		
		private struct State
		{
			public Action Begin, Update, End;
		}
		
		private bool StateInvalid(int state)
		{
			return state < 0 || state >= states.Count;
		}
	}
}
