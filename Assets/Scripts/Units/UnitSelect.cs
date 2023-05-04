using UnityEngine;
using System.Collections;

public class UnitSelect : MonoBehaviour {

  private Transform myRenderer;

  //private string myOnfirmBtnn;

  //public MapManager mapManager;

  readonly float hideTime = .1f;
  readonly float showTime = 1f;
  float maxTime = 0;
  float countTime = 0;

  bool isShow = true;

  private void Awake() {
    //myRenderer = transform.Find( "Renderer" );
    myRenderer = transform.Find( "Canvas/Selected" );
  }
	
	// Update is called once per frame
	void Update () {
    countTime += Time.deltaTime;

    if (countTime > maxTime) {
      isShow = !isShow;
      myRenderer.gameObject.SetActive( isShow );
      countTime = 0;
      maxTime = isShow ? showTime : hideTime;
    }
  }

  void Start() {

  }

  /*
  void FlashSelf() {
    myRenderer.gameObject.SetActive( !myRenderer.gameObject.activeSelf );
  }
  */

  private void OnEnable() {
    //InvokeRepeating( "FlashSelf", 0, .01f );
    myRenderer.gameObject.SetActive( true );
    maxTime = showTime;
    countTime = 0;
    isShow = true;
  }


  private void OnDisable() {
    myRenderer.gameObject.SetActive( false );
    //CancelInvoke( "FlashSelf" );
  }

  /*
  IEnumerator Flash() {
    yield return new WaitForSeconds( 500f );
  }
  */

}


