using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeoController : MonoBehaviour {

  public Transform placeNameTxt;

  void Awake() {
    //placeNameTxt = transform.Find( "PilotImage" );
  }

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void Setup( string placeName ) {
    placeNameTxt.GetComponent<Text>().text = placeName;
  }

}
