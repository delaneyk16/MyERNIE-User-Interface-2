/*
 *From: https://www.youtube.com/watch?v=KkYco_7-ULA
Set this on an empty game object positioned at (0,0,0) and attach your active camera.
The script only runs on mobile devices or the remote app.
*/

using UnityEngine;
using Mapbox.Unity.Location;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Mapbox.Unity.Map;

public class PinchAndZoom : MonoBehaviour
{

    public Camera Camera;
    public AbstractMap _map;
    public bool Rotate;
    protected Plane Plane;

    public float totalZoom = 16;
    public int zoomScale = -200;

    private void Awake()
    {

    }
    private void Start()
    {
        
    }

    private void Update()
    {

        //Update Plane
        if (Input.touchCount >= 1)
        {
            print("User trying to pinch");
            Plane.SetNormalAndPosition(transform.up, transform.position);

        }
        var Delta1 = Vector3.zero;
        var Delta2 = Vector3.zero;

        //Scroll
        if (Input.touchCount >= 1)
        {
            print("User trying to pinch2");
            Delta1 = PlanePositionDelta(Input.GetTouch(0));
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
               
            }
            
        }

        //Pinch
        if (Input.touchCount >= 2)
        {
            print("User trying to pinch3");
            var pos1 = (Input.GetTouch(0).position);
            var pos2 = (Input.GetTouch(1).position);
            var pos1b = (Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            var pos2b = (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            var zoom = Vector3.Distance(pos1b, pos2b) - Vector3.Distance(pos1, pos2);
            print("zoom before divide" + zoom+" pos1"+ Input.GetTouch(0).deltaPosition+" "+ Input.GetTouch(1).deltaPosition);
            //scale down so tiny pinch doesn't show world
            zoom = zoom / (float)zoomScale;


            print("zoom" + zoom);
            //edge case
            if (zoom > 1)
            {
                zoom = 1;
            }

            totalZoom += zoom;

            if (totalZoom < 1)
            {
                totalZoom = 1;

            }
            _map.UpdateMap(totalZoom);
        }

    }

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        //not moved
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;

        //delta
        var rayBefore = Camera.ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = Camera.ScreenPointToRay(touch.position);
        if (Plane.Raycast(rayBefore, out var enterBefore) && Plane.Raycast(rayNow, out var enterNow))
            return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

        //not on plane
        return Vector3.zero;
    }

    protected Vector3 PlanePosition(Vector2 screenPos)
    {
        //position
        var rayNow = Camera.ScreenPointToRay(screenPos);
        if (Plane.Raycast(rayNow, out var enterNow))
            return rayNow.GetPoint(enterNow);

        return Vector3.zero;
    }



}
