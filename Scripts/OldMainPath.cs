using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OldMainPath : MonoBehaviour {
  public bool ShowPaths = false;
  public List<OldPath> paths;
  public Vector2 start, end;
  internal List<Vector2> gizmoLines = null;

  public List<Vector2> PathFind(Vector2 start, Vector2 end) {
    OldPath pstart = FindPathFromPoint(start);
    OldPath pend = FindPathFromPoint(end);
    if (pstart==null || pend==null) {
      Debug.LogWarning("Path not inside nodes!");
      return null;
    }

    List<Vector2> res = new List<Vector2>();
    List<OldPath> openSet = new List<OldPath> { pstart };
    pstart.prev = null;
    pend.prev = null;
    foreach(OldPath n in paths) {
      n.g = float.PositiveInfinity;
      n.h = float.PositiveInfinity;
      n.prev = null;
    }
    pstart.g = 0;
    pstart.h = 0;

    bool found = false;
    while (openSet.Count > 0) {
      OldPath q = GetMinPath(openSet);
      if (q == pend) {
        found = true;
        break;
      }
      openSet.Remove(q);

      if (q.left != null) {
        // d(current,neighbor) is the weight of the edge from current to neighbor
        // tentative_gScore is the distance from start to the neighbor through current

        q.left.h = Vector2.Distance(q.left.Center(), pend.Center());
        float tentativeScore = q.g + q.left.h;
        if (tentativeScore < q.left.g) {
          // This path to neighbor is better than any previous one. Record it!
          q.left.prev = q;
          q.left.g = tentativeScore;
          if (!openSet.Contains(q.left)) openSet.Add(q.left);
        }
      }
      if (q.right != null) {
        q.right.h = Vector2.Distance(q.right.Center(), pend.Center());
        float tentativeScore = q.g + q.right.h;
        if (tentativeScore < q.right.g) {
          // This path to neighbor is better than any previous one. Record it!
          q.right.prev = q;
          q.right.g = tentativeScore;
          if (!openSet.Contains(q.right)) openSet.Add(q.right);
        }
      }
      if (q.up != null) {
        q.up.h = Vector2.Distance(q.up.Center(), pend.Center());
        float tentativeScore = q.g + q.up.h;
        if (tentativeScore < q.up.g) {
          // This path to neighbor is better than any previous one. Record it!
          q.up.prev = q;
          q.up.g = tentativeScore;
          if (!openSet.Contains(q.up)) openSet.Add(q.up);
        }
      }
      if (q.down != null) {
        q.down.h = Vector2.Distance(q.down.Center(), pend.Center());
        float tentativeScore = q.g + q.down.h;
        if (tentativeScore < q.down.g) {
          // This path to neighbor is better than any previous one. Record it!
          q.down.prev = q;
          q.down.g = tentativeScore;
          if (!openSet.Contains(q.down)) openSet.Add(q.down);
        }
      }
    }

    if (!found) return null;
    if (pend.prev == null) return null;

    // reconstruct the path and Create the result of vector2s
    // Minimize the paths

    res.Add(end);
    OldPath p1 = pend;
    OldPath p2 = pend.prev;
    while (p1 != null && p2 != null) {
      Vector2 c1 = p1 == pend ? end : (p1 == pstart ? start : p1.Center());
      Vector2 c2 = p2 == pend ? end : (p2 == pstart ? start : p2.Center());

      // Check if the line cross the edge or not
      Vector2 intersection = FindIntersection(c1, c2, GetEdge(p1, p2, true), GetEdge(p1, p2, false));
      if (intersection.x != float.NaN)
        res.Add(intersection);

      p1 = p2;
      p2 = p2.prev;

      if (p2 == pstart) {
        c1 = p1 == pend ? end : (p1 == pstart ? start : p1.Center());
        c2 = p2 == pend ? end : (p2 == pstart ? start : p2.Center());

        // Check if the line cross the edge or not
        intersection = FindIntersection(c1, c2, GetEdge(p1, p2, true), GetEdge(p1, p2, false));
        if (intersection.x != float.NaN)
          res.Add(intersection);

        res.Add(start);
        break;
      }
    }

    // We need to check if the "line" from prev to current will pass on the merged edge. If not we need to add a point on the edge
    res.Reverse();
    return res;
  }


  Vector2 GetEdge(OldPath p1, OldPath p2, bool first) {
    if (p1.left == p2) {
      if (first) return p1.tl;
      return p1.bl;
    }
    if (p1.right == p2) {
      if (first) return p1.tr;
      return p1.br;
    }
    if (p1.up == p2) {
      if (first) return p1.tl;
      return p1.tr;
    }
    if (p1.down == p2) {
      if (first) return p1.bl;
      return p1.br;
    }
    return p1.tl;
  }

  OldPath GetMinPath(List<OldPath> list) {
    float min = float.MaxValue;
    OldPath res = null;
    foreach(OldPath p in list)
      if (p.g + p.h < min) {
        res = p;
        min = p.g + p.h;
      }
    return res;
  }

  // Find the point of intersection between the lines p1 --> p2 and p3 --> p4.
  private Vector2 FindIntersection(Vector2 p1s, Vector2 p1e, Vector2 p2s, Vector2 p2e) {
    // Get the segments' parameters.
    float dx12 = p1e.x - p1s.x;
    float dy12 = p1e.y - p1s.y;
    float dx34 = p2e.x - p2s.x;
    float dy34 = p2e.y - p2s.y;

    // Solve for t1 and t2
    float denominator = (dy12 * dx34 - dx12 * dy34);

    float t1 = ((p1s.x - p2s.x) * dy34 + (p2s.y - p1s.y) * dx34) / denominator;
    if (float.IsInfinity(t1)) {
      // The lines are parallel (or close enough to it).
      return new Vector2(float.NaN, float.NaN);
    }

    float t2 = ((p2s.x - p1s.x) * dy12 + (p1s.y - p2s.y) * dx12) / -denominator;

    bool segments_intersect = ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));
    if (segments_intersect)
      return new Vector2(p1s.x + dx12 * t1, p1s.y + dy12 * t1); // Find the point of intersection.

    // Find the closest points on the segments.
    if (t2 < 0) {
      t2 = 0;
    }
    else if (t2 > 1) {
      t2 = 1;
    }
    return new Vector2(p2s.x + dx34 * t2, p2s.y + dy34 * t2);
  }


  private OldPath FindPathFromPoint(Vector2 point) {
    return null;
  }

}


