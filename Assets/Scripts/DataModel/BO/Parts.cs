using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityORM;

[Serializable]
public class Parts {

  [Key( AutoIncrement = true )]
  public int ID;

  public string Name { get; set; }

  public string Desc;

  //public int Type;    //1.Barrier  2.Avatar  3. Pre Buff   4. Phase Start  5. Usable  

  //public PartsTypeEnum Type;

  //public enum PartsTypeEnum { Barrier = 1, Avatar, PreBuff, PhaseStart, Usable }

  public bool IsBarrier;
  public bool IsAvatar;
  public bool IsBuffUnit;
  public bool IsPhaseStart;
  public bool IsBattleAttacker;
  public bool IsBattleDefenser;
}

