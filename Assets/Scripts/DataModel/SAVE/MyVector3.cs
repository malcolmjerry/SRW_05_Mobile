using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class MyVector3 {

  public MyVector3( Vector3 vector3 ) {
    x = vector3.x;
    y = vector3.y;
    z = vector3.z;
  }

  float x, y, z;

  public Vector3 ToVector3() {
    return new Vector3( x, y, z );
  }

}

