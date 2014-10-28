using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnumSpace;

public class TooltipGUI : MonoBehaviour {

	public Text title;
	public Text descr;
	private float delay = 1f;
	private RectTransform rectTransform;

	void Awake(){
		gameObject.SetActive(false);
		rectTransform = this.GetComponent<RectTransform>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void showTooltipDelayed(TooltipHelperGUI t){
		gameObject.SetActive(true);
		this.GetComponent<CanvasGroup>().alpha = 0;
		StartCoroutine(waitToShow(t,delay));
	}

	IEnumerator waitToShow (TooltipHelperGUI t, float delay = 0)
	{
		yield return new WaitForSeconds(delay);
		showTooltip(t);
	}

	public void showTooltip(TooltipHelperGUI t){
		this.GetComponent<CanvasGroup>().alpha = 1;
		if(t.rectTrans != null)
			rectTransform.position = t.rectTrans.position;
		else
			rectTransform.position = t.CanvasPos();
		switch(t.type){
			case tooltipTypes.ability:
				showAbilityTip(t);
				break;
			case tooltipTypes.effect:
				showEffectTip(t);
				break;
			case tooltipTypes.unit:
				showUnitTip(t);
				break;
		}
	}

	void showAbilityTip (TooltipHelperGUI t)
	{
		BaseAbility a = t.GetByType() as BaseAbility;
		title.text = a.abilityName;
		descr.text = "";
	}

	void showEffectTip (TooltipHelperGUI t)
	{
		BaseEffect ef = t.GetByType() as BaseEffect;
		title.text = ef.name;
		descr.text = ef.duration.ToString();
	}

	void showUnitTip (TooltipHelperGUI t)
	{
		Unit u = t.GetByType() as Unit;
		title.text = u.unitName;
		descr.text = u.HP.ToString();
	}

	public void hideTooltip ()
	{
		StopAllCoroutines();
		this.GetComponent<CanvasGroup>().alpha = 0;
	}
}
