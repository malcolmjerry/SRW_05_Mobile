using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunShoot : MonoBehaviour {

  public GameObject BulletPrefab;

  Transform a1, a2, a3, a4, a5, a6, a7, a8;

  public bool a1_on = true, a2_on = true, a3_on = true, a4_on = true, a5_on = true, a6_on = true, a7_on = true, a8_on = true;
  public float a1_sound = 1f, a2_sound = 0, a3_sound = 0, a4_sound = 0, a5_sound = 0, a6_sound = 0, a7_sound = 0, a8_sound = 0;

  private float lastTime;
  public float MaxTime = 0.01f;
  public float Speed = 20f;

  // Use this for initialization
  void Start () {
    lastTime = 0;

    a1 = transform.Find( "Renderer/A1" );
    a2 = transform.Find( "Renderer/A2" );
    a3 = transform.Find( "Renderer/A3" );
    a4 = transform.Find( "Renderer/A4" );
    a5 = transform.Find( "Renderer/A5" );
    a6 = transform.Find( "Renderer/A6" );
    a7 = transform.Find( "Renderer/A7" );
    a8 = transform.Find( "Renderer/A8" );

  }

  bool toggle = true;

  // Update is called once per frame
  void Update () {
    lastTime += Time.deltaTime;

    if (lastTime > MaxTime) {
      lastTime = 0;

      //if (true || toggle) {
        if (a1_on) createBulletByParent( a1, a1_sound );
        if (a2_on) createBulletByParent( a2, a2_sound );
        if (a3_on) createBulletByParent( a3, a3_sound ) ;
        if (a4_on) createBulletByParent( a4, a4_sound );
        if (a5_on) createBulletByParent( a5, a5_sound );
        if (a6_on) createBulletByParent( a6, a6_sound );
        if (a7_on) createBulletByParent( a7, a7_sound );
        if (a8_on) createBulletByParent( a8, a8_sound );
      //}                      
      //else {
        /*
        createBulletByParent( a2 );
        createBulletByParent( a4 );
        createBulletByParent( a6 );
        createBulletByParent( a8 );
        */
      //}
      toggle = !toggle;
    }
	}

  public void SetHeadGun() {
    a2_on = a3_on = a4_on = a6_on = a7_on = a8_on = false;
    a1_on = a5_on = true;

    a1_sound = 1f;
    a2_sound = 0;
    a3_sound = 0;
    a4_sound = 0;
    a5_sound = 0;
    a6_sound = 0;
    a7_sound = 0;
    a8_sound = 0;
    Speed = 30f;
  }

  public void SetShipGun() {
    a2_on = a4_on = a6_on = a8_on = false;
    a1_on = a3_on = a5_on = a7_on  = true;

    a1_sound = 1f;
    a2_sound = 0;
    a3_sound = 0;
    a4_sound = 0;
    a5_sound = 0;
    a6_sound = 0;
    a7_sound = 0;
    a8_sound = 0;
    Speed = 30f;
  }

  public void SetSingleGun() {
    a1_on = a3_on = a5_on = a2_on = a4_on = a6_on = a7_on = false;
    a8_on = true;

    a1_sound = 0;
    a2_sound = 0;
    a3_sound = 0;
    a4_sound = 0;
    a5_sound = 0;
    a6_sound = 0;
    a7_sound = 0;
    a8_sound = 1f;
    Speed = 30f;
  }

  public enum GunType { HEAD = 1, SHIP = 2, Single = 3 }
  public void SetGunType( GunType gunType, string BulletPrefabName = null ) {
    if (BulletPrefabName != null)
      BulletPrefab = Resources.Load( $"Battle/Weapons/{BulletPrefabName}" ) as GameObject;

    switch (gunType) {
      case GunType.HEAD:
        SetHeadGun(); break;
      case GunType.SHIP:
        SetShipGun(); break;
      case GunType.Single:
        SetSingleGun();
        break;
      default: break;
    }
  }

  private void createBulletByParent( Transform shootPos, float playSound = .8f ) {
    //var bullet = Instantiate( Bullet, parent );
    var bullet = Instantiate( BulletPrefab );

    bullet.transform.SetParent( shootPos );
    bullet.transform.localPosition = new Vector3( 0, 0, 0 );
    bullet.transform.localEulerAngles = BulletPrefab.transform.localEulerAngles;
	  
    //bullet.transform.localEulerAngles = parent.localEulerAngles;
    //bullet.transform.localRotation = Quaternion.Euler( Bullet.transform.rotation.x, 0, 0 );
    //bullet.transform.localRotation = Quaternion.Euler( 180, 0, 0 );
    //bullet.transform.localScale = new Vector3( bullet.transform.localScale.x / 100f, bullet.transform.localScale.y / 100f, bullet.transform.localScale.z / 100f );
    bullet.GetComponent<BulletShoot>().PlaySound = playSound;
    bullet.GetComponent<BulletShoot>().speed = Speed;
  }

}
