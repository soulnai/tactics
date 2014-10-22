using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEffectController : MonoBehaviour {

	public List<string> effectsID;
	public List<BaseEffect> effects = new List<BaseEffect>();

	public List<BaseEffect> effectsAppliedToUnit = new List<BaseEffect>();

	private GameManager gm = GameManager.instance;
	private Unit owner;
	void Awake () {
		gm.OnRoundStart += OnRoundStart;
		gm.OnTurnStart += OnTurnStart;
		gm.OnUnitPosChange += OnUnitPosChange;
		owner = GetComponent<Unit>();
	}

	void OnUnitPosChange (Unit u)
	{
		updateEffectsTargets();
		updateModsFromAppliedEffects();
	}

	void updateEffectsTargets ()
	{
		foreach (BaseEffect ef in effects) {
			ef.updateTargets();
		}
	}

	void OnTurnStart (Unit u)
	{
		updateEffectsTargets();
		if(u == owner)
			foreach(BaseEffect ef in effectsAppliedToUnit)
			{
				ef.applyTo(owner);
			}
	}

	void OnRoundStart ()
	{

	}
	
	public void initEffects (){
		for(int i=0;i<effectsID.Count;i++)
		{
			if(BaseEffectsManager.instance.getEffect(effectsID[i]) != null){
				BaseEffect tempEffect = BaseEffectsManager.instance.getEffect(effectsID[i]).Clone() as BaseEffect;
				addEffect(tempEffect);
			}
		}
		updateEffectsTargets();
	}

	public void deleteAllEffects()
	{
		effects.Clear();
	}

	public void addEffect(BaseEffect ef)
	{
		ef.Init(owner);
		if(!effects.Contains(ef)){
			effects.Add(ef);
			updateModsFromAppliedEffects();
		}
		else
			Debug.Log("effect already applied");
	}

	public void delete(BaseEffect ef)
	{
		if(effects.Contains(ef)){
			effects.Remove(ef);
		}
		else
			Debug.Log("No such effect");
	}

	public void addAppliedEffect(BaseEffect ef)
	{
		if(!effectsAppliedToUnit.Contains(ef)){
			effectsAppliedToUnit.Add(ef);
			updateModsFromAppliedEffects();
		}
		else
			Debug.Log("This effects already applied");
	}

	public void removeAppliedEffect(BaseEffect ef)
	{
		if(effectsAppliedToUnit.Contains(ef)){
			effectsAppliedToUnit.Remove(ef);
			updateModsFromAppliedEffects();
		}
	}

	public void updateModsFromAppliedEffects()
	{
		updateEffectsTargets();
		foreach(BaseAttribute at in owner.attributes){
			at.clearMods();
		}

		foreach(BaseEffect ef in effectsAppliedToUnit)
		{
			foreach(BaseAttributeChanger ac in ef.affectedAttributes)
			{
				if(ac.mod)
					owner.getAttribute(ac.attribute).addMod(ef.getValue(owner,ac));
			}
		}
	}
}
