﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEffectController : MonoBehaviour {

	public List<string> effectsID;
	public List<BaseEffect> effects = new List<BaseEffect>();

	public List<BaseEffect> effectsAppliedToUnit = new List<BaseEffect>();

	private GameManager gm = GameManager.instance;
	private Unit owner;
	void Awake () {
		gm.OnUnitPosChange += OnUnitPosChange;
		gm.OnPlayerTurnStart += PlayerTurnStart;
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

	void PlayerTurnStart (Player p)
	{
		updateEffectsTargets();
		if(owner.playerOwner == p)
		foreach(BaseEffect ef in effectsAppliedToUnit)
		{
			ef.applyTo(owner);
		}
	}

	public void initEffects (){
		for(int i=0;i<effectsID.Count;i++)
		{
			if(BaseEffectsManager.instance.getEffect(effectsID[i]) != null){
				addEffect(BaseEffectsManager.instance.getEffect(effectsID[i]));
			}
		}
		updateEffectsTargets();
	}

	public void deleteAllEffects(bool useDeathState = false)
	{
		if(useDeathState){
			List<BaseEffect> tempList = new List<BaseEffect>();
			foreach(BaseEffect ef in effects)
			{
				if (ef.deleteAfterOwnerDeath)
					tempList.Add(ef);
			}
			foreach(BaseEffect ef in tempList)
			{
				ef.Delete();
			}

		}
		else
			effects.Clear();
	}

	public void addEffect(BaseEffect ef,Unit target = null)
	{
		if((ef.requireTarget) && (target != null))
			ef.Init(owner,target);
		else
			ef.Init(owner);

		if(!effects.Contains(ef)){
			effects.Add(ef);
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
			UnitEvents.UnitEffectAdded(owner,ef);
		}
		else
			Debug.Log("This effects already applied");
	}

	public void removeAppliedEffect(BaseEffect ef)
	{
		if(effectsAppliedToUnit.Contains(ef)){
			effectsAppliedToUnit.Remove(ef);
			updateModsFromAppliedEffects();
			UnitEvents.UnitEffectRemoved(owner,ef);
		}
	}

	public void updateModsFromAppliedEffects()
	{
//		updateEffectsTargets();
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
