using UnityEditor;

public class MyMenuItem {

  [MenuItem( "GameObject/MyMenu/Copy Path", false, -100 )]
  private static void CopyPath() {
    var go = Selection.activeGameObject;

    if (go == null) {
      return;
    }

    var path = go.name;

    while (go.transform.parent != null) {
      go = go.transform.parent.gameObject;
      //path = string.Format( "/{0}/{1}", go.name, path );
      path = $"{go.name}/{path}";
    }

    EditorGUIUtility.systemCopyBuffer = path;
  }

}