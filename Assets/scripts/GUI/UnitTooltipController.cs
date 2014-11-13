using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitTooltipController : BaseTooltip {
	public Image unitIcon;
	public Text name;
	public Text unitClass;
	public Text hp;
	public Text mp;
	public Text ap;
	public Text str;
	public Text dex;
	public Text mag;
	public Text appliedEffectsCount;

	public void Show(Unit u,Vector3 pos){
		gameObject.SetActive(true);
		setPosition(pos);
		unitIcon.sprite = u.icon;
		name.text = u.unitName;
		unitClass.text = u.UnitClass.ToString();
		hp.text = ""+u.HP+" / "+u.HPmax;
		mp.text = ""+u.MP+" / "+u.MPmax;
		ap.text = ""+u.AP+" / "+u.APmax;
		str.text = ""+u.Strength;
		dex.text = ""+u.Dexterity;
		mag.text = ""+u.Magic;
		appliedEffectsCount.text =""+ u.unitEffects.effectsApplied.Count;
	}
}
