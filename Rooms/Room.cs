using System;
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
  public bool lights;
  List<SpriteRenderer> srs;

  private void Start() {
    srs = new List<SpriteRenderer>();
    CollectAllRenderers(transform);
  }

  void CollectAllRenderers(Transform tran) {
    SpriteRenderer sr = tran.GetComponent<SpriteRenderer>();
    if (sr != null && sr.material.name.IndexOf("SceneSelectionPoint") == -1) 
      srs.Add(sr);
    foreach (Transform t in tran)
      CollectAllRenderers(t);
  }


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

  internal void SetMaterial(Material mat) {
    foreach(SpriteRenderer sr in srs)
    sr.material = mat;
  }

  internal void SetLights(bool lightsOn) {
    lights = lightsOn;
    foreach (SpriteRenderer sr in srs)
      sr.material = lights ? GD.Normal() : GD.LightOffRoom();

    // All actors in the room should get the material
    foreach (Actor a in GD.c.allActors)
      if (a != null && a.currentRoom == this) a.SetLight(lights);
  }

  internal void UpdateLights() {
    foreach (SpriteRenderer sr in srs)
      sr.material = lights ? GD.Normal() : GD.LightOffRoom();
    foreach (Actor a in GD.c.allActors)
      if (a != null && a.currentRoom == this) a.SetLight(lights);
  }
}
