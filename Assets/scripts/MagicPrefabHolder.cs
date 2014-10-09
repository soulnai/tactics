using UnityEngine;
using System.Collections;

public class MagicPrefabHolder : MonoBehaviour {
	public static MagicPrefabHolder instance;
	
	public GameObject Fireball;
	public GameObject FireballExplode;
	public GameObject Lightning;
	public GameObject LightningExplode;
	public GameObject Poison;
	public GameObject PoisonExplode;
	public GameObject Freeze;
	public GameObject FreezeExplode;
	public GameObject Heal;
	public GameObject HealExplode;

	
	void Awake() {
		instance = this;
	}
}