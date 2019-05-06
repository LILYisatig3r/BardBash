using Assets._Scripts;

public partial class S_BattleRhythm
{
    private class BattleSM
    {
        public S_BattleRhythm battleRhythm;

        private BattleState[] states;
        private E_BattleState eCurrentState;
        private BattleState currentState;
        private E_BattleState ePreviousState;
        private BattleState previousState;

        public BattleSM(S_BattleRhythm battleRhythm)
        {
            this.battleRhythm = battleRhythm;

            states = new BattleState[10];
            states[0] = new BS_CameraControl(this);
            states[1] = new BS_End(this);
            states[2] = new BS_Intro(this);
            states[3] = new BS_Overview(this);
            states[4] = new BS_PlayerControl(this);
            states[5] = new BS_Preview(this);
            states[6] = new BS_Transitioning(this);
            states[7] = new BS_PlayerMenu(this);
            states[8] = new BS_EnemyControl(this);
            states[9] = new BS_TurnEnded(this);
            currentState = null;
            previousState = null;
        }

        /// <summary>
        /// For when you want to from one greater state to another
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(E_BattleState newState)
        {
            if (newState == E_BattleState.nil)
                return;

            if (currentState != null)
            {
                currentState.Exit();
                previousState = currentState;
                ePreviousState = eCurrentState;
            }

            currentState = states[(int) newState];
            eCurrentState = newState;
            currentState.Enter(battleRhythm);
        }

        /// <summary>
        /// Execute the current state's update function
        /// </summary>
        public void ExecuteStateUpdate()
        {
            if (currentState == null)
                return;
            currentState.Execute();
        }

        /// <summary>
        /// Execute the current state's beat update function
        /// </summary>
        public void ExecuteBeatUpdate()
        {
            if (currentState == null)
                return;
            currentState.BeatExecute();
        }

        /// <summary>
        /// Returns the current battle state
        /// </summary>
        /// <returns></returns>
        public BattleState GetCurrentState()
        {
            return currentState;
        }

        /// <summary>
        /// Returns the enum of the current battle state
        /// </summary>
        /// <returns></returns>
        public E_BattleState GetCurrentEState()
        {
            return eCurrentState;
        }

        /// <summary>
        /// If you need to recall the last state
        /// </summary>
        /// <returns></returns>
        public E_BattleState GetPreviousState()
        {
            return ePreviousState;
        }

        public void FinishedCameraTransition()
        {
            if (battleRhythm.activeMusician is S_PlayerController)
                ChangeState(E_BattleState.playerMenu);
            else if (battleRhythm.activeMusician is S_EnemyController)
                ChangeState(E_BattleState.enemyControl);
        }

        public void StartPlayerMenu()
        {
            ChangeState(E_BattleState.playerMenu);
        }

        public void StartMusicianTurn()
        {
            ChangeState(E_BattleState.playerControl);
        }

        public void EndMusicianTurn()
        {
            ChangeState(E_BattleState.turnEnded);
        }

        public void TransitionMusician()
        {
            battleRhythm.TransitionMusician();
        }

        public void CameraControl()
        {
            ChangeState(E_BattleState.cameraControl);
        }
    }
}
