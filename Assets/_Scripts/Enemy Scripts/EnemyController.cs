using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : S_Actor
{
    public float speed = 2;

    private void Start()
    {
        maxHp = 5;
        curHp = maxHp;
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
