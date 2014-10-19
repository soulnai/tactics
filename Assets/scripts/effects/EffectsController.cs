using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectsController : MonoBehaviour {
	public List<BaseEffect> effects;

	private Unit unit;
	private List<BaseEffect> effectsToDelete = new List<BaseEffect>();
	// Use this for initialization
	void Awake(){
		unit = GetComponent<Unit>();
	}

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
		BaseEffect tempEffect = effect.Clone() as BaseEffect;
		effects.Add(tempEffect);
		tempEffect.Init(this,unit);
		tempEffect.Activate(false);
	}

	public void ActivateAllEffects()
	{
		//TODO debug clear list!
		//delete old
		ClearOldEffects();
		//activate
		foreach(BaseEffect ef in effects)
			ef.Activate();
	}

	public void AddToDeleteList(BaseEffect effect)
	{
		if((effect != null)&&(effects.Contains(effect)))
			effectsToDelete.Add(effect);
	}

	/// <summary>
	/// Removes all effects that neede to be removed.
	/// Duration = 0, Owner died, etc
	/// </summary>
	public void ClearOldEffects()
	{
		foreach(BaseEffect ef in effectsToDelete)
		{
			effects.Remove(ef);
		}
		effectsToDelete.Clear();
	}

	public void Debuff()
	{
		foreach(BaseEffect ef in effects)
		{
			if(ef.canBeDebuffed)
			{
				AddToDeleteList(ef);
			}
		}
		ClearOldEffects();
	}

	public void DeleteAllEffects()
	{
		effects.Clear();
	}

	public BaseEffect getEffect(string ID)
	{
		return effects.Find(BaseEffect => BaseEffect.effectID == ID);
	}
}
