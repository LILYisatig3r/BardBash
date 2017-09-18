using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2;
    public int health = 3;

    private CircleCollider2D hitbox;

    private void Start()
    {
        hitbox = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        //if (player != null)
        //{
        //    float X = player.position.x - transform.position.x;
        //    float Y = player.position.y - transform.position.y;
        //    transform.Translate((new Vector2(X, Y).normalized) * Time.deltaTime * speed);
        //}
    }

    //private void OnDrawGizmos()
    //{
    //    if (_aggroCollider == null)
    //    {
    //        _aggroCollider = GetComponent<CircleCollider2D>();
    //    }
        
    //    UnityEditor.Handles.color = Color.yellow;
    //    UnityEditor.Handles.DrawWireDisc(new Vector2(_aggroCollider.transform.position.x, _aggroCollider.transform.position.y) + _aggroCollider.offset, Vector3.back, aggroRange);
        
    //}
}
