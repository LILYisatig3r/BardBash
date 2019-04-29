using System.Collections;
using UnityEngine;

public class S_PopupText : MonoBehaviour {

    public void TextRandomMover()
    {
        StartCoroutine(TextRandomMoverRoutine());
    }

    public IEnumerator TextRandomMoverRoutine()
    {
        float n = 10;
        Transform tr = transform;
        float x = Random.Range(tr.position.x - 1f, tr.position.x + 1f);
        float dx = (x - tr.position.x) / n;
        float y = Random.Range(tr.position.y + 1f, tr.position.y + 2f);
        float dy = (y - tr.position.y) / n;
        for (int i = 0; i < n; i++)
        {
            tr.position = new Vector3(tr.position.x + dx, tr.position.y + dy, tr.position.z);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(gameObject);
    }

    public void SmallTextStraightMover()
    {
        StartCoroutine(SmallTextStraightMoverRoutine());
    }

    public IEnumerator SmallTextStraightMoverRoutine()
    {
        float n = 25;
        Transform tr = transform;
        float dy = (tr.position.y + 0.8f) / n;
        for (int i = 0; i < n; i++)
        {
            tr.position = new Vector3(tr.position.x, tr.position.y + dy, tr.position.z);
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(gameObject);
    }
}
