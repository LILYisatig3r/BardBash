using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Instrument : MonoBehaviour {

    S_DamageType damageType;
    SpriteRenderer sr;
    S_PlayerController player;
    private float x, y, z, angle, rotationSpeed, bobSpeed, radius;

	void Start () {
        damageType = S_DamageType.physical;
        player = GetComponentInParent<S_PlayerController>();
        Vector3 pct = player.transform.position;
        transform.position = new Vector3(pct.x + 0.5f, pct.y + 0.5f, pct.z);
        x = y = z = angle = 0;
        rotationSpeed = 4f;
        bobSpeed = rotationSpeed * 2;
        radius = 1f;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 tp = player.transform.position;
        angle += rotationSpeed * Time.deltaTime;
        y += bobSpeed * Time.deltaTime;
        Vector2 offset = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
        transform.position = new Vector3(tp.x + offset.x, tp.y + 0.25f + (0.25f * Mathf.Sin(y)), tp.z + offset.y);
	}
}
