using UnityEngine;
using System.Collections;

public class TooltipGUI : MonoBehaviour {

	private float delay = 1f;

	void Awake(){
		gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void showTooltipDelayed(Vector3 tooltipPos){
		gameObject.SetActive(true);
		this.GetComponent<CanvasGroup>().alpha = 0;
		StartCoroutine(waitToShow(tooltipPos,delay));
	}

	IEnumerator waitToShow (Vector3 tooltipPos, float delay)
	{
		yield return new WaitForSeconds(delay);
		showTooltip(tooltipPos);
	}

	public void showTooltip(Vector3 tooltipPos){
		this.GetComponent<CanvasGroup>().alpha = 1;
		this.GetComponent<RectTransform>().position = tooltipPos;
	}

	public void hideTooltip ()
	{
		StopAllCoroutines();
		this.GetComponent<CanvasGroup>().alpha = 0;
	}
}
