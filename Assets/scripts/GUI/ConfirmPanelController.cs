using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPanelController : MonoBehaviour {

	public Text title;
	public Text descr;
	public Button yesButton;
	public Button noButton;

	private guiConfirmFunc funcToConfirm;

	void Awake(){
		hideConfirmationPanel();
		UnitEvents.onConfirmRequest += showConfirmationPanel;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void showConfirmationPanel(guiConfirmFunc func){
		funcToConfirm = func;
		gameObject.SetActive(true);
		UnitEvents.LockUI();
	}

	public void hideConfirmationPanel(){
		gameObject.SetActive(false);
		UnitEvents.UnlockUI();
	}

	public void answerYes(){
		hideConfirmationPanel();
		funcToConfirm(false);
	}

	public void answerNo(){
		hideConfirmationPanel();
	}

	void OnDestroy(){
		UnitEvents.onConfirmRequest -= showConfirmationPanel;
	}
}
