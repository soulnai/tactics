using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectsController : MonoBehaviour {
	public List<BaseEffect> effects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public BaseEffect getEffect(BaseEffect effect)
	{
		Debug.Log("No such effect");
		return null;
	}

	public void AddEffect(BaseEffect effect)
	{
		effects.Add(effect);
	}

	public void ActivateEffects()
	{
		foreach(BaseEffect ef in effects)
			ef.Activate();
	}

	public void DeleteEffect(BaseEffect effect)
	{
		if((effect != null)&&(effects.Contains(effect)))
		effects.Remove(effect);
	}

	public void Debuff()
	{
		foreach(BaseEffect ef in effects)
		{
			if(ef.canBeDebuffed)
			{
				DeleteEffect(ef);
			}
		}
	}

	public void DeleteAllEffects()
	{
		effects.Clear();
	}
}
