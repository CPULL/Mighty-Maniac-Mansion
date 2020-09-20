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
  public Path left;
  public Path right;
  public Path up;
  public Path down;

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
  MainPath parent;

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
      // Try to snap
      if (parent == null) {
        Transform p = t.transform.parent;
        if (p != null) {
          parent = p.GetComponent<MainPath>();
        }
      }
      if (parent == null) return;
      foreach (Transform sib in parent.transform) {
        if (sib == t.transform) continue;
        Path p = sib.GetComponent<Path>();
        Vector3 edge = p.tr;
        if (np != edge && Vector3.Distance(np, edge) < .05f) {
          Debug.Log("Snap!");
          t.tl = edge;
        }
      }
    }
    np = Handles.FreeMoveHandle(t.tr, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "TR");
      t.tr = np;
      // Try to snap
      if (parent == null) {
        Transform p = t.transform.parent;
        if (p != null) {
          parent = p.GetComponent<MainPath>();
        }
      }
      if (parent == null) return;
      foreach (Transform sib in parent.transform) {
        if (sib == t.transform) continue;
        Path p = sib.GetComponent<Path>();
        Vector3 edge = p.tl;
        if (np != edge && Vector3.Distance(np, edge) < .05f) {
          Debug.Log("Snap!");
          t.tr = edge;
        }
      }
    }
    np = Handles.FreeMoveHandle(t.bl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "BL");
      t.bl = np;
      // Try to snap
      if (parent == null) {
        Transform p = t.transform.parent;
        if (p != null) {
          parent = p.GetComponent<MainPath>();
        }
      }
      if (parent == null) return;
      foreach (Transform sib in parent.transform) {
        if (sib == t.transform) continue;
        Vector3 edge = sib.GetComponent<Path>().br;
        if (np != edge && Vector3.Distance(np, edge) < .05f) {
          Debug.Log("Snap!");
          t.bl = edge;
        }
      }
    }
    np = Handles.FreeMoveHandle(t.br, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "BR");
      t.br = np;
      // Try to snap
      if (parent == null) {
        Transform p = t.transform.parent;
        if (p != null) {
          parent = p.GetComponent<MainPath>();
        }
      }
      if (parent == null) return;
      foreach (Transform sib in parent.transform) {
        if (sib == t.transform) continue;
        Vector3 edge = sib.GetComponent<Path>().bl;
        if (np != edge && Vector3.Distance(np, edge) < .05f) {
          Debug.Log("Snap!");
          t.br = edge;
        }
      }
    }
  }

  public override void OnInspectorGUI() {
    DrawDefaultInspector();
    if (GUILayout.Button("Set collider")) {
      Path t = target as Path;
      PolygonCollider2D poly = t.GetComponent<PolygonCollider2D>();
      if (poly == null) {
        Debug.LogError("Missing Polyugon Collider!!!!");
        return;
      }

      poly.points = new Vector2[] { t.tl, t.tr, t.br, t.bl };
    }

    if (GUILayout.Button("Set siblings")) {
      Path t = target as Path;
      t.left = null;
      t.right = null;
      t.up = null;
      t.down = null;

      if (parent == null) {
        Transform p = t.transform.parent;
        if (p != null) {
          parent = p.GetComponent<MainPath>();
        }
      }
      if (parent == null) {
        Debug.LogError("Missing main parent!");
        return;
      }

      // Find the siblings. They need to have both edges snapped
      foreach (Transform sib in parent.transform) {
        if (sib == t.transform) continue;
        Path p = sib.GetComponent<Path>();
        
        if (t.tl == p.tr && 
            t.bl == p.br) 
          t.left = p;
        if (t.tr == p.tl && 
            t.br == p.bl) 
          t.right = p;

        if (t.tr == p.br && 
            t.tl == p.bl) 
          t.up = p;
        if (t.br == p.tr && 
            t.bl == p.tl) 
          t.down = p;
      }

    }
  }
}

