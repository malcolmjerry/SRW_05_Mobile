using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkController : MonoBehaviour {

  private Transform pilotImage, pilotName, talk1, talk2, talk3, mask;

  public void Init() {
    pilotImage = transform.Find( "PilotImage" );
    pilotName = transform.Find( "PilotName" );
    talk1 = transform.Find( "Talking1" );
    talk2 = transform.Find( "Talking2" );
    talk3 = transform.Find( "Talking3" );
    mask = transform.Find( "Mask" );
  }

  // Use this for initialization
  void Start () {

  }
	
	// Update is called once per frame
	void Update () {
		
	}

  public void Setup( string imageName, string name, string say1, string say2 = "", string say3 = "" ) {
    if (!string.IsNullOrWhiteSpace( imageName ))
      pilotImage.GetComponent<Image>().sprite = Resources.Load<Sprite>( $"Character/{imageName}" );
    pilotName.GetComponent<Text>().text = name;
    talk1.GetComponent<Text>().text = say1;
    talk2.GetComponent<Text>().text = say2;
    talk3.GetComponent<Text>().text = say3;
    mask.gameObject.SetActive( false );
  }

  public void Mask( bool useMask ) {
    mask.gameObject.SetActive( useMask );
  }
}
