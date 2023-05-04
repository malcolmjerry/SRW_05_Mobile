using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryMapCanvas : MonoBehaviour {

  //Transform bgMap,
  Transform locPin;
  LineRenderer line;

  // Use this for initialization
  void Start() {
    //bgMap = transform.Find( "Background" );
    locPin = transform.Find( "LocPin" );
    line = locPin.GetComponent<LineRenderer>();
  }

  // Update is called once per frame
  void Update() {

  }

  public void SetZoomPos( float right, float top ) {
    var rect = locPin.GetComponent<RectTransform>();
    //rect.offsetMax = new Vector2( right, top );
    //rect.offsetMin = new Vector2( right, top );
    rect.localPosition = new Vector3( right, top, 0 );
  }
  
  public void MoveTo( float posX, float posY, float duration ) {
    var newPos = new Vector3( posX, posY, 0 );
    StartCoroutine( MoveToCoroutine( locPin, locPin.localPosition, newPos, duration ) );
  }

  IEnumerator MoveToCoroutine( Transform locPin, Vector3 source, Vector3 dest, float duration ) {
    line.enabled = true;
    line.SetPosition( 0, locPin.position );

    for (float timer = 0; timer < duration; timer += Time.deltaTime) {
      locPin.localPosition = Vector3.Lerp( source, dest, timer / duration );

      //line.material = new Material( Shader.Find( "Particles/Additive" ) );
      //line.SetVertexCount( 2 );//设置两点
      //line.SetColors( Color.yellow, Color.red ); //设置直线颜色
      //line.SetWidth( 0.01f, 0.01f );//设置直线宽度     
      line.SetPosition( 1, locPin.position );

      yield return null;
    }
  }

}
