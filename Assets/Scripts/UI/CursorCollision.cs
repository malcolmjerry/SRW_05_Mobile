using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorCollision : MonoBehaviour {

  private Transform renderModel;
  public float defaultY = 0f;

  private void Awake() {
    //m_Rigidbody = GetComponent<Rigidbody>();
    //Debug.Log( $"CursorCollision Awake name: {gameObject.name}" );
    renderModel = transform.Find( "renderer" );
  }

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  void OnTriggerEnter( Collider other ) {
    var collider = other.GetComponent<Collider>();
    if (collider) {
      float boxY = collider.bounds.center.y + collider.bounds.size.y / 2 + defaultY;
      //transform.position = new Vector3( transform.position.x, boxY, transform.position.z );
      //Debug.Log( "CursorCollision OnTriggerEnter" );
      renderModel.transform.position = new Vector3( renderModel.transform.position.x, boxY, renderModel.transform.position.z );
    }
  }

  void OnTriggerExit( Collider other ) {
    /*
    if (other.gameObject.layer == LayerMask.NameToLayer( "MapThing" )) {
      Debug.Log( "CursorCollision OnTriggerExit" );
      renderModel.transform.position = new Vector3( renderModel.transform.position.x, defaultY, renderModel.transform.position.z );
    }
    */
    ResetBoxCollider();
  }

  public void ResetPos() { 

  }

  public void ResetBoxCollider() {
    renderModel.transform.position = new Vector3( renderModel.transform.position.x, defaultY, renderModel.transform.position.z );
    GetComponent<BoxCollider>().enabled = false;
    CoroutineCommon.CallWaitForOneFrame( () => {
      GetComponent<BoxCollider>().enabled = true;
    } );
  }
}
