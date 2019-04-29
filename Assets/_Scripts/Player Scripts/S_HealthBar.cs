using UnityEngine;

public class S_HealthBar : MonoBehaviour {

    [SerializeField] S_Actor actor;
    [SerializeField] SpriteRenderer backBar;
    [SerializeField] SpriteRenderer frontBar;
	
	void Update ()
	{
	    float x = actor.curHp / actor.maxHp;
	    float barPos = (x * 0.25f) - 0.25f;
	    float barWidth = x * 0.5f;
	    frontBar.transform.localPosition = new Vector3(barPos, 0f, 0f);
	    frontBar.size = new Vector2(barWidth, 0.0625f);
	}
}
