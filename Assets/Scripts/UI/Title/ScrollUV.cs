using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUV : MonoBehaviour {

  public float parralax = 2f;
  public int direction = 1;

	void Update () {
    MeshRenderer mr = GetComponent<MeshRenderer>();

    Material mat = mr.material;

    Vector2 offset = mat.mainTextureOffset;
    if (direction == 1)
      offset.x += Time.deltaTime / parralax;
    else if (direction == 2)
      offset.y += Time.deltaTime / parralax;
    else if (direction == 3) {
      offset.y += Time.deltaTime / parralax;
      offset.x += Time.deltaTime / parralax;
    }
    mat.mainTextureOffset = offset;
  }
}
