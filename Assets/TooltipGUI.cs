using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnumSpace;

public class TooltipGUI : MonoBehaviour {

	public EffectTooltipController effectTooltip;
	public AbilityTooltipController abilityTooltip;
	public UnitTooltipController unitTooltip;

	public Text title;
	public Text descr;
	private float delay = 1f;
	private RectTransform rectTransform;
	private Vector3 position;

	void Awake(){
		rectTransform = this.GetComponent<RectTransform>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void showTooltipDelayed(TooltipHelperGUI t){
		this.GetComponent<CanvasGroup>().alpha = 0;
		StartCoroutine(waitToShow(t,delay));
	}

	IEnumerator waitToShow (TooltipHelperGUI t, float delay = 0)
	{
		yield return new WaitForSeconds(delay);
		showTooltip(t);
	}

	public void showTooltip(TooltipHelperGUI t){
		if(t.rectTrans != null)
			position = t.rectTrans.position;
		else
			position = t.CanvasPos();

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
		abilityTooltip.Show(a);
		abilityTooltip.GetComponent<RectTransform>().position = position;
	}

	void showEffectTip (TooltipHelperGUI t)
	{
		BaseEffect ef = t.GetByType() as BaseEffect;
		effectTooltip.Show(ef);
		effectTooltip.GetComponent<RectTransform>().position = position;
	}

	void showUnitTip (TooltipHelperGUI t)
	{
		Unit u = t.GetByType() as Unit;
		unitTooltip.Show(u);
		unitTooltip.GetComponent<RectTransform>().position = position;
	}

	public void hideTooltip ()
	{
		effectTooltip.Hide();
		abilityTooltip.Hide();
		unitTooltip.Hide();
		StopAllCoroutines();
		this.GetComponent<CanvasGroup>().alpha = 0;
	}
}
