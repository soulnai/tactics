using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MouseOverGUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {

	public bool checkMouseOverGUI = true;

	public void OnPointerEnter(PointerEventData data)
	{
		if(checkMouseOverGUI)
			GUImanager.instance.mouseOverGUI = true;
	}
	
	public void OnPointerExit(PointerEventData data)
	{
		if(checkMouseOverGUI)
			GUImanager.instance.mouseOverGUI = false;
	}
}
