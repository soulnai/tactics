using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectTooltipController : MonoBehaviour {
	public Text name;
	public Text description;
	public Text duration;
	public Text owner;

	void Awake(){
		gameObject.SetActive(false);
	}

	public void Show(BaseEffect ef){
		gameObject.SetActive(true);
		name.text = ef.name;
		description.text = ef.affectedAttributes[0].attribute.ToString();
		duration.text = ef.duration.ToString();
		owner.text = ef.owner.unitName;
	}

	public void Hide(){
		gameObject.SetActive(false);
	}
}
