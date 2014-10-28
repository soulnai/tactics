using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class EffectPanelGUI : MonoBehaviour {

	public Text counter;
	public Image effectImage;
	public BaseEffect effect;

	void Awake () {
		gameObject.SetActive(false);
	}

	public void Init(BaseEffect ef){
		gameObject.SetActive(true);
		effect = ef;
		counter.text = ef.duration.ToString();
	}

	public void updateCounter ()
	{
		counter.text = effect.duration.ToString();
	}

	public void Delete(){
		effect = null;
		counter.text = "0";
		gameObject.SetActive(false);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
