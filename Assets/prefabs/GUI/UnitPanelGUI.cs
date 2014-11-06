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

	public Unit targetUnit;
	private bool canUpdate = false;
	// Use this for initialization
	void Awake(){
		gameObject.SetActive(false);
		selection.gameObject.SetActive(false);
		UnitEvents.onUnitSelectionChanged += updateSelectionBox;
		UnitEvents.onAttributeChanged += updateAttribute;
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
		GameManager.instance.selectUnit(target);
	}

	public void updateValue(unitAttributes at)
	{
		switch(at){
		case unitAttributes.AP:
			APslider.value = ((float)targetUnit.AP/(float)targetUnit.APmax);
			break;
		case unitAttributes.HP:
			HPslider.value = ((float)targetUnit.HP/(float)targetUnit.HPmax);
			break;
		case unitAttributes.MP:
			MPslider.value = ((float)targetUnit.MP/(float)targetUnit.MPmax);
			break;
		}
	}

	// Update is called once per frame
	void Update () {

	}

	void OnDestroy(){
		UnitEvents.onUnitSelectionChanged -= updateSelectionBox;
	}
}
