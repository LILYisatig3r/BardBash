using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    public delegate void BeatAction(KeyCode action);
    public static event BeatAction Used;

    private float frameCounter = 0;
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

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            if (e.keyCode != KeyCode.H)
            {
                Used(e.keyCode);
            }
            else
            {
                Transform h = Instantiate(enemyPrefab) as Transform;
                float r = Random.Range(0, 3);
                if (r >= 0 && r < 1)
                {
                    h.position = new Vector2(Random.Range(-5.62f, 5.3f), Random.Range(-3.2f, 0.75f));
                }
                else if (r >= 1 && r < 2)
                {
                    h.position = new Vector2(Random.Range(-18.0f, -7.08f), Random.Range(-12.57f, -0.45f));
                }
                else
                {
                    h.position = new Vector2(Random.Range(-30.4f, -19.52f), Random.Range(-7.0f, -3.08f));
                }
            }
        }
    }

    public void Shoot()
    {
        Transform p = Instantiate(projectilePrefab) as Transform;
        Projectile projectile = p.GetComponent<Projectile>();
        Vector2 input = _rigidbody.velocity.magnitude < 0.1 ? directionMemory : _rigidbody.velocity;
        projectile.Spawn(transform.position, input);
    }

    public void Dash()
    {
        moveSpeed = 15.0f;
        frameCounter = 5;
    }

    public void Melee()
    {
        Transform w = Instantiate(weaponPrefab) as Transform;
        _weapon = w.GetComponent<Weapon>();
        _weapon.Attack(this);
        frameCounter = 5;
    }

    public void ResetEffects()
    {
        moveSpeed = 5.0f;
        if (_weapon != null)
        {
            Destroy(_weapon.gameObject);
            _weapon = null;
        }
    }
}
