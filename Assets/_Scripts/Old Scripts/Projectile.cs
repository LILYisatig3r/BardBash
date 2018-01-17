using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Rigidbody _rigidbody;
    private float lifespan = 1;
    private float damage = 1;
    private GameManager gm;

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

    public void Spawn(Vector3 position, Vector3 direction, float damage)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = Random.ColorHSV();
        this.damage = damage;

        transform.position = new Vector3(position.x, position.y - 0.25f, position.z);
        _rigidbody = GetComponent<Rigidbody>();
        direction = direction.normalized;
        if (direction.z != 0)
            transform.Rotate(new Vector3(0f, 90f, 0f));
        _rigidbody.velocity = new Vector3(direction.x * speed, direction.y * speed, direction.z * speed);
    }

    void OnTriggerEnter(Collider c)
    {
        if (gm != null || GameManager.TryGetInstance(out gm))
        {
            if (c.tag.Equals("Enemy"))
            {
                gm.ActorDamaged(c.gameObject, damage);
                Destroy(gameObject);
            }
            else if (c.tag.Equals("Terrain"))
            {
                Destroy(gameObject);
            }
        }
    }
}
