using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    private Rigidbody2D _rigidbody;
    PlayerController holder;
	
	// Update is called once per frame
	void FixedUpdate () {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.velocity = holder.GetComponent<Rigidbody2D>().velocity;
	}

    public void Attack(PlayerController attacker)
    {
        holder = attacker;
        Vector2 direction = attacker.GetComponent<Rigidbody2D>().velocity.normalized;
        float angle = Vector2.Angle(new Vector2(1, 0), direction);
        if (direction.y < 0)
            angle *= -1;
        transform.position = new Vector3(attacker.transform.position.x + direction.x
            , attacker.transform.position.y + 0.5f + direction.y, -0.5f);
        transform.Rotate(new Vector3(0,0,angle));
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        //if (c.GetComponent<EnemyController>() != null)
        //{
        //    EnemyController enemy = c.gameObject.GetComponent<EnemyController>();
        //    enemy.health -= 3;
        //    if (enemy.health <= 0)
        //    {
        //        Destroy(enemy.gameObject);
        //    }
        //    Destroy(gameObject);
        //}
    }
}
