// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour 
{
	//
	// VARIABLES
	//
	
	public float turnSpeed = 4.0f;		// Speed of camera turning when mouse moves in along an axis
	public float panSpeed = 4.0f;		// Speed of the camera when being panned
	public float zoomSpeed = 4.0f;		// Speed of the camera going back and forth
	
	private Vector3 mouseOrigin;	// Position of cursor when mouse dragging starts
	private bool isPanning;		// Is the camera being panned?
	private bool isRotating;	// Is the camera being rotated?
	private bool isZooming;		// Is the camera zooming?
	
	//
	// UPDATE
	//
	
	void Update () 
	{
		// Get the left mouse button
		if(Input.GetMouseButtonDown(0))
		{
			// Get mouse origin
			mouseOrigin = Input.mousePosition;
			isRotating = true;
		}
		
		// Get the right mouse button
		if(Input.GetMouseButtonDown(1))
		{
			// Get mouse origin
			mouseOrigin = Input.mousePosition;
			isPanning = true;
		}
		
		// Get the middle mouse button
		if(Input.GetAxis("Mouse ScrollWheel") != 0)
		{
			// Get mouse origin
			mouseOrigin = Input.mousePosition;
			isZooming = true;
		}
		
		// Disable movements on button release
		if (!Input.GetMouseButton(0)) isRotating=false;
		if (!Input.GetMouseButton(1)) isPanning=false;
		if (Input.GetAxis("Mouse ScrollWheel") == 0) isZooming=false;
		
		// Rotate camera along X and Y axis
		if ((isRotating)&&(!GUImanager.instance.mouseOverGUI))
		{
	        	Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
			transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
		}
		
		// Move the camera on it's XY plane
		if ((isPanning)&&(!GUImanager.instance.mouseOverGUI))
		{
	        	Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin).normalized;
//				pos = pos - (Vector3.Dot(pos, transform.up) * transform.up);
				
			Vector3 move = (((transform.up -(Vector3.Dot(transform.up,Vector3.up)* Vector3.up))*pos.y)
				+((transform.right -(Vector3.Dot(transform.right,Vector3.up)* Vector3.up))*pos.x)).normalized*panSpeed;
	        	transform.Translate(move, Space.World);
		}
		
		// Move the camera linearly along Z axis
		if ((isZooming)&&(!GUImanager.instance.mouseOverGUI))
		{
				float val = Input.GetAxis("Mouse ScrollWheel");
	        	Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
	        	Vector3 move = -val * zoomSpeed * Vector3.up; 

	        	transform.Translate(move, Space.World);
		}
	}
}