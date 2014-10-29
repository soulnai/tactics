using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitTooltipController : MonoBehaviour {
	public Text name;
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
		name.text = u.unitName;
		hp.text = ""+u.HP;
		mp.text = ""+u.MP;
		ap.text = ""+u.AP;
		str.text = ""+u.Strength;
		dex.text = ""+u.Dexterity;
		mag.text = ""+u.Magic;
		appliedEffectsCount.text =""+ u.unitBaseEffects.effectsAppliedToUnit.Count;
	}
	
	public void Hide(){
		gameObject.SetActive(false);
	}
}