[CustomEditor(typeof(OldMainPath))]
public class MainPathEditor : Editor {

  public override void OnInspectorGUI() {
    DrawDefaultInspector();

    OldMainPath mp = target as OldMainPath;
    if (GUILayout.Button("Set paths")) {
      mp.paths = new List<OldPath>();
      foreach (Transform sib in mp.transform) {
        OldPath p = sib.GetComponent<OldPath>();
        if (p != null) {
          mp.paths.Add(p);
          p.Set();
        }
      }
    }
    if (GUILayout.Button("Check A*")) {
      mp.gizmoLines = mp.PathFind(mp.start, mp.end);
      if (mp.gizmoLines == null) {
        Debug.LogWarning("No path available");
        return;
      }
      string dbg = "";
      foreach (Vector2 v in mp.gizmoLines)
        dbg += v.ToString() + " ";
      Debug.Log(dbg);
    }
    if (GUILayout.Button("Clean A*")) {
      mp.ShowPaths = true;
      mp.gizmoLines.Clear();
    }
  }
  void OnSceneGUI() {
    OldMainPath mp = target as OldMainPath;
    Handles.color = Color.cyan;
    float size = HandleUtility.GetHandleSize(mp.start) * 0.25f;
    Vector3 snap = Vector3.one * 0.5f;

    Vector2 tp = mp.transform.position;
    EditorGUI.BeginChangeCheck();
    Vector2 np = Handles.FreeMoveHandle(tp + mp.start, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(mp, "ppp");
      mp.start = np - tp;
    }
    EditorGUI.BeginChangeCheck();
    np = Handles.FreeMoveHandle(tp + mp.end, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(mp, "ppp");
      mp.end = np - tp;
    }
  }
}

