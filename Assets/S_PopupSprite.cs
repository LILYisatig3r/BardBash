using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PopupSprite : MonoBehaviour {
	
    public void SpriteMover()
    {
        StartCoroutine("SpriteMoverRoutine");
    }
	
    public IEnumerator SpriteMoverRoutine()
    {
        float n = 100;
        float y = 0;
        while (n > 0)
        {
            transform.position = new Vector3(transform.position.x, 1 + Mathf.Sin(y), transform.position.z);
            y += Time.deltaTime;
            n--;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
