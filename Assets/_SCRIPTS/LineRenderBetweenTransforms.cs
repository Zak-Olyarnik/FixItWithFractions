using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineRenderBetweenTransforms : MonoBehaviour {

    [SerializeField] private Transform[] points;
    [SerializeField] private Vector3[] offsets;

    private LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (points.Length > 1)
        {
            for (int i = 0; i < points.Length; i++)
            {
                if (offsets.Length > i)
                    lineRenderer.SetPosition(i, points[i].position + this.transform.TransformDirection(offsets[i]));
                else
                    lineRenderer.SetPosition(i, points[i].position);
            }
        }
	}
}
