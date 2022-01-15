using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class PathDef : MonoBehaviour {
  public List<Vector3> nodes;

  public Vector3 StartDbg;
  public Vector3 EndDbg;
  public bool DebugPath;
  public bool Triangulate;
}


[CustomEditor(typeof(PathDef))]
public class PathDefEditor : Editor {
  readonly GUIStyle sb = new GUIStyle(), sw = new GUIStyle();
  Vector3 mtl = Vector3.left * .19f + Vector3.up * .15f;
  Vector3 mr = Vector3.right * .6f + Vector3.up * .015f;
  Vector3 off = Vector3.right * .01f + Vector3.down * .005f;

  public void OnSceneGUI() {
    PathDef pd = target as PathDef;
    List<Vector3> l = pd.nodes;
    if (l == null || l.Count < 3) {
      Handles.Label(pd.transform.position, "Add at least 3 points");
      return;
    }
    Vector3 pos;

    sb.normal.textColor = Color.black;
    sw.normal.textColor = Color.white;


    Handles.color = Color.red;
    for (int i = 1; i < l.Count; i++) {
      Handles.DrawLine(l[i - 1], l[i], 2);
      Handles.Label(l[i] + off, i.ToString(), sw);
      Handles.Label(l[i], i.ToString(), sb);
      EditorGUI.BeginChangeCheck();
      pos = Handles.PositionHandle(l[i], Quaternion.identity);
      if (EditorGUI.EndChangeCheck()) l[i] = pos;
    }
    Handles.DrawLine(l[l.Count - 1], l[0], 2);
    Handles.Label(l[0] + off, "0", sw);
    Handles.Label(l[0], "0", sb);
    EditorGUI.BeginChangeCheck();
    pos = Handles.PositionHandle(l[0], Quaternion.identity);
    if (EditorGUI.EndChangeCheck()) l[0] = pos;

    if (pd.Triangulate) {




    }


    if (!pd.DebugPath) return;


    Handles.Label(pd.StartDbg + off + mtl, "Start", sw);
    Handles.Label(pd.StartDbg + mtl, "Start", sb);
    EditorGUI.BeginChangeCheck();
    pos = Handles.FreeMoveHandle(pd.StartDbg, Quaternion.identity, .25f, Vector3.one, Handles.CircleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      pd.StartDbg = pos;
      res = null;
    }

    Handles.Label(pd.EndDbg + off + mtl, "End", sw);
    Handles.Label(pd.EndDbg + mtl, "End", sb);
    EditorGUI.BeginChangeCheck();
    pos = Handles.FreeMoveHandle(pd.EndDbg, Quaternion.identity, .25f, Vector3.one, Handles.CircleHandleCap);
    if (EditorGUI.EndChangeCheck()) {
      pd.EndDbg = pos;
      res = null;
    }

    Handles.Label(pd.StartDbg + off + mr + mtl, "Test", sw);
    Handles.Label(pd.StartDbg + mr + mtl, "Test", sb);
    if (Handles.Button(pd.StartDbg + mr, Quaternion.identity, .25f, .25f, Handles.RectangleHandleCap)) TestPathFind(pd);

    if (res != null) {
      Handles.color = Color.blue;
      for (int i = 1; i < res.Count; i++) {
        Handles.DrawLine(res[i - 1], res[i], 3.5f);
      }
    }

  }

