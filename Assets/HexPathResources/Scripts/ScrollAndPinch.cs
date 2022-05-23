/*
Set this on an empty game object positioned at (0,0,0) and attach your active camera.
The script only runs on mobile devices or the remote app.
*/

using UnityEngine;

namespace HexPathResources.Scripts
{
    public class ScrollAndPinch : MonoBehaviour
    {
#if UNITY_IOS || UNITY_ANDROID
    public Camera Camera;
    public bool Rotate;
    protected Plane Plane;
	public bool swipeActive;
    public Vector2 lastCoords;


    public CameraController cameraController;

    public PathVisualizer pathVisualizer;

    private void Awake()
    {
        if (Camera == null)
            Camera = Camera.main;
		
		//swipeActive = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void Update()
    {
        if (cameraController.cameraControllingMode == CameraController.CameraControllingMode.Off || (cameraController.cameraControllingMode == CameraController.CameraControllingMode.FreeFollow && pathVisualizer.movingFlag) ) return;
        //Update Plane
        if (Input.touchCount >= 1)
            Plane.SetNormalAndPosition(transform.up, transform.position);

        var Delta1 = Vector3.zero;
        var Delta2 = Vector3.zero;
		
        //Scroll
        if (Input.touchCount >= 1)
        {
            Delta1 = PlanePositionDelta(Input.GetTouch(0));
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
                Camera.transform.Translate(Delta1, Space.World);

            lastCoords = Input.GetTouch(0).position;
		if (Mathf.Abs(Input.GetTouch(0).deltaPosition.x)<pathVisualizer.swipeDelta && Mathf.Abs(Input.GetTouch(0).deltaPosition.y)<pathVisualizer.swipeDelta && (Input.GetTouch(0).phase != TouchPhase.Stationary) && (Input.GetTouch(0).phase != TouchPhase.Ended))
		{
			//if (Input.GetTouch(0).phase != TouchPhase.Moved)
			//{
				swipeActive = true;
				Debug.Log ("tap");
			//}
		}
		else if (Mathf.Abs(Input.GetTouch(0).deltaPosition.x)>=pathVisualizer.swipeDelta && Mathf.Abs(Input.GetTouch(0).deltaPosition.y)>=pathVisualizer.swipeDelta && (Input.GetTouch(0).phase != TouchPhase.Stationary))
		{
			swipeActive = false;
			Debug.Log ("drag");
		}
		Debug.Log ("delta" + Input.GetTouch(0).deltaPosition.x + " " + Input.GetTouch(0).deltaPosition.y + "position" + Input.GetTouch(0).position.x + " " + Input.GetTouch(0).position.y);
		}
        
        //Pinch
        if (Input.touchCount >= 2)
        {
            var pos1  = PlanePosition(Input.GetTouch(0).position);
            var pos2  = PlanePosition(Input.GetTouch(1).position);
            var pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);
			
			

            //calc zoom
            var zoom = Vector3.Distance(pos1, pos2) /
                       Vector3.Distance(pos1b, pos2b);

            //edge case
            if (zoom == 0 || zoom > 5)
                return;

            if ((Camera.transform.position.y > 35 && zoom < 1)  || (Camera.transform.position.y < 10 && zoom > 1)) return;

            //Move cam amount the mid ray
            Camera.transform.position = Vector3.LerpUnclamped(pos1, Camera.transform.position, 1 / zoom);

            if (Rotate && pos2b != pos2)
                Camera.transform.RotateAround(pos1, Plane.normal, Vector3.SignedAngle(pos2 - pos1, pos2b - pos1b, Plane.normal));
        
		
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

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + transform.up);
    }
#endif
    }
}