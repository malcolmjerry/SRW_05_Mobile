using System.Collections.Generic;
using UnityEngine;

public class Barrier {

  // 0 表示沒有防護罩  1 Beam coat  2 I-Field  3 FF-Field  4 PS裝甲  5 念動-Field  6 Pinpoint Barrier  7 歪曲-Field  8 Beam吸收  9 Gravity Barrier
  // 10 AT Field  11 Aura Barrier
  public static string GetName( int index ) {
    switch (index) {
      case 1: return "Beam coat";
      case 2: return "I-Field";
      case 3: return "FF-Field";
      case 4: return "PS裝甲";
      case 5: return "念動-Field";
      case 6: return "Pinpoint Barrier";
      case 7: return "歪曲-Field";
      case 8: return "Beam 吸收";
      case 9: return "Gravity Barrier";
      case 10: return "AT Field";
      case 11: return "Aura Barrier";
      default: return "";
    }
  }

}

