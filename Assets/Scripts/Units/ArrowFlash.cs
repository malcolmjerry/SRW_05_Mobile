using UnityEngine;
using System.Collections;

public class ArrowFlash : MonoBehaviour {

  private Transform myRenderer;

  //private string myOnfirmBtnn;

  //public MapManager mapManager;

  private void Awake() {
    myRenderer = transform.Find( "Renderer" );
  }

  // Update is called once per frame
  void Update() {
    //if (Input.GetButtonDown( "Confirm" )) {
    //mapManager.m_MenuPrefab.SetActive( true );
    //}
  }

  void Start() {
    flashSelf();
  }

  void flashSelf() {
    float wait = myRenderer.gameObject.activeSelf? 1f : 0.2f;

    CoroutineCommon.CallWaitForSeconds( wait, () => {
      myRenderer.gameObject.SetActive( !myRenderer.gameObject.activeSelf );
      CoroutineCommon.CallWaitForOneFrame( flashSelf );
    } );
  }

  private void OnEnable() {
    //InvokeRepeating( "FlashSelf", 0, .2f );
    //flashSelf();
  }

  /*
  private void OnDisable() {
    myRenderer.gameObject.SetActive( true );
    CancelInvoke( "FlashSelf" );
  }*/

  IEnumerator Flash() {
    yield return new WaitForSeconds( 500f );
  }


}


