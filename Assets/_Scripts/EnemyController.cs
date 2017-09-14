using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int aggroRange = 3;

    private CircleCollider2D _aggroCollider;

    private void Start()
    {
        _aggroCollider = GetComponent<CircleCollider2D>();
        _aggroCollider.radius = aggroRange;
    }

    private void Update()
    {
        // if no target
            // Check if player is in aggro range

        // else follow target
    }

    private void OnDrawGizmos()
    {
        if (_aggroCollider == null)
        {
            _aggroCollider = GetComponent<CircleCollider2D>();
        }
        
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(new Vector2(_aggroCollider.transform.position.x, _aggroCollider.transform.position.y) + _aggroCollider.offset, Vector3.back, aggroRange);
        
    }
}
