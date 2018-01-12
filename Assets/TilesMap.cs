using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesMap : MonoBehaviour {

    private Dictionary<GameObject, S_PlayerController> playersDictionary;
    private static TilesMap instance = null;
    Vector2 offset;

	void Start () {
        Transform poa = transform.GetChild(0);
        offset = new Vector2(poa.position.x, poa.position.y);
        playersDictionary = new Dictionary<GameObject, S_PlayerController>();

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    public static bool TryGetInstance(out TilesMap tm)
    {
        tm = instance;
        if (instance == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void AddPlayer(GameObject player)
    {
        playersDictionary.Add(player, player.GetComponent<S_PlayerController>());
    }

    public Vector2 GridToWorldPosition(Vector2 pos)
    {
        return new Vector2(pos.x + offset.x, pos.y + offset.y);
    }
}
