using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Rigidbody2D _rigidbody;
    private float lifespan = 1;

    [SerializeField]
    private float speed;

    void Update()
    {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Spawn(Vector2 position, Vector2 direction)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = Random.ColorHSV();

        transform.position = position;
        _rigidbody = GetComponent<Rigidbody2D>();
        direction = direction.normalized;
        if (direction.x == 0 && direction.y == 0)
        {
            direction.x = -1;
        }
        _rigidbody.velocity = new Vector2(direction.x * speed, direction.y * speed);
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.GetComponent<EnemyController>() != null)
        {
            EnemyController enemy = c.gameObject.GetComponent<EnemyController>();
            enemy.health -= 1;
            if (enemy.health == 0)
            {
                Destroy(enemy.gameObject);
            }
            Destroy(gameObject);
        }
        else if (c.gameObject.tag.Equals("Terrain"))
        {
            Destroy(gameObject);
        }
    }
}
