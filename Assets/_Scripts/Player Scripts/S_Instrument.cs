using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Instrument : MonoBehaviour {

    [Header("Variables")]
    [SerializeField] S_DamageType damageType;

    [SerializeField] private float multiplierIntellect;
    [SerializeField] private float multiplierIntuition;
    [SerializeField] private float multiplierInveiglement;

    #region Hidden Variables

    SpriteRenderer sr;
    S_Actor actor;
    private float x, y, z, angle, rotationSpeed, bobSpeed, radius;
    #endregion

    void Start () {
        actor = GetComponentInParent<S_Actor>();
        Vector3 pct = actor.transform.position;
        transform.position = new Vector3(pct.x + 0.5f, pct.y + 0.5f, pct.z);
        x = y = z = angle = 0;
        rotationSpeed = 4f;
        bobSpeed = rotationSpeed * 2;
        radius = 1f;
	}
	
	void Update () {
        Vector3 tp = actor.transform.position;
        angle += rotationSpeed * Time.deltaTime;
        z += bobSpeed * Time.deltaTime;
        Vector3 offset = new Vector3(Mathf.Sin(angle), 0.25f * Mathf.Sin(2f * angle), Mathf.Cos(angle)) * radius;
        transform.position = new Vector3(tp.x + offset.x, tp.y + 0.15f + offset.y, tp.z + offset.z);
	}

    public float[] StatCalculator(float[] core)
    {
        return new float[]
            {core[3] * multiplierIntellect, core[4] * multiplierIntuition, core[5] * multiplierInveiglement};
    }
}
