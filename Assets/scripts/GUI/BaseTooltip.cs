using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BaseTooltip : MonoBehaviour {
	[HideInInspector]
	public RectTransform transform;
	void Awake(){
		gameObject.SetActive(false);
		transform = GetComponent<RectTransform>();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setPosition(Vector3 pos){
		Vector3[] corners = new Vector3[4];
		Vector2 tempPivot = new Vector2(0f,1f);
		transform.pivot = tempPivot;

		transform.position = pos;
		transform.GetWorldCorners(corners);

		if(corners[2].x > Screen.width){
			if(tempPivot.x == 0)
				tempPivot.x = 1;
			else
				tempPivot.x = 0;
		}
		if(corners[2].y > Screen.height){
			if(tempPivot.y == 0)
				tempPivot.y = 1;
			else
				tempPivot.y = 0;
		}

		if(corners[0].x < 0){
			if(tempPivot.x == 0)
				tempPivot.x = 1;
			else
				tempPivot.x = 0;
		}
		if(corners[0].y < 0){
			if(tempPivot.y == 0)
				tempPivot.y = 1;
			else
				tempPivot.y = 0;
		}
		transform.pivot = tempPivot;

	}

	public void Hide(){
		gameObject.SetActive(false);
	}
}
