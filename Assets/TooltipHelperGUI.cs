using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TooltipHelperGUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
	private GUImanager guim;

	// Use this for initialization
	void Start () {
		guim = GUImanager.instance;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerEnter(PointerEventData data)
	{
		guim.showTooltip(this.GetComponent<RectTransform>().position);
	}

	public void OnPointerExit(PointerEventData data)
	{
		guim.hideTooltip();
	}
}
