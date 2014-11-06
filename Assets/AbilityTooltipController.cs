using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityTooltipController : BaseTooltip {
	public Text name;
	public Text description;

	public void Show(BaseAbility a,Vector3 pos){
		gameObject.SetActive(true);
		setPosition(pos);
		name.text = a.abilityName;
		description.text = a.attackType.ToString();
	}
}
