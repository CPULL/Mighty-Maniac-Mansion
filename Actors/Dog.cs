using UnityEngine;

public class Dog : MonoBehaviour {

  public Animator HeadAnim;
  public Animator TailAnim;
  public Animator BodyAnim;
  public SpriteRenderer HeadSR;
  public SpriteRenderer TailSR;
  public SpriteRenderer BodySR;
  public AudioSource Audio;

  void Start() {

  }


  void Update() {
    if (Input.GetKeyDown(KeyCode.Alpha1)) BodyAnim.Play("Body Idle");
    if (Input.GetKeyDown(KeyCode.Alpha2)) BodyAnim.Play("Body Sit");
    if (Input.GetKeyDown(KeyCode.Alpha3)) BodyAnim.Play("Body Walk");

    if (Input.GetKeyDown(KeyCode.Alpha4)) HeadAnim.Play("Head Idle");
    if (Input.GetKeyDown(KeyCode.Alpha5)) HeadAnim.Play("Head Grind");
    if (Input.GetKeyDown(KeyCode.Alpha6)) HeadAnim.Play("Head Chow");

    if (Input.GetKeyDown(KeyCode.Alpha7)) TailAnim.Play("Tail Idle");
    if (Input.GetKeyDown(KeyCode.Alpha8)) TailAnim.Play("Tail Wiggle");

    if (Input.GetKeyDown(KeyCode.Alpha9)) {
      bool flip = !HeadSR.flipX;
      HeadSR.flipX = flip;
      BodySR.flipX = flip;
      TailSR.flipX = flip;
    }
  }
}
