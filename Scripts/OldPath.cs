using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OldPath : MonoBehaviour {
  public float minY;
  public float maxY;
  public bool isStair;
  public Vector2 tl = new Vector2(0, 1);
  public Vector2 tr = new Vector2(1, 1);
  public Vector2 bl = new Vector2(0, 0);
  public Vector2 br = new Vector2(1, 0);
  public OldPath left;
  public OldPath right;
  public OldPath up;
  public OldPath down;
  public float g;
  public float h;
  public OldPath prev;

  Mesh mesh;
  public OldMainPath parent;
  public bool ShowPaths = false;

  void Start() {
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    if (sr != null) sr.color = new Color32(0, 0, 0, 0);
    ShowPaths = false;
  }

  internal bool FixParent() {
    if (parent == null) parent = transform.parent.GetComponent<OldMainPath>();

    if (parent == null) {
      Debug.LogError("Missing main parent! " + gameObject.name);
      return true;
    }

    return false;
  }


#if UNITY_EDITOR
  void OnDrawGizmos() {
    FixParent();
    ShowPaths = parent.ShowPaths;

    if (!ShowPaths) return;
    Vector2 pos = transform.position;
    Gizmos.DrawIcon(pos + bl, "SelectorBL.png", false);
    Gizmos.DrawIcon(pos + br, "SelectorBR.png", false);
    Gizmos.DrawIcon(pos + tl, "SelectorTL.png", false);
    Gizmos.DrawIcon(pos + tr, "SelectorTR.png", false);
  }

  void OnDrawGizmosSelected() {
    if (!ShowPaths) return;

    Vector3 posbl = new Vector3(transform.position.x + bl.x, transform.position.y + bl.y, 1);
    Vector3 posbr = new Vector3(transform.position.x + br.x, transform.position.y + br.y, 1);
    Vector3 postl = new Vector3(transform.position.x + tl.x, transform.position.y + tl.y, 1);
    Vector3 postr = new Vector3(transform.position.x + tr.x, transform.position.y + tr.y, 1);

    mesh = new Mesh {
      vertices = new Vector3[] { posbl, posbr, postl, postr },
      triangles = new int[] { 0, 2, 1, 2, 3, 1 },
      normals = new Vector3[4] { -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward },
      uv = new Vector2[4] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) }
    };
    mesh.RecalculateNormals();
    mesh.RecalculateBounds();

    Gizmos.color = IsConvex() ? Color.yellow : Color.red;
    Gizmos.DrawMesh(mesh);
  }

  internal void CalculateH(OldPath other) {
    h = Vector2.Distance(Center(), other.Center());
  }

  internal Vector2 Center() {
    return new Vector2((tl.x + tr.x + bl.x + br.x) / 4, (tl.y + tr.y + bl.y + br.y) / 4);
  }


  public bool IsConvex() {
    bool got_negative = false;
    bool got_positive = false;
    int num_points = 4;
    Vector2[] Points = new Vector2[] { tl, tr, br, bl };
    int B, C;
    for (int A = 0; A < num_points; A++) {
      B = (A + 1) % num_points;
      C = (B + 1) % num_points;

      float cross_product =
          CrossProductLength(
              Points[A].x, Points[A].y,
              Points[B].x, Points[B].y,
              Points[C].x, Points[C].y);
      if (cross_product < 0) {
        got_negative = true;
      }
      else if (cross_product > 0) {
        got_positive = true;
      }
      if (got_negative && got_positive) return false;
    }

    return true; // If we got this far, the polygon is convex.
  }

  private static float CrossProductLength(float Ax, float Ay, float Bx, float By, float Cx, float Cy) {
    // Get the vectors' coordinates.
    float BAx = Ax - Bx;
    float BAy = Ay - By;
    float BCx = Cx - Bx;
    float BCy = Cy - By;

    // Calculate the Z coordinate of the cross product.
    return (BAx * BCy - BAy * BCx);
  }

  internal void Set() {
    PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
    if (poly == null) {
      Debug.LogError("Missing Polyugon Collider!!!!");
      return;
    }

    poly.points = new Vector2[] { tl, tr, br, bl };

    left = null;
    right = null;
    up = null;
    down = null;

    // Find the siblings. They need to have both edges snapped
    foreach (Transform sib in parent.transform) {
      if (sib == transform) continue;
      OldPath p = sib.GetComponent<OldPath>();

      if (tl == p.tr && bl == p.br) left = p;
      if (tr == p.tl && br == p.bl) right = p;

      if (tr == p.br && tl == p.bl) up = p;
      if (br == p.tr && bl == p.tl) down = p;
    }
  }
