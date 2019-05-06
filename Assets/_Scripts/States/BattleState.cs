using System;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public partial class S_BattleRhythm
{
    private class BattleState
    {
        protected S_BattleRhythm br;
        protected BattleSM sm;

        protected Camera cam;
        protected Vector3 target;
        protected Vector3 velocity = Vector3.zero;
        protected float dampTime = 0.15f;

        public BattleState(BattleSM machine)
        {
            sm = machine;
            br = machine.battleRhythm;
            cam = br.cam;
        }
        public virtual void Enter(S_BattleRhythm br) {}
        public virtual void Execute() { }
        public virtual void OnBeatEvent() { }
        public virtual void BeatExecute() { }
        public virtual void ReceiveInput(Player p) { }
        public virtual void Exit() { }

        protected Vector3 UpdateCamPositionTowardsTarget()
        {
            Vector3 point = cam.WorldToViewportPoint(target);
            Vector3 delta = target - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = cam.transform.position + delta;
            Vector3 newPos = Vector3.SmoothDamp(cam.transform.position, destination, ref velocity, dampTime);
            Vector3 diff = newPos - cam.transform.position;
            cam.transform.position = newPos;
            return diff;
        }
    }

    private class BS_CameraControl : BattleState
    {
        private bool overview = false;

        private List<S_Actor> actors;

        private Vector3 up = new Vector3(0f, 0f, 0.075f);
        private Vector3 right = new Vector3(0.075f, 0f, 0f);
        private Vector3 down = new Vector3(0f, 0f, -0.075f);
        private Vector3 left = new Vector3(-0.075f, 0f, 0f);

        private const int MoveLeft = 0;
        private const int MoveRight = 1;
        private const int MoveUp = 2;
        private const int MoveDown = 3;
        private const int Ability0 = 4;
        private const int Ability1 = 5;
        private const int Ability2 = 6;
        private const int Ability3 = 7;
        private const int UISubmit = 10;
        private const int UICancel = 11;
        private const int Shift = 12;
        private const int SpaceBar = 13;

        public BS_CameraControl(BattleSM machine) : base(machine)
        {
            cam = br.cam;
            actors = br.o;
        }

        public override void Enter(S_BattleRhythm br)
        {
            target = br.activeMusician.position;
        }

        public override void Execute()
        {
            UpdateCamPositionTowardsTarget();
        }

        public override void Exit()
        {
            target = br.activeMusician.position;
        }

        public override void ReceiveInput(Player p)
        {
            if (p.GetButtonDown(UISubmit))
                Overview();

            if (!overview)
            {
                if (p.GetButtonDown(Ability2))
                    cam.orthographicSize += 1.5f;
                else if (p.GetButtonDown(Ability3))
                    cam.orthographicSize -= 1.5f;

                float m = 1f;
                if (p.GetButton(Ability0))
                    m = 2f;

                if (p.GetButton(MoveUp))
                    target += (m * up);
                else if (p.GetButton(MoveDown))
                    target += (m * down);

                if (p.GetButton(MoveRight))
                    target += (m * right);
                else if (p.GetButton(MoveLeft))
                    target += (m * left);

                if (p.GetButton(UICancel))
                    sm.ChangeState(Assets._Scripts.E_BattleState.playerMenu);
            }
        }

        private void Overview()
        {
            if (!overview)
            {
                overview = true;
                float minx = Mathf.Infinity, maxx = 0, minz = Mathf.Infinity, maxz = 0;
                foreach (S_Actor actor in actors)
                {
                    Vector3 ap = actor.transform.position;
                    minx = Mathf.Min(minx, ap.x);
                    maxx = Mathf.Max(maxx, ap.x);
                    minz = Mathf.Min(minz, ap.z);
                    maxz = Mathf.Max(maxz, ap.z);
                }
                cam.orthographicSize = Mathf.Max(3f, Mathf.Max((maxx - minx) * 1.41f, maxz - minz) / 4f);
                target = new Vector3((minx + maxx) / 2, 0f, (minz + maxz) / 2);
            }
            else
            {
                overview = false;
                cam.orthographicSize = 3.5f;
                target = br.activeMusician.position;
            }
        }
    }

    private class BS_PlayerControl : BattleState
    {
        KeyCode bufferedMovement;

        AudioClip clockTick;
        AudioClip bellDing;

        public BS_PlayerControl(BattleSM machine) : base(machine)
        {
            clockTick = S_SoundEffectsSO.Instance.GetClockTickSoundEffect();
            bellDing = S_SoundEffectsSO.Instance.GetBellDingSoundEffect();
        }

        public override void Enter(S_BattleRhythm br)
        {
            br.remainingMeasures = 4f;
            cam.orthographicSize = 3.5f;
            target = br.activeMusician.transform.position;
            bufferedMovement = KeyCode.None;
        }

        public override void Execute()
        {
            target = br.activeMusician.transform.position;
            UpdateCamPositionTowardsTarget();
        }

        public override void BeatExecute()
        {
            br.activeMusician.BeatUpdate();

            if (br.remainingMeasures > 0)
            {
                br.DecrementMeasures(0.25f);
                if (br.remainingMeasures == Mathf.Round(br.remainingMeasures) && br.remainingMeasures > 0f)
                    br.activeMusician.SmallText(br.remainingMeasures * 4 + "", Color.white);
                if (br.remainingMeasures <= 1f && br.remainingMeasures > 0f)
                {
                    br.activeMusician.SmallText(br.remainingMeasures * 4 + "", Color.white);
                    br.PlaySound(clockTick);
                }
            }
            if (br.remainingMeasures <= 0)
            {
                br.activeMusician.SmallText(0 + "", Color.white);
                br.PlaySound(bellDing);
                sm.EndMusicianTurn();
            }
        }

        public override void ReceiveInput(Player p)
        {
            if (br.TryPlayerAction())
                BeatExecute();
        }

        public override void Exit()
        {
            
        }
    }

    private class BS_EnemyControl : BattleState
    {
        AudioClip clockTick;
        AudioClip bellDing;

        private short beatsPassed;

        public BS_EnemyControl(BattleSM machine) : base(machine)
        {
            clockTick = S_SoundEffectsSO.Instance.GetClockTickSoundEffect();
            bellDing = S_SoundEffectsSO.Instance.GetBellDingSoundEffect();
        }

        public override void Enter(S_BattleRhythm br)
        {
            br.remainingMeasures = 2f;
            cam.orthographicSize = 3.5f;
            target = br.activeMusician.transform.position;
            br.Banner();
            beatsPassed = 0;
        }

        public override void Execute()
        {
            target = br.activeMusician.transform.position;
            UpdateCamPositionTowardsTarget();
        }

        public override void OnBeatEvent()
        {
            br.activeMusician.BeatUpdate();

            if (++beatsPassed == 2)
                br.Banner();

            if (br.remainingMeasures > 0)
            {
                br.DecrementMeasures(0.25f);
                if (br.remainingMeasures == Mathf.Round(br.remainingMeasures) && br.remainingMeasures > 0f)
                    br.activeMusician.SmallText(br.remainingMeasures * 4 + "", Color.white);
                if (br.remainingMeasures < 1f && br.remainingMeasures > 0f)
                {
                    br.activeMusician.SmallText(br.remainingMeasures * 4 + "", Color.white);
                    br.PlaySound(clockTick);
                }
            }
            if (br.remainingMeasures <= 0)
            {
                br.activeMusician.SmallText(0 + "", Color.white);
                br.PlaySound(bellDing);
                sm.EndMusicianTurn();
            }
        }

        public override void Exit()
        {

        }
    }

    private class BS_End : BattleState
    {
        public BS_End(BattleSM machine) : base(machine)
        {
        }

        public override void Enter(S_BattleRhythm br)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }
    }

    private class BS_Intro : BattleState
    {

        #region Members, Constructor, and Sub-states
        private List<S_Actor> targets;
        private int iterator;
        private IS[] introStates;
        private IS introState;

        private float waitTime;
        private float timeWaited;
        private float camSize;

        public BS_Intro(BattleSM machine): base(machine)
        {
            waitTime = 1.5f;
        }

        private enum IS
        {
            PanActor = 0,
            PanOverview = 1,
            Wait = 2
        }
        #endregion

        #region State Operations
        override public void Enter(S_BattleRhythm br)
        {
            targets = br.o;
            iterator = 0;
            target = targets[iterator].transform.position;
            cam = br.cam;
            cam.orthographicSize = 3.5f;
            introStates = new IS[] { IS.PanActor, IS.PanOverview, IS.Wait };
            SwitchISState(IS.PanActor);
        }

        override public void Execute()
        {
            switch (introState)
            {
                case IS.PanActor:
                    target = targets[iterator].transform.position;
                    UpdateCamPositionTowardsTarget();
                    timeWaited += Time.deltaTime;
                    if (timeWaited > waitTime)
                    {
                        timeWaited = 0f;
                        iterator++;
                        if (iterator < targets.Count)
                            target = targets[iterator].transform.position;
                        else
                            SwitchISState(IS.PanOverview);
                    }
                    break;
                case IS.PanOverview:
                    UpdateCamPositionTowardsTarget();
                    timeWaited += Time.deltaTime;
                    if (Input.anyKeyDown && timeWaited > waitTime)
                        sm.StartPlayerMenu();
                    break;
                case IS.Wait:
                    break;
            }
        }

        public override void ReceiveInput(Player p)
        {
            //if (br.TryPlayerAction())
            //    BeatExecute();

            switch (introState)
            {
                case IS.PanActor:
                    SwitchISState(IS.PanOverview);
                    break;
                case IS.PanOverview:
                    UpdateCamPositionTowardsTarget();
                    timeWaited += Time.deltaTime;
                    if (Input.anyKeyDown && timeWaited > waitTime)
                        sm.StartPlayerMenu();
                    break;
                case IS.Wait:
                    break;
            }
        }

        override public void Exit()
        {
            // Exiting requires no action. Do nothing.
        }

        private void SwitchISState(IS iState)
        {
            introState = iState;
            switch (iState)
            {
                case IS.PanActor:
                    break;
                case IS.PanOverview:
                    float minx = Mathf.Infinity, maxx = 0, minz = Mathf.Infinity, maxz = 0;
                    foreach (S_Actor actor in targets)
                    {
                        Vector3 ap = actor.transform.position;
                        minx = Mathf.Min(minx, ap.x);
                        maxx = Mathf.Max(maxx, ap.x);
                        minz = Mathf.Min(minz, ap.z);
                        maxz = Mathf.Max(maxz, ap.z);
                    }
                    cam.orthographicSize = Mathf.Max(3f, Mathf.Max((maxx - minx) * 1.41f, maxz - minz) / 4f);
                    target = new Vector3((minx + maxx) / 2, 0f, (minz + maxz) / 2);
                    break;
                case IS.Wait:
                    // We're just going to keep on keepin' on. Do nothing.
                    break;
            }
            timeWaited = 0f;
        }
        #endregion

    }

    private class BS_Overview : BattleState
    {
        public BS_Overview(BattleSM machine) : base(machine)
        {
        }

        public override void Enter(S_BattleRhythm br)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }

    }

    private class BS_Preview : BattleState
    {

        #region Members, Constructors, and Sub-states
        private Vector3 target;
        private LinkedListNode<S_Actor> iterator;

        private float waitTime;
        private float timeWaited;
        private Camera cam;
        private float camSize;
        private Vector3 velocity = Vector3.zero;
        private float dampTime = 0.15f;

        public BS_Preview(BattleSM machine):base(machine)
        {
            waitTime = 20f;
        }
        #endregion

        #region State Operations
        public override void Enter(S_BattleRhythm br)
        {
        }

        public override void Execute()
        {
            if (br.remainingMeasures >= 0)
            {
                br.DecrementMeasures(0.25f);
            }

            if (br.remainingMeasures <= 0)
            {
                br.StartMusicianTurn();
            }
        }

        public override void Exit()
        {
        }
        #endregion
    }

    private class BS_Transitioning : BattleState
    {
        Vector3 diff;

        public BS_Transitioning(BattleSM machine) : base(machine)
        {
        }

        public override void Enter(S_BattleRhythm br)
        {
            cam.orthographicSize = 3.5f;
            target = br.activeMusician.transform.position;
        }

        public override void Execute()
        {
            diff = UpdateCamPositionTowardsTarget();
            if (diff.sqrMagnitude <= 0.35355f)
                sm.FinishedCameraTransition();
        }

        public override void Exit()
        {
        }
    }

    private class BS_PlayerMenu : BattleState
    {
        private const int Ability0 = 4;
        private const int Ability1 = 5;
        private const int Ability2 = 6;
        private const int Ability3 = 7;

        private UnityEngine.Events.UnityAction startTurn;
        private UnityEngine.Events.UnityAction camControl;

        public BS_PlayerMenu(BattleSM machine) : base(machine)
        {
            startTurn = sm.StartMusicianTurn;
            camControl = sm.CameraControl;
        }

        public override void Enter(S_BattleRhythm br)
        {
            cam.orthographicSize = 3.5f;
            target = br.activeMusician.transform.position;
            br.Banner();
            br.SetBattleMenuActive(true);
            br.startButton.onClick.AddListener(startTurn);
            br.viewMapButton.onClick.AddListener(camControl);
        }

        public override void Execute()
        {
            UpdateCamPositionTowardsTarget();
        }

        public override void Exit()
        {
            br.Banner();
            br.SetBattleMenuActive(false);
            br.startButton.onClick.RemoveListener(startTurn);
            br.viewMapButton.onClick.RemoveListener(camControl);
        }

        public override void ReceiveInput(Player p)
        {
            if (p.GetButtonDown(Ability0))
                sm.StartMusicianTurn();
            else if (p.GetButtonDown(Ability1))
                sm.CameraControl();
        }
    }

    private class BS_TurnEnded : BattleState
    {
        private float timeRemaining;

        public BS_TurnEnded(BattleSM machine) : base(machine)
        {

        }

        public override void Enter(S_BattleRhythm br)
        {
            cam.orthographicSize = 3.5f;
            target = br.activeMusician.transform.position;
            timeRemaining = 1.25f;
        }

        public override void Execute()
        {
            UpdateCamPositionTowardsTarget();
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0 && br.activeMusician is S_EnemyController)
                sm.TransitionMusician();
        }

        public override void ReceiveInput(Player p)
        {
            if (timeRemaining <= 0)
                sm.TransitionMusician();
        }

        public override void Exit()
        {
        }
    }
}



