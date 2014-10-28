using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AbilityTooltipController : MonoBehaviour {
	public Text name;
	public Text description;
	
	void Awake(){
		gameObject.SetActive(false);
	}
	
	public void Show(BaseAbility a){
		gameObject.SetActive(true);
		name.text = a.abilityName;
		description.text = a.attackType.ToString();
	}
	
	public void Hide(){
		gameObject.SetActive(false);
	}
}
