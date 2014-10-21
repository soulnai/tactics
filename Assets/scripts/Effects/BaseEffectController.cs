using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEffectController : MonoBehaviour {

	public List<string> effectsID;
	public List<BaseEffect> effects = new List<BaseEffect>();

	public List<BaseEffect> effectsAppliedToUnit = new List<BaseEffect>();

	private GameManager gm = GameManager.instance;
	private Unit owner;
	void Start () {
		gm.OnRoundStart += OnRoundStart;
		gm.OnTurnStart += OnTurnStart;
		gm.OnUnitPosChange += OnUnitPosChange;
		owner = GetComponent<Unit>();
		initEffects();
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
				tempEffect.owner = this.GetComponent<Unit>();
				tempEffect.Init();
				effects.Add(tempEffect);
			}
		}
	}

	public void deleteAllEffects()
	{
		effects.Clear();
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
