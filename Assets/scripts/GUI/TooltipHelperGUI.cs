﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using EnumSpace;

public class TooltipHelperGUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
	public tooltipTypes type;
	[HideInInspector]
	public RectTransform rectTrans;

	void Awake () {
		rectTrans = GetComponent<RectTransform>();
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerEnter(PointerEventData data)
	{
		GUImanager.instance.showTooltip(this);
	}

	public void OnPointerExit(PointerEventData data)
	{
		GUImanager.instance.hideTooltip();
	}

	public object GetByType(){
		object temp = null;
		switch(type){
		case tooltipTypes.ability:
			if(GetComponent<buttonController>() != null)
				temp = GetComponent<buttonController>().assignedElement;
			break;
		case tooltipTypes.effect:
			if(GetComponent<EffectPanelGUI>() != null)
				temp = GetComponent<EffectPanelGUI>().effect;
			break;
		case tooltipTypes.unit:
			if(GetComponent<Unit>() != null)
				temp = this.GetComponent<Unit>();
			else if(GetComponentInParent<UnitPanelGUI>() != null)
				temp = GetComponentInParent<UnitPanelGUI>().targetUnit;
			break;
		}
		return temp;
	}

	public Vector3 CanvasPos()
	{
		Vector3 tempPos = Camera.main.WorldToScreenPoint(transform.position);
		tempPos.z = 0;

		return tempPos;
	}
}
