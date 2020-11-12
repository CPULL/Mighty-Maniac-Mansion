using System.Collections.Generic;
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
  public bool external;
  public LightMode lightsStatus = LightMode.LightsOn;
  List<SpriteRenderer> srs;
  List<PathNode> paths;

  private void Start() {
    srs = new List<SpriteRenderer>();
    CollectAllRenderers(transform);
    CollectAllPaths();
  }

  void CollectAllRenderers(Transform tran) {
    SpriteRenderer sr = tran.GetComponent<SpriteRenderer>();
    if (sr != null) {
      if (sr.material.name.IndexOf("SceneSelectionPoint") == -1)
        srs.Add(sr);
      else
        sr.enabled = false;
    }
    foreach (Transform t in tran)
      CollectAllRenderers(t);
  }

  void CollectAllPaths() {
    NavPath path = transform.GetComponentInChildren<NavPath>();
    if (path == null) {
      Debug.LogError("Missing path for room: " + ID);
      return;
    }
    paths = path.nodes;
  }


  internal PathNode GetPathNode(Vector3 position) {
    if (paths == null) return null;
    float minDist = float.MaxValue;
    PathNode closest = null;
    foreach(PathNode pn in paths) {
      float dist = Vector2.Distance(position, pn.Center());
      if (dist < minDist) {
        minDist = dist;
        closest = pn;
      }
    }
    return closest;
  }

  internal void SetLights(LightMode lightsOn) {
    lightsStatus = lightsOn;
    UpdateLights();
  }

  internal void SwitchLights() {
    if (lightsStatus == LightMode.LightsOn) lightsStatus = LightMode.LightsOff;
    if (lightsStatus == LightMode.LightsOff) lightsStatus = LightMode.LightsOn;
    if (lightsStatus == LightMode.FlashLights) lightsStatus = LightMode.LightsOn;
    UpdateLights();
  }

  internal void UpdateLights() {
    Material m = GD.Normal();
    switch (lightsStatus) {
      case LightMode.LightsOn: m = GD.Normal(); break;
      case LightMode.FlashLights: m = GD.FlashLight(); break;
      case LightMode.LightsOff: m = GD.LightOffRoom(); break;
    }
    foreach (SpriteRenderer sr in srs)
      sr.material = m;

    // All actors in the room should get the material
    foreach (Actor a in GD.c.allActors)
      if (a != null && a.currentRoom == this) a.SetLight(lightsStatus);
  }
}

public enum LightMode {
  LightsOn,
  FlashLights,
  LightsOff,
}