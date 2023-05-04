using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileShooter : MonoBehaviour {

  Transform a1, a2, a3, a4, a1_left, a1_up, a2_right, a2_up, a3_left, a3_down, a4_right, a4_down;

  private GameObject missilePf;

  // Use this for initialization
  void Start () {

  }

  void Awake() {
    a1 = transform.Find( "A1" );
    a2 = transform.Find( "A2" );
    a3 = transform.Find( "A3" );
    a4 = transform.Find( "A4" );
    a1_left = transform.Find( "A1_Left" );
    a1_up = transform.Find( "A1_Up" );
    a2_right = transform.Find( "A2_Right" );
    a2_up = transform.Find( "A2_Up" );
    a3_left = transform.Find( "A3_Left" );
    a3_down = transform.Find( "A3_Down" );
    a4_right = transform.Find( "A4_Right" );
    a4_down = transform.Find( "A4_Down" );
    //missilePf = Resources.Load( "Battle/Weapons/Missile" ) as GameObject;
  }

  // Update is called once per frame
  void Update () {
		
	}

  private List<Transform> missileTfList;
  private int missileCount;
  private Transform target;
  public int speed = 10;
  private bool trail;
  private bool IsCollider;
  private bool Show;
  private string sfx, trailName;
  private float trailTime, width;

  public void playMissileFrom( Transform target, int loop, int row = 1, int speed = 10, bool trail = false, bool isCollider = true, bool show = true, 
    string missileSfx = "Missile", float trailTime = 1, float width = 0.2f, string customPf = "Battle/Weapons/Missile", string trailName = "BeamTrail" ) {
    missileTfList = new List<Transform>();
    this.target = target;
    this.speed = speed;
    this.trail = trail;
    IsCollider = isCollider;
    Show = show;
    sfx = missileSfx;
    this.trailName = trailName;
    this.trailTime = trailTime;
    this.width = width;

    for (int i = 0; i<loop; i++) {
      if (row == 0) {
        missileTfList.Add( a3 );
      }
      else if (row == 1) {
        missileTfList.Add( a1 );
        missileTfList.Add( a2 );
      }
      else if (row == 2) {
        missileTfList.Add( a1 );
        missileTfList.Add( a2 );
        missileTfList.Add( a3 );
        missileTfList.Add( a4 );
      }
      else {
        missileTfList.Add( a1 );
        missileTfList.Add( a2 );
        missileTfList.Add( a3 );
        missileTfList.Add( a4 );
        missileTfList.Add( a1_left );
        missileTfList.Add( a2_right );
        missileTfList.Add( a3_left );
        missileTfList.Add( a4_right );
        missileTfList.Add( a1_up );
        missileTfList.Add( a2_up );
        missileTfList.Add( a3_down );
        missileTfList.Add( a4_down );
      }
    }

    missileCount = 0;
    //this.missileCallback = callback;
    missilePf = Resources.Load( customPf ) as GameObject;
    playMissileFrom();
  }

  protected void playMissileFrom() {
    if (missileCount >= missileTfList.Count) {
      //missileCallback();
      return;
    }

    var missTf = missileTfList[missileCount++];
    CoroutineCommon.CallWaitForSeconds( .2f, () => {
      var missileGo = Instantiate( missilePf, missTf.transform.position, missTf.transform.localRotation );
      missileGo.transform.SetParent( missTf );
      missileGo.transform.localPosition = new Vector3( 0, 0, 0 );
      //missileGo.transform.localEulerAngles = missilePf.transform.localEulerAngles;
      missileGo.transform.LookAt( target );
      missileGo.GetComponent<BulletShoot>().direction = Vector3.forward;
      missileGo.GetComponent<BulletShoot>().speed = speed;
      missileGo.GetComponent<BulletShoot>().Show = Show;
      missileGo.GetComponent<BulletShoot>().SfxName = sfx;
      missileGo.GetComponent<BulletShoot>().Trail = trail;
      missileGo.GetComponent<BulletShoot>().TrailName = trailName;
      missileGo.GetComponent<BulletShoot>().TrailTime = trailTime;
      missileGo.GetComponent<BulletShoot>().Width = width;

      missileGo.GetComponent<CapsuleCollider>().enabled = IsCollider;
      //CoroutineCommon.CallWaitForFrames( 1, () => missileGo.GetComponent<BulletShoot>().Trail = Trail );
      playMissileFrom();
    } );

  }

}
