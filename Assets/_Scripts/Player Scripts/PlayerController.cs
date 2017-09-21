using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    public delegate void BeatAction(string action);
    public static event BeatAction Used;

    public float moveSpeed = 5.0f;
    private Rigidbody2D _rigidbody;
    private Vector2 directionMemory;
    private Vector2[] spawnPositions;

    [SerializeField]
    private Transform projectilePrefab;
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
        _rigidbody.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, Input.GetAxisRaw("Vertical") * moveSpeed);

        if (_rigidbody.velocity.x != 0 || _rigidbody.velocity.y != 0)
        {
            directionMemory = _rigidbody.velocity;
        }
        
        if (moveSpeed > 5.0f)
        {
            moveSpeed = 5.0f;
        }

        if (_rigidbody.velocity.x < 0)
        {
            // Face left
        }
        else
        {
            // Face right
        }
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey)
        {
            if (e.keyCode == KeyCode.Space)
            {
                Used("attack");
            }
            else if (e.keyCode == KeyCode.J)
            {
                Used("dash");
            }
            else if (e.keyCode == KeyCode.H)
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
        Transform go = Instantiate(projectilePrefab) as Transform;
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.Spawn(transform.position, directionMemory);
    }

    public void Dash()
    {
        moveSpeed = 100.0f;
    }

}
