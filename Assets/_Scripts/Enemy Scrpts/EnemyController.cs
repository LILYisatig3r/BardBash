using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 2;
    public int health = 3;

    private void Start()
    {

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
