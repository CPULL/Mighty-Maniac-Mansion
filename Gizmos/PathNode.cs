using UnityEditor;
using UnityEngine;

public class PathNode : MonoBehaviour {
  public float minY;
  public float maxY;
  public bool isStair;

  public PathNode left, right, top, down;
  public Vector2 tl, tr, br, bl;
  private PolygonCollider2D pcoll;
  public NavPath parent;
  public bool showMeshLocal;
  private Color yellowish = new Color32(200, 220, 10, 200);
  public PathNode prev;
  public float g;
  public float h;

  void OnDrawGizmos() {
    if (!showMeshLocal) return;

    Vector2 pos = transform.position;
    Mesh mesh = new Mesh {
      vertices = new Vector3[] { pos + bl, pos + br, pos + tl, pos + tr },
      triangles = new int[] { 0, 2, 1, 2, 3, 1 },
      normals = new Vector3[4] { -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward },
      uv = new Vector2[4] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) }
    };
    mesh.RecalculateNormals();
    mesh.RecalculateBounds();

    Gizmos.color = yellowish;
    Gizmos.DrawMesh(mesh);
  }

  public Vector2 Center() {
    return new Vector2((tl.x + tr.x + bl.x + br.x) / 4, (tl.y + tr.y + bl.y + br.y) / 4);
  }

  public void Init(string name) {
    gameObject.name = name;
    pcoll = GetComponent<PolygonCollider2D>();
    left = null;
    right = null;
    top = null;
    down = null;
  }

  internal void UpdateEdges(Vector2 utl, Vector2 utr, Vector2 ubr, Vector2 ubl) {
    if (utl != Vector2.zero) tl = utl;
    if (utr != Vector2.zero) tr = utr;
    if (ubl != Vector2.zero) bl = ubl;
    if (ubr != Vector2.zero) br = ubr;
    UpdatePoly();
  }

  internal void UpdatePoly() {
    if (br.x < bl.x) {
      Vector2 tmp = br;
      br = bl;
      bl = tmp;
    }
    if (tr.x < tl.x) {
      Vector2 tmp = tr;
      tr = tl;
      tl = tmp;
    }

    if (tl.y < bl.y) {
      Vector2 tmp = tl;
      tl = bl;
      bl = tmp;
    }
    if (tr.y < br.y) {
      Vector2 tmp = tr;
      tr = br;
      br = tmp;
    }
    if (pcoll == null) pcoll = GetComponent<PolygonCollider2D>();
    pcoll.points = new Vector2[] { tl, tr, br, bl };
  }
}

[CustomEditor(typeof(PathNode))]
public class PathNodeEditor : Editor {
  PathNode t = null;
  float size = 1;
  Vector3 snap = Vector3.one * 0.5f;
  Vector2 tp;


  void OnEnable() {
    t = target as PathNode;
    size = HandleUtility.GetHandleSize(Vector2.zero) * 0.25f;
    snap = Vector3.one * 0.5f;
    tp = t.transform.position;
  }

  public override void OnInspectorGUI() {
    EditorGUIUtility.labelWidth = 45;
    EditorGUILayout.BeginHorizontal();
    t.minY = EditorGUILayout.FloatField("Min Y", t.minY);
    t.maxY = EditorGUILayout.FloatField("Max Y", t.maxY);
    t.isStair = EditorGUILayout.Toggle("Stairs", t.isStair);
    EditorGUILayout.EndHorizontal();


    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.BeginHorizontal();
    t.tl = EditorGUILayout.Vector2Field("TL", t.tl);
    t.top = (PathNode)EditorGUILayout.ObjectField("Top", t.top, typeof(PathNode), true);
    t.tr = EditorGUILayout.Vector2Field("TR", t.tr);
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    t.left = (PathNode)EditorGUILayout.ObjectField("Left", t.left, typeof(PathNode), true);
    t.parent = (NavPath)EditorGUILayout.ObjectField("Parent", t.parent, typeof(NavPath), true);
    t.right = (PathNode)EditorGUILayout.ObjectField("Right", t.right, typeof(PathNode), true);
    EditorGUILayout.EndHorizontal();

    EditorGUILayout.BeginHorizontal();
    t.bl = EditorGUILayout.Vector2Field("BL", t.bl);
    t.down = (PathNode)EditorGUILayout.ObjectField("Down", t.down, typeof(PathNode), true);
    t.br = EditorGUILayout.Vector2Field("BR", t.br);
    EditorGUILayout.EndHorizontal();

    EditorGUIUtility.labelWidth = 80;
    t.showMeshLocal = EditorGUILayout.Toggle("Show mesh", t.showMeshLocal);
  }

