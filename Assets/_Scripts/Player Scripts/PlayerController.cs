using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    public float moveSpeed = 5.0f;

    private Rigidbody2D _rigidbody;
    private Vector2 directionMemory;

    [SerializeField]
    private Transform projectilePrefab;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update ()
    {
        _rigidbody.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, Input.GetAxisRaw("Vertical") * moveSpeed);

        if (_rigidbody.velocity.x != 0 || _rigidbody.velocity.y != 0)
        {
            directionMemory = _rigidbody.velocity;
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

    public void Shoot()
    {
        Transform p = Instantiate(projectilePrefab) as Transform;
        Projectile projectile = p.GetComponent<Projectile>();
        projectile.Spawn(transform.position, directionMemory);
    }

}
