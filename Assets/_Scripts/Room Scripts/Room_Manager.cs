using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_Manager : MonoBehaviour
{
    public GameObject[] topWalls;
    public GameObject fogOfWar;

    public float minAlpha = 0.0f;
    public float maxAlpha = 1f;
    public float fadeInDuration = .2f;
    public float fadeOutDuration = 2f;
    
    private Camera cam;
    private Dungeon_Manager dm;
    private SpriteRenderer[] wallSprites;
    private SpriteRenderer exploredSprite;
    private float fadeStartTime;
    private bool active = false;
    private bool explored = false;

    private void Awake ()
    {
        cam = Camera.main;
        dm = transform.parent.GetComponent<Dungeon_Manager>();
        wallSprites = new SpriteRenderer[topWalls.Length];
        exploredSprite = fogOfWar.GetComponent<SpriteRenderer>();
        fadeStartTime = -10;

        int i = 0;
        foreach (GameObject go in topWalls)
        {
            wallSprites[i] = go.GetComponent<SpriteRenderer>();
            i++;
        }
	}

    private void Start()
    {
        exploredSprite.color = new Color(1f, 1f, 1f, 1);
    }

    private void Update()
    {
        if (active)
        {
            float t = (Time.time - fadeStartTime) / fadeInDuration;
            foreach (SpriteRenderer sprite in wallSprites)
                sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(minAlpha, maxAlpha, t));
        }
        else
        {
            float t = (Time.time - fadeStartTime) / fadeOutDuration;
            foreach (SpriteRenderer sprite in wallSprites)
                sprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(maxAlpha, minAlpha, t));

            if (explored)
                exploredSprite.color = new Color(1f, 1f, 1f, Mathf.SmoothStep(0f, .5f, t));
            else
                exploredSprite.color = new Color(1f, 1f, 1f, 1);
        }
    }

    public void ActivateRoom()
    {
        if (!active)
        {
            dm.DeactivateActiveRoom();
            cam.GetComponent<CameraController>().target = transform;
            active = true;
            explored = true;
            fadeStartTime = Time.time;
            exploredSprite.color = new Color(1f, 1f, 1f, 0);
            dm.activeRoom = this;
        }
    }

    public void DeactivateRoom()
    {
        active = false;
        fadeStartTime = Time.time;
    }
}
