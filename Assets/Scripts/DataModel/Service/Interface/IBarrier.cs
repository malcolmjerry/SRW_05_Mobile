using System.Collections.Generic;
using UnityEngine;

public interface IBarrier {
  
  int BarrierID { get; }     // 0 表示沒有防護罩  1 Beam coat  2 I-Field  3 FF-Field  4 PS裝甲  5 念動-Field  6 Pinpoint Barrier  7 歪曲-Field  8 Beam吸收  9 Gravity Barrier
                      // 10 AT Field  11 Aura Barrier
  string Name { get; }

  int EN { get; }

  int Amount { get; }

  bool CanActive( AttackData atkData );

  float Effect( AttackData atkData );

}

