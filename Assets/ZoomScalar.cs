using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomScalar : MonoBehaviour
{
    public PinchAndZoom zoomer;

    private float oldTotalZoom;
    private float originalTotalZoom;

    public void Zoomed()
    { 
        float zoomScale = Mathf.Pow(2, (zoomer.totalZoom - originalTotalZoom));
        
        print("zoomScale = " + zoomScale + " zoomer.totalZoom = " + zoomer.totalZoom + " zoomer.zoomScale = " + zoomer.zoomScale);
        
        
    }
    private void Start()
    {
        originalTotalZoom = zoomer.totalZoom;
        oldTotalZoom = zoomer.totalZoom;
    }

    private void Update()
    {
        if (oldTotalZoom != zoomer.totalZoom)
        {
            oldTotalZoom = zoomer.totalZoom;
            Zoomed();
        }
    }
}


