using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class S_BasicCastEffect : MonoBehaviour {

    public LineRenderer line;
    public ParticleSystem particles;
    public float lifetime;

    private Vector3[] anchorPositions;
    private Vector3[] positions;
    private Vector3 reduction;
    private float t;

    private void Start()
    {
        lifetime = 1f;
    }

    void Update () {
	    if (t < lifetime)
	    {
	        for (int i = 0; i < line.positionCount; i++)
	        {
	            positions[i] = new Vector3(positions[i].x * reduction.x, positions[i].y * reduction.y, positions[i].z * reduction.z);
	        }
	        line.SetPositions(positions);
	        line.widthMultiplier *= 0.95f;
	        t += Time.deltaTime;
	    }
	    else
	    {
	        Destroy(gameObject);
	    }
	}

    public void Initialize(Vector3 origin, Vector3 end, Vector3 dir, float lifetime)
    {
        this.lifetime = lifetime;
        reduction = new Vector3(dir.x == 0f ? 0.95f : 1f, dir.y == 0f ? 0.95f : 1f, dir.z == 0f ? 0.95f : 1f);

        int length = (int)(Mathf.Abs(end.x - origin.x) + Mathf.Abs(end.y - origin.y) + Mathf.Abs(end.z - origin.z));
        line.positionCount = length + 1;
        Vector3 pos = Vector3.zero;
        Vector3 _pos = pos;
        anchorPositions = new Vector3[line.positionCount];
        positions = new Vector3[line.positionCount];
        for (int i = 0; i < line.positionCount; i++)
        {
            anchorPositions[i] = _pos;
            positions[i] = new Vector3(_pos.x + 0.5f * (Random.value - 0.5f), _pos.y + 0.5f * (Random.value - 0.5f),
                _pos.z + 0.5f * (Random.value - 0.5f));
            _pos += dir;
        }

        line.SetPositions(positions);
        particles.transform.localPosition = positions[positions.Length - 1];
    }
}
