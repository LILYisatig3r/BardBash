using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMouseOver : MonoBehaviour {
	
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color normalColor;

	void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rch;
        if (GetComponent<Collider>().Raycast(ray,out rch,200f))
        {
            GetComponent<Renderer>().material.color = highlightColor;
        }
        else
        {
            GetComponent<Renderer>().material.color = normalColor;
        }
	}
}
