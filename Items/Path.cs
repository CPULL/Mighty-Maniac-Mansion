using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Path : MonoBehaviour {
  public float minY;
  public float maxY;
  public bool isStair;
  public Vector2 tl = new Vector2(0, 1);
  public Vector2 tr = new Vector2(1, 1);
  public Vector2 bl = new Vector2(0, 0);
  public Vector2 br = new Vector2(1, 0);
  public Path left;
  public Path right;
  public Path up;
  public Path down;
  public float g;
  public float h;
  public Path prev;

  Mesh mesh;
  public MainPath parent;
  public bool ShowPaths = false;

  void Start() {
    GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
  }

  internal bool FixParent() {
    if (parent == null) parent = transform.parent.GetComponent<MainPath>();

    if (parent == null) {
      Debug.LogError("Missing main parent!");
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

  internal void CalculateH(Path other) {
    h = Vector2.Distance(Center(), other.Center());
  }

  internal Vector2 Center() {
    return new Vector2((tl.x + tr.x + bl.x + br.x) / 4, (tl.y + tr.y + bl.y + br.y) / 4);
  }

  void OnDrawGizmosSelected() {
    if (!ShowPaths) return;

    Vector2 pos = transform.position;
    mesh = new Mesh {
      vertices = new Vector3[] { pos + bl, pos + br, pos + tl, pos + tr },
      triangles = new int[] { 0, 2, 1, 2, 3, 1 },
      normals = new Vector3[4] { -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward },
      uv = new Vector2[4] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) }
    };
    mesh.RecalculateNormals();
    mesh.RecalculateBounds();

    Gizmos.color = IsConvex() ? Color.yellow : Color.red;
    Gizmos.DrawMesh(mesh);
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

  private static float CrossProductLength(float Ax, float Ay,
    float Bx, float By, float Cx, float Cy) {
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
      Path p = sib.GetComponent<Path>();

      if (tl == p.tr && bl == p.br) left = p;
      if (tr == p.tl && br == p.bl) right = p;

      if (tr == p.br && tl == p.bl) up = p;
      if (br == p.tr && bl == p.tl) down = p;
    }
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
    if (t.FixParent()) return;

    EditorGUI.BeginChangeCheck();
    Vector2 np = Handles.FreeMoveHandle(t.tl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "TL");
      t.tl = np;
      // Try to snap
      foreach (Transform sib in t.parent.transform) {
        if (sib == t.transform) continue;
        Path p = sib.GetComponent<Path>();
        Vector2 edge = p.tr;
        if (np != edge && Vector2.Distance(np, edge) < .05f) {
          t.tl = edge;
        }
      }
    }
    EditorGUI.BeginChangeCheck();
    np = Handles.FreeMoveHandle(t.tr, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "TR");
      t.tr = np;
      // Try to snap
      foreach (Transform sib in t.parent.transform) {
        if (sib == t.transform) continue;
        Path p = sib.GetComponent<Path>();
        Vector2 edge = p.tl;
        if (np != edge && Vector2.Distance(np, edge) < .05f) {
          t.tr = edge;
        }
      }
    }
    EditorGUI.BeginChangeCheck();
    np = Handles.FreeMoveHandle(t.bl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "BL");
      t.bl = np;
      // Try to snap
      foreach (Transform sib in t.parent.transform) {
        if (sib == t.transform) continue;
        Vector2 edge = sib.GetComponent<Path>().br;
        if (np != edge && Vector2.Distance(np, edge) < .05f) {
          t.bl = edge;
        }
      }
    }
    EditorGUI.BeginChangeCheck();
    np = Handles.FreeMoveHandle(t.br, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "BR");
      t.br = np;
      // Try to snap
      foreach (Transform sib in t.parent.transform) {
        if (sib == t.transform) continue;
        Vector2 edge = sib.GetComponent<Path>().bl;
        if (np != edge && Vector2.Distance(np, edge) < .05f) {
          t.br = edge;
        }
      }
    }
  }

  public override void OnInspectorGUI() {
    DrawDefaultInspector();

    if (GUILayout.Button("Set")) {
      Path t = target as Path;
      if (t.FixParent()) return;

      // Find the siblings. They need to have both edges snapped
      t.parent.paths = new List<Path>();
      foreach (Transform sib in t.parent.transform) {
        Path p = sib.GetComponent<Path>();
        if (p != null) t.parent.paths.Add(p);
      }

      t.Set();
    }
  }
}

