using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnumSpace;

public class UnitPanelGUI : MonoBehaviour {
	public Image icon;
    public Text unitName;
    public Text unitClass;
	public Image selection;
	public Slider HPslider;
	public Slider MPslider;
	public Slider APslider;
	public EffectsPanelControllerGUI effectsGUI;
	public Text castCounter;

	public Unit targetUnit;
    // Use this for initialization
	void Awake(){
		gameObject.SetActive(false);
		castCounter.gameObject.SetActive(false);
	    if (selection != null) selection.gameObject.SetActive(false);
	    EventManager.onUnitSelectionChanged += updateSelectionBox;
		EventManager.onAttributeChanged += updateAttribute;
		EventManager.OnUnitCastDelayChanged += updateCastCounter;
        EventManager.OnUnitDead += updateIcon;
	}

    private void updateIcon(Unit unit)
    {
        if (unit == targetUnit)
        {
            icon.sprite = unit.iconDead;
        }
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
			UpdateValue(at.attribute);

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

	    if (unitName != null)
	    {
	        unitName.text = target.unitName;
	    }
        if (unitClass != null)
        {
            unitClass.text = ""+target.UnitClass;
        }

		icon.sprite = targetUnit.icon;
		UpdateValue(unitAttributes.AP);
		UpdateValue(unitAttributes.HP);
		UpdateValue(unitAttributes.MP);
	    effectsGUI.Init(targetUnit);

		if(icon.GetComponent<Button>() != null)
		{
			icon.GetComponent<Button>().onClick.RemoveAllListeners();
			icon.GetComponent<Button>().onClick.AddListener(delegate{OnIconClick(target);});
		}
	}

	void OnIconClick (Unit target)
	{
        if ((GameManager.instance.currentUnit.UnitAction == unitActions.idle) || (GameManager.instance.currentUnit.UnitAction == unitActions.readyToMove))
			GameManager.instance.selectUnit(target);
		else
		{
			//TODO onUnit/Tile click event for abilities
			EventManager.UnitClick(targetUnit);
		}
	}

	public void UpdateValue(unitAttributes at)
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

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
	// Update is called once per frame
	void Update () {

	}

	void OnDestroy(){
		EventManager.onUnitSelectionChanged -= updateSelectionBox;
		EventManager.onAttributeChanged -= updateAttribute;
		EventManager.OnUnitCastDelayChanged -= updateCastCounter;
	}
}
