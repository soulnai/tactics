using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using EnumSpace;

public class TooltipHelperGUI : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler {
	public tooltipTypes type;
	public RectTransform rectTrans;

	private GUImanager guim;

	void Awake () {
		rectTrans = GetComponent<RectTransform>();
	}
	// Use this for initialization
	void Start () {

		guim = GUImanager.instance;
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

	public void OnMouseEnter()
	{
		GUImanager.instance.showTooltip(this);
	}
	
	public void OnMouseExit()
	{
		GUImanager.instance.hideTooltip();
	}

	public object GetByType(){
		object temp = null;
		switch(type){
		case tooltipTypes.ability:
//			if(GetComponent<AbilitiesController>() != null)
//				temp = this.GetComponent<BaseAbility>();
			break;
		case tooltipTypes.effect:
			if(GetComponent<EffectPanelGUI>() != null)
				temp = GetComponent<EffectPanelGUI>().effect;
			break;
		case tooltipTypes.unit:
			if(GetComponent<Unit>() != null)
				temp = this.GetComponent<Unit>();
			break;
		}
		return temp;
	}

	public Vector3 CanvasPos()
	{
		return Camera.current.WorldToScreenPoint(transform.position);
	}
}
