using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    public delegate void BeatAction(KeyCode action);
    public static event BeatAction Pressed;

    public bool casting = false;
    private float frameCounter = 0;
    int latencyDelay = 0;
    KeyCode preppedInput;
    public float moveSpeed = 5.0f;
    private Rigidbody2D _rigidbody;
    public Vector2 directionMemory;
    private Vector2[] spawnPositions;

    [SerializeField]
    private Transform projectilePrefab;
    [SerializeField]
    private Transform weaponPrefab;
    Weapon _weapon;
    [SerializeField]
    private Transform enemyPrefab;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        spawnPositions = new Vector2[3];
        spawnPositions[0] = new Vector2(-17.5f, -1.0f);
        spawnPositions[1] = new Vector2();
    }

    private void Update ()
    {
        frameCounter--;
        if (frameCounter == 0)
            ResetEffects();
        if (transform.localScale.x != 1.0f)
            BeatUpdate();

        latencyDelay--;
        if ((Input.GetButtonDown("A_Button") || Input.GetKeyDown(KeyCode.K)) && latencyDelay <= 0)
        {
            preppedInput = KeyCode.K;
            latencyDelay = 5;
        }
        if ((Input.GetButtonDown("B_Button") || Input.GetKeyDown(KeyCode.L)) && latencyDelay <= 0)
        {
            preppedInput = KeyCode.L;
            latencyDelay = 5;
        }
        if ((Input.GetButtonDown("X_Button") || Input.GetKeyDown(KeyCode.J)) && latencyDelay <= 0)
        {
            preppedInput = KeyCode.J;
            latencyDelay = 5;
        }
        if ((Input.GetButtonDown("Y_Button") || Input.GetKeyDown(KeyCode.I)) && latencyDelay <= 0)
        {
            preppedInput = KeyCode.I;
            latencyDelay = 5;
        }
        if (latencyDelay == 0)
        {
            Pressed(preppedInput);
        }
    }

    private void FixedUpdate()
    {
        Vector2 lastFrameVelocity = _rigidbody.velocity;
        _rigidbody.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, Input.GetAxisRaw("Vertical") * moveSpeed);

        if (_rigidbody.velocity.magnitude == 0.0 && lastFrameVelocity.magnitude > 0.0)
        {
            directionMemory = lastFrameVelocity;
        }
    }

    public void Shoot()
    {
        BeatUpdate();
        Transform p = Instantiate(projectilePrefab) as Transform;
        Projectile projectile = p.GetComponent<Projectile>();
        Vector2 input = _rigidbody.velocity.magnitude < 0.1 ? directionMemory : _rigidbody.velocity;
        projectile.Spawn(transform.position, input);
    }

    public void Dash()
    {
        BeatUpdate();
        moveSpeed += 10.0f;
        frameCounter = 5;
    }

    public void Melee()
    {
        BeatUpdate();
        Transform w = Instantiate(weaponPrefab) as Transform;
        _weapon = w.GetComponent<Weapon>();
        _weapon.Attack(this);
        moveSpeed += 2.0f;
        frameCounter = 10;
    }

    public void Cast()
    {
        BeatUpdate();
        casting = !casting;
    }

    public void SpellBlast()
    {
        casting = false;
        BeatUpdate();
        for (int i = 0; i < 10; i++)
        {
            Transform p = Instantiate(projectilePrefab) as Transform;
            Projectile projectile = p.GetComponent<Projectile>();
            float r = 1.5f;
            Vector2 dM = new Vector2(directionMemory.x + Random.Range(-r, r), directionMemory.y + Random.Range(-r, r));
            Vector2 V = new Vector2(_rigidbody.velocity.x + Random.Range(-r, r), _rigidbody.velocity.y + Random.Range(-r, r));
            Vector2 input = _rigidbody.velocity.magnitude < 0.1 ? dM : V;
            projectile.Spawn(transform.position, input);
        }
    }

    /**
     * Helper Methods
     **/

    public void ResetEffects()
    {
        moveSpeed = 5.0f;
        if (_weapon != null)
        {
            Destroy(_weapon.gameObject);
            _weapon = null;
        }
    }

    private void BeatUpdate()
    {
        if (transform.localScale.x == 1.0f)
        {
            transform.localScale = new Vector3(1.1f, 1.1f);
        }
        else if (transform.localScale.x == 1.1f)
        {
            transform.localScale = new Vector3(1.2f, 1.2f);
        }
        else if (transform.localScale.x == 1.2f)
        {
            transform.localScale = new Vector3(1.15f, 1.15f);
        }
        else if (transform.localScale.x == 1.15f)
        {
            transform.localScale = new Vector3(1.11f, 1.11f);
        }
        else if (transform.localScale.x == 1.11f)
        {
            transform.localScale = new Vector3(1.05f, 1.05f);
        }
        else if (transform.localScale.x == 1.05f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f);
        }
    }

    /**
     * Legacy Code *
     * Put old code that you don't want to delete here.
     * Make sure to label it properly!
     **/

    /** Enemy spawner
     * Place this in the "OnGui" method to have 'H' spawn an enemy
     **/
    //if (e.keyCode != KeyCode.H)
    //{
    //    Used(e.keyCode);
    //}
    //else
    //{
    //    Transform h = Instantiate(enemyPrefab) as Transform;
    //    float r = Random.Range(0, 3);
    //    if (r >= 0 && r< 1)
    //    {
    //        h.position = new Vector2(Random.Range(-5.62f, 5.3f), Random.Range(-3.2f, 0.75f));
    //    }
    //    else if (r >= 1 && r< 2)
    //    {
    //        h.position = new Vector2(Random.Range(-18.0f, -7.08f), Random.Range(-12.57f, -0.45f));
    //    }
    //    else
    //    {
    //        h.position = new Vector2(Random.Range(-30.4f, -19.52f), Random.Range(-7.0f, -3.08f));
    //    }
    //}

    /**
     * OnGui input
     * Detects key presses using the OnGui method
     **/

    //void OnGUI()
    //{
    //    Event e = Event.current;
    //    if (e.isKey && inputLockout <= 0)
    //    {
    //        if (!e.keyCode.Equals(KeyCode.W) && !e.keyCode.Equals(KeyCode.A)
    //            && !e.keyCode.Equals(KeyCode.S) && !e.keyCode.Equals(KeyCode.D) && !e.keyCode.Equals(KeyCode.None))
    //        {
    //            print("pressed");
    //            Pressed(e.keyCode);
    //            inputLockout = 7;
    //        }
    //    }
    //}
}
