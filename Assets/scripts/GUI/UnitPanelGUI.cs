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
	public AttributeSliderController HPslider;
	public AttributeSliderController MPslider;
	public AttributeSliderController APslider;
	public EffectsPanelControllerGUI effectsGUI;
	public Text castCounter;

	public Unit targetUnit;
    public bool showSelectedUnit = false;
    // Use this for initialization
	void Awake(){
		gameObject.SetActive(false);
		castCounter.gameObject.SetActive(false);
	    if (selection != null) selection.gameObject.SetActive(false);
	    EventManager.onUnitSelectionChanged += updateSelectionBox;
	    if (showSelectedUnit)
	    {
	        EventManager.onUnitSelectionChanged += ChangeTargetUnit;
	    }
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



	public void updateSelectionBox(Unit u){
		if(selection != null){
			if(u == targetUnit)
				selection.gameObject.SetActive(true);
			else
				selection.gameObject.SetActive(false);
		}
	}

    public void ChangeTargetUnit(Unit u)
    {
        HPslider.Init(u.getAttribute(unitAttributes.HP));
        MPslider.Init(u.getAttribute(unitAttributes.MP));
        APslider.Init(u.getAttribute(unitAttributes.AP));
        effectsGUI.Init(u);
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
        //TODO init sliders
        HPslider.Init(target.getAttribute(unitAttributes.HP));
        MPslider.Init(target.getAttribute(unitAttributes.MP));
        APslider.Init(target.getAttribute(unitAttributes.AP));
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
		EventManager.OnUnitCastDelayChanged -= updateCastCounter;
	}
}
