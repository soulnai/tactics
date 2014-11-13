using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnumSpace;

public class UnitPanelGUI : MonoBehaviour {
	public Image icon;
	public Image selection;
	public Slider HPslider;
	public Slider MPslider;
	public Slider APslider;
	public EffectsPanelControllerGUI effectsGUI;
	public Text castCounter;

	public Unit targetUnit;
	private bool canUpdate = false;
	// Use this for initialization
	void Awake(){
		gameObject.SetActive(false);
		castCounter.gameObject.SetActive(false);
		selection.gameObject.SetActive(false);
		UnitEvents.onUnitSelectionChanged += updateSelectionBox;
		UnitEvents.onAttributeChanged += updateAttribute;
		UnitEvents.OnUnitCastDelayChanged += updateCastCounter;
	}

	void updateCastCounter(Unit u){

		if(u == targetUnit){
			castCounter.text = ""+u.CastingDelay;
			if(u.CastingDelay <= 0)
				castCounter.gameObject.SetActive(false);
			else
				castCounter.gameObject.SetActive(true);
		}
	}

	void updateAttribute (Unit u, BaseAttribute at)
	{
		if(u == targetUnit)
			updateValue(at.attribute);

	}

	public void updateSelectionBox(Unit u){
		if(selection != null){
			if(u == targetUnit)
				selection.gameObject.SetActive(true);
			else
				selection.gameObject.SetActive(false);
		}
	}

	public void Init(Unit target)
	{
		gameObject.SetActive(true);
		targetUnit = target;
		icon.sprite = targetUnit.icon;
		updateValue(unitAttributes.AP);
		updateValue(unitAttributes.HP);
		updateValue(unitAttributes.MP);
		canUpdate = true;
		effectsGUI.Init(targetUnit);

		if(icon.GetComponent<Button>() != null)
		{
			icon.GetComponent<Button>().onClick.RemoveAllListeners();
			icon.GetComponent<Button>().onClick.AddListener(delegate{onIconClick(target);});
		}
	}

	void onIconClick (Unit target)
	{
		if(GameManager.instance.currentUnit.UnitAction == unitActions.idle)
			GameManager.instance.selectUnit(target);
		else
		{
			//TODO onUnit/Tile click event for abilities
			UnitEvents.UnitClick(targetUnit);
		}
	}

	public void updateValue(unitAttributes at)
	{
		switch(at){
		case unitAttributes.AP:
			if(targetUnit.AP > 0)
				APslider.value = ((float)targetUnit.AP/(float)targetUnit.APmax);
			else
				APslider.value = 0;
			break;
		case unitAttributes.HP:
			if(targetUnit.HP > 0)
				HPslider.value = ((float)targetUnit.HP/(float)targetUnit.HPmax);
			else
				HPslider.value = 0;
			break;
		case unitAttributes.MP:
			if(targetUnit.MP > 0)
				MPslider.value = ((float)targetUnit.MP/(float)targetUnit.MPmax);
			else
				MPslider.value = 0;
			break;
		}
	}

	// Update is called once per frame
	void Update () {

	}

	void OnDestroy(){
		UnitEvents.onUnitSelectionChanged -= updateSelectionBox;
		UnitEvents.onAttributeChanged -= updateAttribute;
		UnitEvents.OnUnitCastDelayChanged -= updateCastCounter;
	}
}
