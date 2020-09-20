using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour {
  public float minY;
  public float maxY;
  public bool isStair;
  public Vector3 tl = new Vector3(0, 1);
  public Vector3 tr = new Vector3(1, 1);
  public Vector3 bl = new Vector3(0, 0);
  public Vector3 br = new Vector3(1, 0);

  Mesh mesh;
  MainPath parent;
  public bool ShowPaths = false;

  void Start() {
    GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);

  }


#if UNITY_EDITOR
  void OnDrawGizmos() {
    if (parent == null) parent = transform.parent.GetComponent<MainPath>();
    ShowPaths = parent.ShowPaths;

    if (!ShowPaths) return;
    Gizmos.DrawIcon(bl, "SelectorBL.png", false);
    Gizmos.DrawIcon(br, "SelectorBR.png", false);
    Gizmos.DrawIcon(tl, "SelectorTL.png", false);
    Gizmos.DrawIcon(tr, "SelectorTR.png", false);
  }
  void OnDrawGizmosSelected() {
    if (!ShowPaths) return;

    mesh = new Mesh();
    mesh.vertices = new Vector3[] { bl, br, tl, tr };
    mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
    mesh.normals = new Vector3[4] { -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward };
    mesh.uv = new Vector2[4] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
    mesh.RecalculateNormals();
    mesh.RecalculateBounds();

    Gizmos.color = Color.yellow;
    Gizmos.DrawMesh(mesh);
  }
#endif
}

[CustomEditor(typeof(Path))]
public class PathEditor : Editor {
  void OnSceneGUI() {
    Path t = target as Path;
    if (!t.ShowPaths) return;

    Handles.color = Color.red;
    float size = HandleUtility.GetHandleSize(t.tl) * 0.25f;
    Vector3 snap = Vector3.one * 0.5f;

    EditorGUI.BeginChangeCheck();
    Vector3 np = Handles.FreeMoveHandle(t.tl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "TL");
      t.tl = np;
    }
    np = Handles.FreeMoveHandle(t.tr, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "TR");
      t.tr = np;
    }
    np = Handles.FreeMoveHandle(t.bl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "BL");
      t.bl = np;
    }
    np = Handles.FreeMoveHandle(t.br, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "BR");
      t.br = np;
    }
  }

  public override void OnInspectorGUI() {
    if (GUILayout.Button("Set collider")) {
      Path t = target as Path;
      PolygonCollider2D poly = t.GetComponent<PolygonCollider2D>();
      if (poly == null) {
        Debug.LogError("Missing Polyugon Collider!!!!");
        return;
      }

      poly.points = new Vector2[] { t.tl, t.tr, t.br, t.bl };
    }
  }
}

