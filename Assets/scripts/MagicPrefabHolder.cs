using UnityEngine;
using System.Collections;

public class MagicPrefabHolder : MonoBehaviour {
	public static MagicPrefabHolder instance;
	
	public GameObject Fireball;
	
	public GameObject Lightning;
	public GameObject LightningExplode;

	
	void Awake() {
		instance = this;
	}
}