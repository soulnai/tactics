using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class StartScreen : MonoBehaviour {

	public ToggleGroup switchArray;
	public Toggle def;
	public Toggle knigth;
	public Toggle scout;

	public GameObject[] userUnitPrefabs ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnToggleChange() {
		if (def.isOn) {
						Debug.Log ("Defaut model");
			StartScreenPersistentObj.instance.UserUnitPrefab[0] = userUnitPrefabs[0];
				} 
		if (knigth.isOn) {
						Debug.Log ("Knigth model");
			StartScreenPersistentObj.instance.UserUnitPrefab[0] = userUnitPrefabs[1];
				} 
		if (scout.isOn) {
						Debug.Log ("Scout model");
			StartScreenPersistentObj.instance.UserUnitPrefab[0] = userUnitPrefabs[2];
				}

	}

	public void OnToggleChange2() {
		if (def.isOn) {
			Debug.Log ("Defaut model");
			StartScreenPersistentObj.instance.UserUnitPrefab[1] = userUnitPrefabs[0];
		} 
		if (knigth.isOn) {
			Debug.Log ("Knigth model");
			StartScreenPersistentObj.instance.UserUnitPrefab[1] = userUnitPrefabs[1];
		} 
		if (scout.isOn) {
			Debug.Log ("Scout model");
			StartScreenPersistentObj.instance.UserUnitPrefab[1] = userUnitPrefabs[2];
		}
		
	}

	public void OnToggleChange3() {
		if (def.isOn) {
			Debug.Log ("Defaut model");
			StartScreenPersistentObj.instance.UserUnitPrefab[2] = userUnitPrefabs[0];
		} 
		if (knigth.isOn) {
			Debug.Log ("Knigth model");
			StartScreenPersistentObj.instance.UserUnitPrefab[2] = userUnitPrefabs[1];
		} 
		if (scout.isOn) {
			Debug.Log ("Scout model");
			StartScreenPersistentObj.instance.UserUnitPrefab[2] = userUnitPrefabs[2];
		}
		
	}

	public void OnToggleChange4() {
		if (def.isOn) {
			Debug.Log ("Defaut model");
			StartScreenPersistentObj.instance.UserUnitPrefab[3] = userUnitPrefabs[0];
		} 
		if (knigth.isOn) {
			Debug.Log ("Knigth model");
			StartScreenPersistentObj.instance.UserUnitPrefab[3] = userUnitPrefabs[1];
		} 
		if (scout.isOn) {
			Debug.Log ("Scout model");
			StartScreenPersistentObj.instance.UserUnitPrefab[3] = userUnitPrefabs[2];
		}
		
	}

	public void MainSceneLoad() {
		Application.LoadLevel ("gameScene");
	}

	public void MapEditorSceneLoad() {
		Application.LoadLevel ("MapCreatorScene");
	}
}
