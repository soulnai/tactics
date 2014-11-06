using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EffectTooltipController : BaseTooltip {
	public Image icon;
	public Text name;
	public Text description;
	public Text duration;
	public Text owner;

	public void Show(BaseEffect ef,Vector3 pos){
		gameObject.SetActive(true);
		setPosition(pos);
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
}
