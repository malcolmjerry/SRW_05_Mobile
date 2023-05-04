using UnityEngine;
using System.Collections;
using System;
using UnityORM;

[Serializable]
public class WeaponInstance {

  public int? SaveSlot;
  public int? SeqNo;

  public int RobotInstanceSaveSlot;
  public int RobotInstanceSeqNo;
  //[Ignore]public RobotInstance RobotInstance { set; get; }

  public int WeaponID;

  [Ignore] //For SQLite
  public Weapon Weapon { set; get; }

  public int Level;
  public int UpgradHitPoint() {
    if (Level == 0)
      return 0;
    int[] improveArr = Weapon.ImproveTypes[Weapon.ImproveType-1];
    int sum = 0;
    for (int i = 0; i < Level; i++) {
      sum += improveArr[i];
    }
    return sum;
  }

  public int Enable;

  //public int Bonus;      //1.最小射程-1 最大射程+1 (不含MAP)  2.全武器可P(不包含MAP)  3.消耗EN-20%  4.彈藥1.5倍  5.全地形適性A  6.命中+40  7.CRI+25  8.解除氣力限制(不含MAP)

  /*
  public WeaponInfo CloneInfo( RobotInfo robotInfo ) {
    return new WeaponInfo( robotInfo ).Init( this );
  }*/
}
