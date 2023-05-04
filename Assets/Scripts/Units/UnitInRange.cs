using UnityEngine;
using System.Collections;

public class UnitInRange : MonoBehaviour {

  private Transform myHealthBar;

  private void Awake() {
    myHealthBar = transform.Find( "Canvas" );
  }
	
	// Update is called once per frame
	void Update () {

  }

  void Start() {

  }

  void FlashSelf() {
    myHealthBar.gameObject.SetActive( !myHealthBar.gameObject.activeSelf );
  }

  private void OnEnable() {
    InvokeRepeating( "FlashSelf", 0, .2f );
  }


  private void OnDisable() {
    myHealthBar.gameObject.SetActive( true );
    CancelInvoke( "FlashSelf" );
  }


}