  void OnSceneGUI() {
    Handles.color = Color.red;
    size = HandleUtility.GetHandleSize(Vector2.zero) * 0.25f;
    snap = Vector3.one * 0.5f;

    if (t.left == null && Handles.Button(tp + (t.tl + t.bl) / 2, Quaternion.identity, size * .5f, size * .5f, Handles.CircleHandleCap)) {
      t.left = Instantiate(t.gameObject, t.transform.parent).GetComponent<PathNode>();
      t.left.Init(t.name + "L");
      t.left.tr = t.tl;
      t.left.br = t.bl;
      t.left.tl = t.tl + Vector2.left * 3;
      t.left.bl = t.bl + Vector2.left * 3;
      t.left.right = t;
      if (!t.parent.nodes.Contains(t.left)) t.parent.nodes.Add(t.left);
    }

    if (t.right == null && Handles.Button(tp + (t.tr + t.br) / 2, Quaternion.identity, size * .5f, size * .5f, Handles.CircleHandleCap)) {
      t.right = Instantiate(t.gameObject, t.transform.parent).GetComponent<PathNode>();
      t.right.Init(t.name + "R");
      t.right.tl = t.tr;
      t.right.bl = t.br;
      t.right.tr = t.tr + Vector2.right * 3;
      t.right.br = t.br + Vector2.right * 3;
      t.right.left = t;
      if (!t.parent.nodes.Contains(t.right)) t.parent.nodes.Add(t.right);
    }

    if (t.top == null && Handles.Button(tp + (t.tr + t.tl) / 2, Quaternion.identity, size * .5f, size * .5f, Handles.CircleHandleCap)) {
      t.top = Instantiate(t.gameObject, t.transform.parent).GetComponent<PathNode>();
      t.top.Init(t.name + "T");
      t.top.bl = t.tl;
      t.top.br = t.tr;
      t.top.tl = t.tl + Vector2.up * 2;
      t.top.tr = t.tr + Vector2.up * 2;
      t.top.down = t;
      if (!t.parent.nodes.Contains(t.top)) t.parent.nodes.Add(t.top);
    }

    if (t.down == null && Handles.Button(tp + (t.br + t.bl) / 2, Quaternion.identity, size * .5f, size * .5f, Handles.CircleHandleCap)) {
      t.down = Instantiate(t.gameObject, t.transform.parent).GetComponent<PathNode>();
      t.down.Init(t.name + "D");
      t.down.tl = t.bl;
      t.down.tr = t.br;
      t.down.bl = t.bl + Vector2.down * 2;
      t.down.br = t.br + Vector2.down * 2;
      t.down.top = t;
      if (!t.parent.nodes.Contains(t.down)) t.parent.nodes.Add(t.down);
    }


    EditorGUI.BeginChangeCheck();
    Vector2 wptl = Handles.FreeMoveHandle(tp + t.tl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    Vector2 wptr = Handles.FreeMoveHandle(tp + t.tr, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    Vector2 wpbr = Handles.FreeMoveHandle(tp + t.br, Quaternion.identity, size, snap, Handles.RectangleHandleCap);
    Vector2 wpbl = Handles.FreeMoveHandle(tp + t.bl, Quaternion.identity, size, snap, Handles.RectangleHandleCap);

    if (EditorGUI.EndChangeCheck()) {
      Undo.RecordObject(t, "Bounds");
      t.tl = wptl - tp;
      t.tr = wptr - tp;
      t.br = wpbr - tp;
      t.bl = wpbl - tp;

      // Move siblings
      if (t.left != null) {
        t.left.UpdateEdges(Vector2.zero, t.tl, t.bl, Vector2.zero);
        t.left.tr = t.tl;
        t.left.br = t.bl;
      }
      if (t.right != null) {
        t.right.UpdateEdges(t.tr, Vector2.zero, Vector2.zero, t.br);
        t.right.tl = t.tr;
        t.right.bl = t.br;
      }
      if (t.top != null) {
        t.top.UpdateEdges(Vector2.zero, Vector2.zero, t.tl, t.tr);
        t.top.bl = t.tl;
        t.top.br = t.tr;
      }
      if (t.down != null) {
        t.down.UpdateEdges(t.bl, t.br, Vector2.zero, Vector2.zero);
        t.down.tl = t.bl;
        t.down.tr = t.br;
      }
      t.UpdatePoly();
    }

  }
}