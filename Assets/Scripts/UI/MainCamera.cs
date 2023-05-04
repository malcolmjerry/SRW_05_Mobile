using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  private static readonly Plane plane = new Plane( Vector3.up, Vector3.zero );
  private static readonly Vector3 v3Center = new Vector3( 0.5f, 0.5f, 0.0f );
  public void MoveCamToCenterObject( Transform targetTransform ) {
    Vector3 v3Hit = GetPositionByRay( v3Center );
    Vector3 goPos = new Vector3( targetTransform.position.x, 0, targetTransform.position.z );
    Vector3 v3Delta = goPos - v3Hit;
    var camPos = Camera.main.transform.position;
    transform.position = new Vector3( camPos.x + v3Delta.x, camPos.y, camPos.z + v3Delta.z );
  }

  public Vector3 GetPositionByRay( Vector3 viewPort ) {
    Ray ray = Camera.main.ViewportPointToRay( viewPort );
    float fDist;
    plane.Raycast( ray, out fDist );
    Vector3 v3Hit = ray.GetPoint( fDist );
    return v3Hit;
  }

}
