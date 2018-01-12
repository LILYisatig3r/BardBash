using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotController : MonoBehaviour {

    Vector2 position;
    TilesMap map;

    void Start()
    {
        position = new Vector2(0f, 0f);
        if (map != null || TilesMap.TryGetInstance(out map))
        {
            map.AddPlayer(gameObject);
        }
        transform.position = map.GridToWorldPosition(position);
    }
}
