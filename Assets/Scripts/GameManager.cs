using UnityEngine;
using System.Collections.Generic;

namespace GameFSM
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
            transition_info = new TransitionDetails(false, current_state, false);
            game_states[current_state].Initialise();
        }

        
        void Update()
        {
            // /REMOVE 
            if (Input.GetKeyDown(KeyCode.P)) SetState( STATE.PAUSE);
            if (Input.GetKeyDown(KeyCode.Escape)) SetState(STATE.GAME_PLAY);

            game_states[current_state].Tick();
        }

        private void LateUpdate()
        {
            if (transition_info.Transition)
            {
                // Exit current state & clear if specified.
                game_states[current_state].OnExit();
                if (transition_info.ClearPreviousState) { game_states[current_state] = game_states[current_state].ConstructorFunc(); }

                // Enter new state. 
                current_state = transition_info.NextState;
                game_states[current_state].OnEnter();
            }
        }

        public void SetState(STATE _new_state, bool _clear = false)
        {
            transition_info = new TransitionDetails(true, _new_state, _clear);
        }
    }
}
