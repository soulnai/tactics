using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEffectController : MonoBehaviour {

	public List<string> effectsID;
	public List<BaseEffect> effects = new List<BaseEffect>();

	public List<BaseEffect> effectsAppliedToUnit = new List<BaseEffect>();

	void Start () {
		initEffects();
	}
	
	public void initEffects (){
		for(int i=0;i<effectsID.Count;i++)
		{
			BaseEffect tempEffect = BaseEffectsManager.instance.getEffect(effectsID[i]).Clone() as BaseEffect;
			tempEffect.owner = this.GetComponent<Unit>();
			tempEffect.Init();
			effects.Add(tempEffect);
		}
	}

	public void activateAllEffects()
	{
		foreach(BaseEffect ef in effects)
		{
			ef.updateTargets();
			ef.applyToAllTargets();
		}
	}

	public void deleteAllEffects()
	{
		effects.Clear();
	}

	public void delete(BaseEffect ef)
	{
		if(effects.Contains(ef)){
//			ef.Delete();
			effects.Remove(ef);
		}
		else
			Debug.Log("No such effect");
	}

	public void addAppliedEffect(BaseEffect ef)
	{
		if(!effectsAppliedToUnit.Contains(ef))
			effectsAppliedToUnit.Add(ef);
		else
			Debug.Log("This effects already applied");
	}

	public void removeAppliedEffect(BaseEffect ef)
	{
		if(effectsAppliedToUnit.Contains(ef))
			effectsAppliedToUnit.Remove(ef);
	}

	public void updateModsFromAppliedEffects()
	{
		Unit u = GetComponent<Unit>();
		foreach(BaseEffect ef in effectsAppliedToUnit)
		{
			foreach(BaseAttributeChanger ac in ef.affectedAttributes)
			{
				u.getAttribute(ac.attribute).addMod(ef.calculateValue(u,ac));
			}
		}
	}
}
