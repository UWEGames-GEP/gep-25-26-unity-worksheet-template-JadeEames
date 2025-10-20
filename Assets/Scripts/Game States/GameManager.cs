using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game_States
{
    public struct TransitionDetails
    {
        public TransitionDetails(bool _transition, STATE _next_state, bool _clear)
        {
            Transition = _transition;
            NextState = _next_state;
            ClearPreviousState = _clear;
        }

        public bool Transition { get; set; }
        public STATE NextState {  get; set; }
        public bool ClearPreviousState { get; }
    }

    public class GameManager : MonoBehaviour
    {
        // Dictionary holding the game states mapped to their enum identity.
        private Dictionary<STATE, GameState> game_states;

        // Enum variable storing the current state of the game. 
        private STATE current_state;
        private STATE previous_state;
        private TransitionDetails transition_info;


        void Start()
        {
            // Initialise game states & set current state.
            game_states = new Dictionary<STATE, GameState>
            {
                { STATE.GAME_PLAY, new PlayState() },
                { STATE.PAUSE, new PauseState() }
            };

            foreach (KeyValuePair<STATE, GameState> state in game_states)
            {
                state.Value.Initialise();
            }

            current_state = STATE.GAME_PLAY;
            previous_state = STATE.GAME_PLAY;
            transition_info = new TransitionDetails(false, current_state, false);
            game_states[current_state].Initialise();
        }

        
        void Update()
        {
            game_states[current_state].Tick();
        }

        private void LateUpdate()
        {
            ExecuteStateChange();
        }

        private void ExecuteStateChange()
        {
            if (transition_info.Transition)
            {
                // Exit current state & clear if specified.
                game_states[current_state].OnExit();
                if (transition_info.ClearPreviousState)
                {
                    game_states[current_state] = game_states[current_state].ConstructorFunc();
                }

                // Enter new state. 
                previous_state = current_state;
                current_state = transition_info.NextState;
                game_states[current_state].OnEnter();
                transition_info.Transition = false;
            }
        }

        public void SetState(GameStateEvent transitionEvent)
        {
            if (transitionEvent.transitioning) {transition_info = new TransitionDetails(true, transitionEvent.nextState, transitionEvent.clearPreviousState);}
        }
        public void SetState(TransitionDetails transitionDetails)
        {
            if (transitionDetails.Transition) { transition_info = transitionDetails; }
        }

        public void togglePause(PauseInputEvent pauseInputEvent)
        {
            switch (current_state)
            {
                case STATE.GAME_PLAY:
                    SetState(new TransitionDetails(true, STATE.PAUSE, false));
                    break;
                case STATE.PAUSE:
                    transition_info = new TransitionDetails(true, previous_state, false);
                    ExecuteStateChange();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
