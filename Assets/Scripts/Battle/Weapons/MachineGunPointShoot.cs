using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunPointShoot : MonoBehaviour {

  public GameObject BulletPrefab;

  private float lastTime;
  public float MaxTime = 0.01f;

  public bool playSound = true;
  private Vector3 bulletDirection = Vector3.down;

  // Use this for initialization
  void Start () {
    lastTime = 0;

  }

  public void Setup( Vector3? direction = null, string BulletPrefabName = null ) {
    if (direction.HasValue)
      bulletDirection = direction.Value;

    if (BulletPrefabName != null)
      BulletPrefab = Resources.Load( $"Battle/Weapons/{BulletPrefabName}" ) as GameObject;
  }

  // Update is called once per frame
  void Update () {
    lastTime += Time.deltaTime;

    if (lastTime > MaxTime) {
      lastTime = 0;
      createBulletByParent();
    }
  }

  private void createBulletByParent() {
    //var bullet = Instantiate( Bullet, parent );
    var bullet = Instantiate( BulletPrefab );


    bullet.transform.SetParent( transform );
    bullet.transform.localPosition = new Vector3( 0, 0, 0 );
    bullet.transform.localEulerAngles = BulletPrefab.transform.localEulerAngles;

    bullet.GetComponent<BulletShoot>().PlaySound = playSound? 1f : 0;
    bullet.GetComponent<BulletShoot>().speed = 30f;

    bullet.GetComponent<BulletShoot>().direction = bulletDirection;
  }

}
