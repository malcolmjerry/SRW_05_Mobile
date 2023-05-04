using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeColider : MonoBehaviour {


  public void Setup( float moveRange ) {
    transform.localScale = new Vector3( moveRange, transform.localScale.y, moveRange );
    transform.Find( "CapsuleUnitWallRenderer" ).GetComponent<AddInvertedMeshCollider>().Setup();
  }

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
