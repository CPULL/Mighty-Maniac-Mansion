using UnityEngine;

public class Room : MonoBehaviour {
  public string ID;
  public string RoomName;

  public float minL;
  public float maxR;
  public float minY;
  public float maxY;
  public float scalePerc = -.05f;
  public float CameraGround;

  internal PathNode GetPathNode(Vector3 position) {
    NavPath nav = transform.GetComponentInChildren<NavPath>();
    if (nav == null) {
      Debug.Log("Fucking hell!");
      return null;
    }
    float minDist = float.MaxValue;
    PathNode closest = null;
    foreach(PathNode pn in nav.nodes) {
      float dist = Vector2.Distance(position, pn.Center());
      if (dist<minDist) {
        minDist = dist;
        closest = pn;
      }
    }
    return closest;
  }
}
