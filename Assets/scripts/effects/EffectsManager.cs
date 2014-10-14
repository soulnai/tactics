using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


public class EffectsManager : MonoBehaviour {
	public static EffectsManager instance;

	public List<BaseEffect> effectsList;

	void Awake()
	{
		instance = this;

	}

	// Use this for initialization
	void Start () {
		EffectsContainer ec = new EffectsContainer();
		ec.Save("effects.xml",effectsList);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public BaseEffect getEffect(string ID)
	{
		return effectsList.Find(BaseEffect => BaseEffect.effectID == ID);
	}
}
