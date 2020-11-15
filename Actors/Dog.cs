using UnityEngine;

public class Dog : MonoBehaviour {

  public Animator HeadAnim;
  public Animator TailAnim;
  public Animator BodyAnim;
  public SpriteRenderer HeadSR;
  public SpriteRenderer TailSR;
  public SpriteRenderer BodySR;
  public AudioSource Audio;
  public Room currentRoom;

  /* FIXME
   
  // Check its own conditions


  If not friend, stay put if the player is far away, use grind if closer
  From time to time move and grind
  Bark also.


   
   
   */

  bool friendly = true;
  float friendCheck = 1f;

  float timeout = 1f;
  bool sit = false;
  bool tail = false;
  bool tongue = false;
  bool walking = false;
  float walkTime = 0;
  float walkedTime = 0;
  Vector3 startpos, endpos;

  public float dist = 0;

  private void Start() {
    ScaleByPosition(transform.position.y);
  }

  void Update() {
    if (!friendly) {
      friendCheck -= Time.deltaTime;
      if (friendCheck < 0) {
        friendly = AllObjects.GetFlag(GameFlag.SamIsFriend) != 0;
        friendCheck = 1f;
      }
    }
    dist = Mathf.Abs(GD.c.currentActor.transform.position.x - transform.position.x);

    if (friendly || dist > 20) { // Friend
      // If made friend, just stay close to the home, and move tail, toungue randomly

      if (walking) {
        transform.localPosition = Vector3.Lerp(startpos, endpos, walkedTime / walkTime);
        ScaleByPosition(transform.position.y);
        walkedTime += Time.deltaTime;
        if (walkedTime >= walkTime) {
          BodyAnim.Play("Body Idle");
          walking = false;
        }
      }

      timeout -= Time.deltaTime;
      if (timeout > 0) return;
      timeout = 1;


      int rnd = Random.Range(0, 5);
      if (rnd == 0 && !walking) { // Stay idle --------------------------------------------------------------------------------------------------
        if (sit) {
          sit = false;
          Vector3 pos = transform.localPosition;
          pos.y += .3f;
          transform.localPosition = pos;
        }
        BodyAnim.Play("Body Idle");
      }
      else if (rnd == 1 && !walking) { // Walk to random pos --------------------------------------------------------------------------------------------------
        if (sit) {
          sit = false;
          Vector3 pos = transform.localPosition;
          pos.y += .3f;
          transform.localPosition = pos;
        }
        BodyAnim.Play("Body Walk");

        startpos = transform.localPosition;
        endpos = Vector3.zero;
        endpos.x += Random.Range(-3, 0);
        endpos.y += Random.Range(-.9f, .5f);
        bool flip = endpos.x < startpos.x;
        HeadSR.flipX = flip;
        BodySR.flipX = flip;
        TailSR.flipX = flip;
        walking = true;
        walkTime = (endpos - startpos).magnitude;
        walkedTime = 0;
      }
      else if (rnd == 2 && !walking) { // Stay put --------------------------------------------------------------------------------------------------
        if (sit) return;
        sit = true;
        Vector3 pos = transform.localPosition;
        pos.y -= .3f;
        transform.localPosition = pos;
        BodyAnim.Play("Body Sit");
      }
      else if (rnd == 3) { // Wiggle tail on/off --------------------------------------------------------------------------------------------------
        tail = !tail;
        if (tail)
          TailAnim.Play("Tail Idle");
        else
          TailAnim.Play("Tail Wiggle");
      }
      else if (rnd == 4) { // Tongue on/off --------------------------------------------------------------------------------------------------
        tongue = !tongue;
        if (tongue)
          HeadAnim.Play("Head Tongue");
        else
          HeadAnim.Play("Head Idle");
      }

    }
    else if (dist > 10) { // Not friend and too close

      // Point actor and grind. Stop wiggle
      // Bark from time to time

    }
    else { // Not friend and too close
      // Block actor, let currentactor say something, and then walk away
      // Point actor and bark
      // Bark strong

    }

  }

  void ScaleByPosition(float y) {
    float ty = y;
    if (ty < currentRoom.minY) ty = currentRoom.minY;
    if (ty > currentRoom.maxY) ty = currentRoom.maxY;
    float scaley = -.05f * (ty - currentRoom.minY - 1.9f) + .39f;

    scaley *= currentRoom.scalePerc * 1.25f;
    transform.localScale = new Vector3(scaley, scaley, 1);
    int zpos = (int)(scaley * 10000);
    TailSR.sortingOrder = zpos;
    HeadSR.sortingOrder = zpos + 1;
    BodySR.sortingOrder = zpos + 2;
  }

}
