using UnityEngine;
using System.Collections;
using System.Linq;

public class AddInvertedMeshCollider : MonoBehaviour {

  //[HideInInspector] public float unitRadius;

  void Start () {
  }
	
	void Update () {	
	}

  public void Setup( /*float moveRange*/ ) {
    //transform.parent.localScale = new Vector3( moveRange, transform.localScale.y, moveRange );
    //transform.position = new Vector3( transform.position.x, 5, transform.position.z );

    RemoveExistingColliders();
    Mesh mesh = GetComponent<MeshFilter>().mesh;
    //mesh.triangles = mesh.triangles.Concat<int>( mesh.triangles.Reverse().ToArray() ).ToArray();
    mesh.triangles = mesh.triangles.Reverse().ToArray();
    gameObject.AddComponent<MeshCollider>();
  } 

  void OnEnable() {
  }

  void OnDisable() {
  }

  private void RemoveExistingColliders() {
    Collider[] colliders = GetComponents<Collider>();
    for (int i = 0; i < colliders.Length; i++)
      Destroy( colliders[i] );
  }

}
