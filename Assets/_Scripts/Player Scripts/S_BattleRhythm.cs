using Assets._Scripts;
using Assets._Scripts.Player_Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class S_BattleRhythm : MonoBehaviour {

    private class MusicianClockComparer : IComparer<S_Actor>
    {
        public int Compare(S_Actor a, S_Actor b)
        {
            if (Mathf.Approximately(a.clock, b.clock))
            {
                if (a is S_PlayerController && b is S_EnemyController)
                    return -1;
                else if (b is S_PlayerController && a is S_EnemyController)
                    return 1;
                else if (Mathf.Approximately(a.speed, b.speed))
                    return -1;
                else if (a.speed < b.speed)
                    return -1;
                else if (b.speed > a.speed)
                    return 1;
                else
                    return -1;
            }
            else if (a.clock > b.clock)
                return -1;
            else if (b.clock > a.clock)
                return 1;
            else 
                return -1;
        }
    }

    #region Members
    // Game Manager
    GameManager gm;

    // Map
    [SerializeField] public GraphicsTilesMap map;

    // Koreographer
    //[SerializeField] string trackName;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip track;
    [SerializeField] private S_Composition composition;
    [SerializeField] private float beatOffset;
    [SerializeField] private float beatForgiveness;
    [SerializeField] public S_SoundEffectsSO sounds;
    private S_BeatScroller beatScroller;

    // UI
    [SerializeField] Transform banner;
    private Image bannerPortrait;
    private Text bannerName;
    private bool bannerExtended;
    [SerializeField] Transform turnQueueUI;
    Image[] turnQueueImages;
    [SerializeField] EventSystem eventSystem;
    [SerializeField] Transform battleMenu;
    public Button startButton { get; private set; }
    public Button viewMapButton { get; private set; }

    // Battle and Camera state machines
    private BattleSM battleStateMachine;
    private CameraController cameraController;
    public Camera cam;
    public float shakeDuration;
    public float shakeMagnitude;

    // Orchestra
    public List<S_Actor> players { get; private set; }
    public LinkedList<S_Actor> orchestra { get; private set; }
    private float counterThreshold;
    public List<S_Actor> o { get; private set; }
    public LinkedList<S_Actor> o2 { get; private set; }
    public S_Actor lastMusician { get; private set; }
    public S_Actor activeMusician { get; private set; }
    public float remainingMeasures { get; private set; }
    public float previewMeasures { get; private set; }
    #endregion

    #region Monobehaviour

    void Start () {
        // Initialize members and references
        GameManager.TryGetInstance(out gm);
        players = new List<S_Actor>();
        orchestra = new LinkedList<S_Actor>();
        o = new List<S_Actor>();
        o2 = new LinkedList<S_Actor>();
        cam = Camera.main;
        previewMeasures = 1f;
        counterThreshold = 24f;
        map.BuildMesh();
        //composition.Initialize(track, 120f, 0.05f);
        composition.Initialize(track, 120f, beatOffset, beatForgiveness);
        composition.BeatHit += OnBeatHit;
        composition.BeatLost += OnBeatLost;

        // Initialize UI
        banner.transform.localPosition = new Vector3(-130f, 0f, 0f);
        bannerExtended = false;
        bannerPortrait = banner.transform.GetChild(0).GetComponent<Image>();
        bannerName = banner.transform.GetChild(1).GetComponent<Text>();
        StartCoroutine("StartSequence");
        turnQueueImages = new Image[6];
        turnQueueImages[0] = turnQueueUI.GetChild(0).GetComponent<Image>();
        turnQueueImages[1] = turnQueueUI.GetChild(1).GetComponent<Image>();
        turnQueueImages[2] = turnQueueUI.GetChild(2).GetComponent<Image>();
        turnQueueImages[3] = turnQueueUI.GetChild(3).GetComponent<Image>();
        turnQueueImages[4] = turnQueueUI.GetChild(4).GetComponent<Image>();
        turnQueueImages[5] = turnQueueUI.GetChild(5).GetComponent<Image>();
        eventSystem = EventSystem.current;
        startButton = battleMenu.GetChild(0).GetComponent<Button>();
        viewMapButton = battleMenu.GetChild(1).GetComponent<Button>();
        eventSystem.SetSelectedGameObject(null);

        // Add all musicians
        AddAllMusicians();

        // Start state machines
        battleStateMachine = new BattleSM(this);
        battleStateMachine.ChangeState(E_BattleState.intro);

        
    }

    void Update()
    {
        composition.Updater(Time.deltaTime);
        battleStateMachine.ExecuteStateUpdate();
        //cameraController.Update();
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShakeCamera(shakeDuration, 1f);
        }
    }

    public void BeatUpdate()
    {
        //battleStateMachine.ExecuteBeatUpdate();
        //battleStateMachine.ExecuteStateUpdate();
    }

    public void ReceiveInput(Rewired.Player p)
    {
        battleStateMachine.GetCurrentState().ReceiveInput(p);
    }

    public bool CheckPlayerHitBeat()
    {
        return composition.CheckForBeatHit(beatForgiveness);
    }

    public bool TryPlayerAction()
    {
        if (composition.CheckForBeatHit(beatForgiveness) && (activeMusician as S_PlayerController).TryAction())
        {
            composition.HitBeat(beatForgiveness);
            return true;
        }
        return false;
    }

    public void OnBeatHit(object source, EventArgs e)
    {
        battleStateMachine.GetCurrentState().OnBeatEvent();
    }

    public void OnBeatLost(object source, EventArgs e)
    {
        battleStateMachine.GetCurrentState().BeatExecute();
    }

    public void PlaySound(string s)
    {

    }

    #endregion

    #region Orchestral Operations

    private void AddAllMusicians()
    {
        foreach (S_Actor player in gm.playerCharacters)
        {
            players.Add(player);
            AddMusician(player);
        }

        foreach (S_Actor npc in gm.npcCharacters)
        {
            AddMusician(npc);
        }

        lastMusician = null;
        ClockTick();
        activeMusician = o2.First.Value;
        DrawQueue();
    }

    public void AddMusician(S_Actor musician)
    {
        musician.CombatStatCalculator();
        musician.clock = musician.speed;
        o.Add(musician);
    }

    public void TransitionMusician()
    {
        lastMusician = activeMusician;
        o2.RemoveFirst();
        ClockTick();
        activeMusician = o2.First.Value;
        activeMusician.PrepForNewTurn();

        battleStateMachine.ChangeState(E_BattleState.transitioning);
        //StartCoroutine("Banner");

        DrawQueue();
    }

    private void ClockTick()
    {
        SortedList<S_Actor, int> addQueue = new SortedList<S_Actor, int>(new MusicianClockComparer());
        while (o2.Count < 6)
        {
            while (addQueue.Count == 0)
            {
                foreach (S_Actor musician in o)
                {
                    musician.clock += musician.speed;
                    if (musician.clock >= counterThreshold)
                        addQueue.Add(musician, 0);
                }
            }

            foreach (KeyValuePair<S_Actor, int> kvp in addQueue)
            {
                o2.AddLast(kvp.Key);
                kvp.Key.clock = kvp.Key.speed;
            }

            addQueue.Clear();
        }
    }

    private void AddMusicianToQueue(S_Actor musician)
    {
        LinkedListNode<S_Actor> iterator = o2.First;
        if (iterator == null)
            o2.AddFirst(musician);
        else
        {
            while (iterator != null)
            {
                if ((musician.clock > iterator.Value.clock)
                    || (musician.clock == iterator.Value.clock && musician.speed < iterator.Value.speed))
                {
                    o2.AddBefore(iterator, musician);
                    break;
                }
                iterator = iterator.Next;
            }
            if (iterator == null)
                o2.AddLast(new LinkedListNode<S_Actor>(musician));

            activeMusician = o2.First.Value;
        }

        musician.clock = musician.speed;
    }

    public void StartMusicianTurn()
    {
        remainingMeasures = activeMusician.GetMeasures();
        battleStateMachine.ChangeState(E_BattleState.playerControl);
        //StartCoroutine("Banner");
    }

    public void RemoveMusician(S_Actor musician)
    {
        if (musician.Equals(activeMusician))
        {
            activeMusician = orchestra.First.Value;
            remainingMeasures = activeMusician.GetMeasures();
            if (bannerExtended)
                Banner();
        }

        if (o.Remove(musician))
        {
            LinkedListNode<S_Actor> m = o2.First;
            LinkedListNode<S_Actor> mt = m;
            while (m != null)
            {
                if (m.Value.Equals(musician))
                {
                    mt = m.Next;
                    o2.Remove(m);
                    m = mt;
                }
                else
                    m = m.Next;
            }
            ClockTick();
            DrawQueue();

            Destroy(musician.gameObject);
            if (GetMusicianCount() <= 1)
            {
                gm.FinishBattle();
            }
        }
    }

    public void MusicianDamaged(S_Actor defender, S_Actor attacker, float damage, bool magic)
    {
        float defense = magic ? defender.magicDefense : defender.defense;
        float piercing = magic ? attacker.magicPiercing : attacker.piercing;

        float damageDealt = Mathf.Max(damage - Mathf.Max(defense - piercing, -piercing * 0.2f), 0f);
        if (damageDealt > defense * 3)
        {
            if (magic)
                defender.magicDefense -= 1;
            else
                defender.defense -= 1;
        }

        defender.SetCurrentHp(defender.curHp - damageDealt);
        defender.DamageText("" + damageDealt);
        //audioSource.PlayOneShot(S_SoundEffectsSO.GetRandomBashSoundEffect());
        if (defender.GetCurrentHp() <= 0)
            RemoveMusician(defender);
    }

    public Vector3 MusicianCanMove(S_Actor actor, Vector3 destination)
    {
        DataTile destTile = map.GetTile((int)destination.x, (int)destination.z);
        if (destTile.type == DataTile.TileType.grass && !destTile.occupant)
            return destination;
        else
            return new Vector3(-1f, -1f, -1f);
    }

    public Vector3 MusicianMove(S_Actor actor, Vector3 destination)
    {
        DataTile destTile = map.GetTile((int)destination.x, (int)destination.z);
        if (destTile.type == DataTile.TileType.grass && !destTile.occupant)
        {
            map.GetTile((int)actor.position.x, (int)actor.position.z).occupant = null;
            destTile.occupant = actor;
            return destination;
        }
        else
            return new Vector3(-1f, -1f, -1f);
    }

    public Vector3 MusicianRandomSpawn(S_Actor actor)
    {
        return map.OccupyRandomWalkableTile(actor);
    }

    public void DecrementMeasures(float measures)
    {
        remainingMeasures -= measures;
    }

    private void DrawQueue()
    {
        LinkedListNode<S_Actor> l = o2.First;
        for (int i = 0; i < 6; i++)
        {
            turnQueueImages[i].sprite = l.Value.GetPortrait();
            l = l.Next;
            if (l == null)
                l = o2.First;
        }
    }

    #endregion

    #region State Machine Operations
    public void ChangeCameraState(E_CameraState state)
    {
        cameraController.ChangeState(state);
    }

    public void CameraFinishedMovingToNewPlayer()
    {
        remainingMeasures = previewMeasures;
        ChangeBattleState(E_BattleState.playerMenu);
    }

    public E_BattleState GetBattleState()
    {
        return battleStateMachine.GetCurrentEState();
    }

    public void ChangeBattleState(E_BattleState state)
    {
        battleStateMachine.ChangeState(state);
    }

    public void SetBattleMenuActive(bool a)
    {
        if (a)
        {
            eventSystem.SetSelectedGameObject(startButton.gameObject);
        }
        else
            eventSystem.SetSelectedGameObject(null);
    }
    #endregion

    private IEnumerator StartSequence()
    {
        while (orchestra.Count == 0 || !Input.anyKey)
            yield return new WaitForEndOfFrame();

        activeMusician = orchestra.First.Value;
        activeMusician.PopupSprite();
        //StartCoroutine("Banner");
    }

    public void Banner()
    {
        StartCoroutine(BannerRoutine());
    }

    private IEnumerator BannerRoutine()
    {
        if (!bannerExtended)
        {
            bannerExtended = !bannerExtended;
            bannerPortrait.sprite = activeMusician.GetPortrait();
            bannerName.color = activeMusician.GetPrimaryColor();
            bannerName.text = activeMusician.GetActorName();
            while (banner.transform.localPosition.x < 120f)
            {
                banner.transform.localPosition += new Vector3(15f, 0f, 0f);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            bannerExtended = !bannerExtended;
            while (banner.transform.localPosition.x > -130f)
            {
                banner.transform.localPosition += new Vector3(-10f, 0f, 0f);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        StartCoroutine(CameraShake(duration, magnitude));
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        //Vector3 op = cam.transform.position;
        while (duration > 0f)
        {
            float x = UnityEngine.Random.Range(-shakeMagnitude * magnitude, shakeMagnitude * magnitude);
            float y = UnityEngine.Random.Range(-shakeMagnitude * magnitude, shakeMagnitude * magnitude);
            float z = UnityEngine.Random.Range(-shakeMagnitude * magnitude, shakeMagnitude * magnitude);
            Vector3 ctp = cam.transform.position;
            cam.transform.position = new Vector3(ctp.x + x, ctp.y + y, ctp.z + z);

            duration -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // cam.transform.position = op;
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlaySoundInQuickSuccession(AudioClip clip, int repeat)
    {
        StartCoroutine(RepeatSound(clip, repeat));
    }

    private IEnumerator RepeatSound(AudioClip clip, int repeat)
    {
        while (repeat > 0)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(0.1f);
            repeat--;
        }
    }

    public float GetBPM()
    {
        return composition.bpm;
    }

    public int GetMusicianCount()
    {
        return orchestra.Count;
    }

    public S_Actor GetActiveMusician()
    {
        return activeMusician;
    }

    public DataTile GetTile(Vector3 tile)
    {
        return map.GetTile((int)tile.x, (int)tile.z);
    }

    public S_Actor CheckTileOccupant(Vector3 tile)
    {
        return map.GetTile((int)tile.x, (int)tile.z).occupant;
    }
}
