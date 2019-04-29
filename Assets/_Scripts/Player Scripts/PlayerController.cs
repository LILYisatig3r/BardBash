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

    [SerializeField] private GameManager gm;

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
        //maxHp = 20;
        //curHp = maxHp;
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
        //gm.ActorDamaged(gameObject, 1);
        Transform p = Instantiate(projectilePrefab) as Transform;
        Projectile projectile = p.GetComponent<Projectile>();
        Vector2 input = _rigidbody.velocity.magnitude < 0.1 ? directionMemory : _rigidbody.velocity;
        //projectile.Spawn(transform.position, input, 1);
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
            //projectile.Spawn(transform.position, input, 1);
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

}
