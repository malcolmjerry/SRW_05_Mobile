using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefreshChild : MonoBehaviour {

  public GameObject RefreshGo;

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  private void OnEnable() {
    Debug.Log( $"Refresh: {RefreshGo.name}" );
    RefreshGo.SetActive( false );

    CoroutineCommon.CallWaitForOneFrame( () => {
      RefreshGo.SetActive( true );
      Debug.Log( $"Refresh OK: {RefreshGo.name}" );
    } );

  }

}
