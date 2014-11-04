using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitTooltipController : MonoBehaviour {
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

	
	void Awake(){
		gameObject.SetActive(false);
	}
	
	public void Show(Unit u){
		gameObject.SetActive(true);
		unitIcon.sprite = u.icon;
		name.text = u.unitName;
		unitClass.text = u.UnitClass.ToString();
		hp.text = ""+u.HP+" / "+u.HPmax;
		mp.text = ""+u.MP+" / "+u.MPmax;
		ap.text = ""+u.AP+" / "+u.APmax;
		str.text = ""+u.Strength;
		dex.text = ""+u.Dexterity;
		mag.text = ""+u.Magic;
		appliedEffectsCount.text =""+ u.unitBaseEffects.effectsAppliedToUnit.Count;
	}
	
	public void Hide(){
		gameObject.SetActive(false);
	}
}
