using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectTooltipController : MonoBehaviour {
	public Image icon;
	public Text name;
	public Text description;
	public Text duration;
	public Text owner;

	void Awake(){
		gameObject.SetActive(false);
	}

	public void Show(BaseEffect ef){
		gameObject.SetActive(true);
		if(ef.icon != null)
			icon.sprite = ef.icon;
		name.text = ef.name;
		description.text = ef.affectedAttributes[0].attribute.ToString();
		if(ef.infinite)
			duration.text = "Infinite";
		else
			duration.text = ef.duration.ToString();
		owner.text = ef.owner.unitName;
	}

	public void Hide(){
		gameObject.SetActive(false);
	}
}
