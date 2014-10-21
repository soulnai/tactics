using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


public class BaseEffectsManager : MonoBehaviour {
	public static BaseEffectsManager instance;
	
	public List<BaseEffect> effectsList;
	
	void Awake()
	{
		instance = this;
	}
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public BaseEffect getEffect(string ID)
	{
		BaseEffect tempEf = null;
		if(ID != "")
			tempEf = effectsList.Find(BaseEffect => BaseEffect.ID == ID).Clone() as BaseEffect;
		else
			Debug.Log("empty effect slot");
		if (tempEf == null)
			Debug.Log("No such effect in Effect Manager - " + ID);
		return tempEf;
	}
}