#endif
}

[CustomEditor(typeof(OldPath))]
public class PathEditor : Editor {
  void OnSceneGUI() {
    OldPath t = target as OldPath;
    if (!t.ShowPaths) return;

    Handles.color = Color.red;
    float size = HandleUtility.GetHandleSize(t.tl) * 0.25f;
    Vector3 snap = Vector3.one * 0.5f;
    if (t.FixParent()) return;

    Vector2 tp = t.transform.position;
    EditorGUI.BeginChangeCheck();
    Vector2 np = Handles.FreeMoveHandle(tp + t.tl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "TL");
      t.tl = np - tp;
      // Try to snap
      foreach (Transform sib in t.parent.transform) {
        if (sib == t.transform) continue;
        OldPath p = sib.GetComponent<OldPath>();
        Vector2 edge = p.tr + tp;
        if (np != edge && Vector2.Distance(np, edge) < .05f) t.tl = edge - tp;
        else {
          edge = p.bl + tp;
          if (np != edge && Vector2.Distance(np, edge) < .05f) t.tl = edge - tp;
        }
      }
    }
    EditorGUI.BeginChangeCheck();
    np = Handles.FreeMoveHandle(tp + t.tr, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "TR");
      t.tr = np - tp;
      // Try to snap
      foreach (Transform sib in t.parent.transform) {
        if (sib == t.transform) continue;
        OldPath p = sib.GetComponent<OldPath>();
        Vector2 edge = p.tl + tp;
        if (np != edge && Vector2.Distance(np, edge) < .05f) t.tr = edge - tp;
        else {
          edge = p.br + tp;
          if (np != edge && Vector2.Distance(np, edge) < .05f) t.tr = edge - tp;
        }
      }
    }
    EditorGUI.BeginChangeCheck();
    np = Handles.FreeMoveHandle(tp + t.bl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "BL");
      t.bl = np - tp;
      // Try to snap
      foreach (Transform sib in t.parent.transform) {
        if (sib == t.transform) continue;
        OldPath p = sib.GetComponent<OldPath>();
        Vector2 edge = p.br + tp;
        if (np != edge && Vector2.Distance(np, edge) < .05f) t.bl = edge - tp;
        else {
          edge = p.tl + tp;
          if (np != edge && Vector2.Distance(np, edge) < .05f) t.bl = edge - tp;
        }
      }
    }
    EditorGUI.BeginChangeCheck();
    np = Handles.FreeMoveHandle(tp + t.br, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "BR");
      t.br = np - tp;
      // Try to snap
      foreach (Transform sib in t.parent.transform) {
        if (sib == t.transform) continue;
        OldPath p = sib.GetComponent<OldPath>();
        Vector2 edge = p.bl + tp;
        if (np != edge && Vector2.Distance(np, edge) < .05f) t.br = edge - tp;
        else {
          edge = p.tr + tp;
          if (np != edge && Vector2.Distance(np, edge) < .05f) t.br = edge - tp;
        }
      }
    }
  }

  public override void OnInspectorGUI() {
    DrawDefaultInspector();

    if (GUILayout.Button("Set")) {
      OldPath t = target as OldPath;
      if (t.FixParent()) return;

      // Find the siblings. They need to have both edges snapped
      t.parent.paths = new List<OldPath>();
      foreach (Transform sib in t.parent.transform) {
        OldPath p = sib.GetComponent<OldPath>();
        if (p != null) t.parent.paths.Add(p);
      }

      t.Set();
    }
  }
}

