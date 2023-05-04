using UnityEngine;
using System.Collections;
using System;
using UnityORM;
using System.Collections.Generic;

//[Serializable]
public class PilotDialog {

  [Key( AutoIncrement = true )]
  public int ID;

  public int PilotID { get; set; }

  public int? WeaponID { get; set; }            //武器ID

  public int? WeaponNameType { get; set; }      //1 火神炮  2 Funnel

  public string Text { get; set; }

  public bool IsDead { get; set; }         //南無三!

  public bool IsDanger { get; set; }       //竟能把我迫到這種程度...

  public bool IsBigDam { get; set; }     //竟然有這種力量

  public bool IsSmallDam { get; set; }   //哈哈, 就這點程度?

  public bool IsDodge { get; set; }        //太慢了, 看到了

  public bool IsAttack { get; set; }        //打中吧

  public bool IsSupport { get; set; }        //援護來了!

  public bool IsAssist { get; set; }        //我支援你!

  public int OtherPilotID { get; set; }     //阿寶, 還不明白嗎, 人類被地球的重力束縛

  public bool IsMelee { get; set; }         //格鬥!

  public bool IsShoot { get; set; }         //格鬥!

  public bool IsUnable { get; set; }       //無法反擊!
}