  List<Vector3> res = null;
  void TestPathFind(PathDef pd) {
    List<Vector3> ns = pd.nodes;
    // If we can go directly do it
    if (!CheckIntersect(pd.StartDbg, pd.EndDbg, ns)) {
      res = new List<Vector3>() { pd.StartDbg, pd.EndDbg };
      return;
    }

    // Pick the closest spot to start and end
    float mins = float.MaxValue;
    float mine = float.MaxValue;
    int sp = -1;
    int ep = -1;
    for (int i = 0; i < ns.Count; i++) {
      float d = Vector2.Distance(pd.StartDbg, ns[i]);
      if (d < mins) {
        mins = d;
        sp = i;
      }
      d = Vector2.Distance(pd.EndDbg, ns[i]);
      if (d < mine) {
        mine = d;
        ep = i;
      }
    }

    // Calculate the path on the borders
    res = new List<Vector3>() { pd.StartDbg };
    if (sp < ep) {
      if (ep - sp < ns.Count - (ep - sp)) { // internal, forward
        for (int i = sp; i <= ep; i++) { res.Add(ns[i]); Vector3 dbg = res[res.Count - 1]; dbg.z = i; res[res.Count - 1] = dbg; }
      } else { // crossing, back
        for (int i = sp; i >= 0; i--) { res.Add(ns[i]); Vector3 dbg = res[res.Count - 1]; dbg.z = i; res[res.Count - 1] = dbg; }
        for (int i = ns.Count - 1; i >= ep; i--) { res.Add(ns[i]); Vector3 dbg = res[res.Count - 1]; dbg.z = i; res[res.Count - 1] = dbg; }
      }
    } else {
      if (sp - ep < ns.Count - (sp - ep)) { // internal, back
        for (int i = sp; i >= ep; i--) { res.Add(ns[i]); Vector3 dbg = res[res.Count - 1]; dbg.z = i; res[res.Count - 1] = dbg; }
      } else { // crossing, forw
        for (int i = sp; i < ns.Count; i++) { res.Add(ns[i]); Vector3 dbg = res[res.Count - 1]; dbg.z = i; res[res.Count - 1] = dbg; }
        for (int i = 0; i <= ep; i++) { res.Add(ns[i]); Vector3 dbg = res[res.Count - 1]; dbg.z = i; res[res.Count - 1] = dbg; }
      }
    }
    res.Add(pd.EndDbg);

    // Reduce all nodes that will not create intersections
    bool oneRemoved = true;
    List<Vector3> rw = new List<Vector3>();
    while (oneRemoved) {
      oneRemoved = false;
      rw.Clear();
      rw.Add(pd.StartDbg);
      for (int i = 1; i < res.Count - 1; i++) {
        // Can we go from this point to the end?
        if (!CheckIntersect(res[i], pd.EndDbg, ns) && IsInside(res[i], pd.EndDbg, ns)) {
          // We are done, with this point we complete
          rw.Add(res[i]);
          rw.Add(pd.EndDbg);
          res = rw;
          return;
        }

        // Check if we can remove it
        Vector3 pointA = res[i - 1];
        Vector3 pointB = res[i + 1];
        if (!CheckIntersect(pointA, pointB, ns) && IsInside(pointA, pointB, ns)) {
          for (int j = i + 1; j < res.Count; j++)
            rw.Add(res[j]);
          oneRemoved = true;
          break;
        }
        rw.Add(res[i]); // This node is important, add it
      }
      res.Clear();
      res.AddRange(rw);
    }
    res.Add(pd.EndDbg); // This node will never be re-added in the cycle


    // Now, check if we have a closest point for each of the middle points that is not the closest of the edges, in case add the point and re-do the cleaning



    for (int i = 0; i < res.Count; i++) {
      Vector3 dbg = res[i];
      dbg.z = 0;
      res[i] = dbg;
    }
  }


  bool CheckIntersect(Vector3 s, Vector3 e, List<Vector3> nodes) {
    // Check if we intersect something
    for (int i = 0; i < nodes.Count; i++) {
      Vector3 b0 = nodes[i];
      Vector3 b1 = nodes[i == nodes.Count - 1 ? 0 : i + 1];
      if (Intersect(s, e, b0, b1)) return true;
    }
    return false;
  }

  bool Intersect(Vector2 a0, Vector2 a1, Vector2 a2, Vector2 a3) {
    if (a0 == a2 || a0 == a3 || a1 == a2 || a1 == a3) return false;

    float p0 = (a3.y - a2.y) * (a3.x - a0.x) - (a3.x - a2.x) * (a3.y - a0.y);
    float p1 = (a3.y - a2.y) * (a3.x - a1.x) - (a3.x - a2.x) * (a3.y - a1.y);
    float p2 = (a1.y - a0.y) * (a1.x - a2.x) - (a1.x - a0.x) * (a1.y - a2.y);
    float p3 = (a1.y - a0.y) * (a1.x - a3.x) - (a1.x - a0.x) * (a1.y - a3.y);
    return p0 * p1 <= 0 && p2 * p3 <= 0;
  }

  bool IsInside(Vector3 s, Vector3 e, List<Vector3> nodes) {
    Vector3 mid = (s + e) * .5f;
    Vector3 inf = mid + Vector3.up * 10000; // To infinity and beyond
    int num = 0;
    for (int i = 0; i < nodes.Count; i++) {
      Vector3 b0 = nodes[i];
      Vector3 b1 = nodes[i == nodes.Count - 1 ? 0 : i + 1];
      if (Intersect(mid, inf, b0, b1)) num++;
    }
    return num % 2 == 1;
  }

    /*
    Set the start point, try if we have any intersection to the end line, if not close the path
    If yes, find the edge we are intersectiong and pick the vertex closest to the end, then continue



    */


  }