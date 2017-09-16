﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Rigidbody2D _rigidbody;
    private float lifespan = 1;

    [SerializeField]
    private float speed;

    void Update()
    {
        print(lifespan);
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
        if (direction.x == 0 && direction.y == 0)
        {
            direction.x = -5;
        }
        _rigidbody.velocity = new Vector2(direction.x * speed, direction.y * speed);
    }
}