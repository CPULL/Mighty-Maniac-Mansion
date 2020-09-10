using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour {
  Animator anim;
  Vector3 destination = Vector2.zero;
  bool walking = false;
  Dir dir = Dir.F;

  private void Awake() {
    anim = GetComponent<Animator>();
  }

  public void WalkTo(Vector2 dest) {
    destination = dest;
    walking = true;
  }

  private void Update() {
    if (!walking) {
      anim.Play("Idle" + dir);
      return;
    }

    Vector3 wdir = destination - transform.position;
    float speed = 5;
    if (Mathf.Abs(wdir.y) > Mathf.Abs(wdir.x) * 1.05f) {
      if (wdir.y > 0) dir = Dir.B;
      if (wdir.y < 0) dir = Dir.F;
    }
    else {
      if (wdir.x > 0) dir = Dir.R;
      if (wdir.x < 0) dir = Dir.L;
      speed = 7.5f;
    }

    anim.Play("Walk" + dir);
    wdir.z = 0;
    if (wdir.sqrMagnitude < .25f) {
      walking = false;
      return;
    }

    wdir.Normalize();
    transform.position += wdir * speed * Time.deltaTime;

  }
}

public enum Dir {  F, B, L, R };
