using UnityEditor;
using UnityEngine;

public class PathNode : MonoBehaviour {
  public float minY;
  public float maxY;
  public bool isStair;

  public PathNode left, right, top, down;
  public Vector2 tl, tr, br, bl;
  private PolygonCollider2D pcoll;
  private Vector2 roomabs = Vector2.up;
  public NavPath parent;
  public bool showMeshLocal;
  private Color yellowish = new Color32(200, 220, 10, 200);
  public PathNode prev;
  public float g;
  public float h;

  void OnDrawGizmos() {
    if (!showMeshLocal) return;
    if (roomabs == Vector2.up) roomabs = parent.transform.parent.position;

    Mesh mesh = new Mesh {
      vertices = new Vector3[] { bl, br, tl, tr },
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

  public void UpdateEdges(Vector2 utl, Vector2 utr, Vector2 ubr, Vector2 ubl) {
    if (utl != Vector2.zero) tl = utl;
    if (utr != Vector2.zero) tr = utr;
    if (ubl != Vector2.zero) bl = ubl;
    if (ubr != Vector2.zero) br = ubr;
    UpdatePoly();
  }

  public void UpdatePoly() {
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
    pcoll.points = new Vector2[] { tl - roomabs, tr - roomabs, br - roomabs, bl - roomabs };
  }
}

