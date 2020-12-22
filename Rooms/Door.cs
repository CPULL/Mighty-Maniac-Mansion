using UnityEngine;

public class Door : Item {
  public Room src;
  public Room dst;
  public Door correspondingDoor;
  public TransitionDirection transition;
  public Dir arrivalDirection;
  public AudioClip OpenSound;
  public AudioClip CloseSound;
  public AudioClip UnlockSound;
  public AudioClip LockSound;
  public AudioSource Audio;

  public MeshFilter mf;
  public MeshRenderer mr;
  public PolygonCollider2D poly;
  bool isSR = false;
  bool isMR = false;

  private void Start() {
    sr = GetComponent<SpriteRenderer>();
    if (sr == null) {
      mr = GetComponent<MeshRenderer>();
      if (mr == null) return;

      isMR = true;
      mf = GetComponent<MeshFilter>();
      poly = GetComponent<PolygonCollider2D>();
      Mesh m = mf.mesh;
      Vector2 tl = poly.points[0]; // Find the points of the polygon in order: BL, BR, TL, TR
      Vector2 tr = poly.points[1];
      Vector2 bl = poly.points[2];
      Vector2 br = poly.points[3];
      Vector2 swap;
      if (tl.x > tr.x) { swap = tl; tl = tr; tr = swap; }
      if (bl.x > br.x) { swap = bl; bl = br; br = swap; }
      if (tl.y > bl.y) { swap = tl; tl = bl; bl = swap; }
      if (tr.y > br.y) { swap = tr; tr = br; br = swap; }
      if (tl.x > tr.x) { swap = tl; tl = tr; tr = swap; }
      if (bl.x > br.x) { swap = bl; bl = br; br = swap; }
      if (tl.y > bl.y) { swap = tl; tl = bl; bl = swap; }
      if (tr.y > br.y) { swap = tr; tr = br; br = swap; }
      if (tl.x > tr.x) { swap = tl; tl = tr; tr = swap; }
      if (bl.x > br.x) { swap = bl; bl = br; br = swap; }
      if (tl.y > bl.y) { swap = tl; tl = bl; bl = swap; }
      if (tr.y > br.y) { swap = tr; tr = br; br = swap; }

      Vector3[] vertices = new Vector3[4];
      vertices[0] = bl;
      vertices[1] = br;
      vertices[2] = tl;
      vertices[3] = tr;
      m.vertices = vertices;

      int[] tris = new int[6] { 0, 2, 1, 2, 3, 1 };
      m.triangles = tris;

      Vector3[] normals = new Vector3[4] { -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward };
      m.normals = normals;

      Vector2[] uv = new Vector2[4] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };
      m.uv = uv;
      mf.mesh = m;
      mr.material.SetColor("_Color", GD.DoorColor);
    }
    else {
      isSR = true;
      sr.color = normalColor;
    }

    Audio = GetComponent<AudioSource>();
  }

  private void OnMouseEnter() {
    if (Options.IsActive() || Controller.InventoryActive()) return;
    if (isSR) {
      Controller.SetItem(this);
      sr.color = overColor;
    }
    else if (isMR) {
      Controller.SetItem(this);
      mr.material.SetColor("_Color", GD.OverColor);
    }
  }

  private void OnMouseExit() {
    Controller.SetItem(null);
    if (isSR) {
      sr.color = normalColor;
    }
    else if (isMR) {
      mr.material.SetColor("_Color", GD.DoorColor);
    }
  }
}





