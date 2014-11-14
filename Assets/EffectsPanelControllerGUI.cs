using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectsPanelControllerGUI : MonoBehaviour {
	public List<EffectPanelGUI> effectPanels;
	private List<EffectPanelGUI> effectPanelsUsed = new List<EffectPanelGUI>();
	private List<EffectPanelGUI> effectPanelsUnused = new List<EffectPanelGUI>();

	private Unit owner;

	public void Init(Unit u){
		owner = u;
	}

	// Use this for initialization
	void Start () {
		EventManager.OnUnitEffectChanged += updateEffectPanels;
		EventManager.OnUnitEffectAdded += addEffectPanel;
		EventManager.OnUnitEffectRemoved += removeEffectPanel;

		effectPanelsUnused = effectPanels;
	}

	void addEffectPanel(Unit u, BaseEffect ef)
	{
		if(u == owner){
			effectPanelsUnused[0].Init(ef);
			effectPanelsUsed.Add(effectPanelsUnused[0]);
			effectPanelsUnused.RemoveAt(0);
		}
	}

	void removeEffectPanel(Unit u, BaseEffect ef)
	{
		if(u == owner){
			EffectPanelGUI tempPanel = effectPanelsUsed.Find(EffectPanelGUI => EffectPanelGUI.effect == ef) as EffectPanelGUI;
			if(tempPanel != null){
				tempPanel.Delete();
				effectPanelsUnused.Add(tempPanel);
				effectPanelsUsed.Remove(tempPanel);
			}
//			foreach(EffectPanelGUI panel in effectPanelsUsed){
//				if(panel.effect == ef){
//					effectPanelsUnused.Add(panel);
//				}
//			}
//			foreach(EffectPanelGUI panel in effectPanelsUnused){
//				if(effectPanelsUsed.Contains(panel))
//					effectPanelsUsed.Remove(panel);
//			}
		}
	}

	void updateEffectPanels (Unit u, BaseEffect ef)
	{
        if (ef.targets.Contains(owner))
        {
            foreach (EffectPanelGUI p in effectPanelsUsed)
            {
                if (p.effect == ef)
                {
                    p.updateCounter();
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy(){
		EventManager.OnUnitEffectChanged -= updateEffectPanels;
		EventManager.OnUnitEffectAdded -= addEffectPanel;
		EventManager.OnUnitEffectRemoved -= removeEffectPanel;
	}
}
