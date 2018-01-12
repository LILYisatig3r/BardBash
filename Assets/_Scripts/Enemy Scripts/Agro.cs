using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agro : MonoBehaviour {

    private CircleCollider2D _aggroCollider;
    private Transform player;
    private Transform body;
    private float speed;

    void Start () {
        _aggroCollider = GetComponent<CircleCollider2D>();
        speed = transform.parent.GetComponent<EnemyController>().speed;
        body = transform.parent.transform;
    }
	
	void Update () {
        if (player != null)
        {
            float X = player.position.x - transform.position.x;
            float Y = player.position.y - transform.position.y;
            body.Translate((new Vector2(X, Y).normalized) * Time.deltaTime * speed);
        }
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag.Equals("Player"))
        {
            player = c.transform;
        }
    }
}
