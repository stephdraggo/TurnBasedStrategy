using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    LineRenderer lineRenderer;
    Transform boatTransform;

    public void ConnectLine(Transform _boatTransform)
    {
        lineRenderer = GetComponent<LineRenderer>();
        boatTransform = _boatTransform;
    }

    private void Update()
    {
        if (boatTransform) 
        {
            lineRenderer.SetPosition(0, boatTransform.position);
            lineRenderer.SetPosition(1, transform.position);
        }
        
    }
}
