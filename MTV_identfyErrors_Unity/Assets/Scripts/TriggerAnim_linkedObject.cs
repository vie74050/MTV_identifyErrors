﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * (SO=Selectable Object)
 * Add to SO item that is meant to act as an invisible trigger animation clip on the assigned 'linked_object_anim' 
 * 2023 note: Animation must use animation clips that are AnimationType "Legacy" 
*/

public class TriggerAnim_linkedObject : MonoBehaviour {
	[Tooltip("The anim of a linked object to trigger if selected")]
	public Animation linked_object_anim;

	public AnimationClip clipToPlay;  // set specific clip to play for multistep use.

	private bool col_status;			  // reference to the initial collider status (on or off)  
	private SelectableObject so;
	private bool animTriggered = false;

	void Start(){
		so = GetComponent<SelectableObject> ();
		col_status = GetComponent<Collider> ().enabled;
	}

	// handles broadcast message event from parent SelectableObject
	private void e_selected(){
		
	}

	private void e_reset(){
		linked_object_anim.Rewind();
		linked_object_anim.Stop();
		animTriggered = false;
		//Debug.Log ("reset anim " + transform.name);
	}

	private void e_resetAll(){
        //Debug.Log ("Reset all");
        e_reset();
		GetComponent<Collider> ().enabled = col_status;
	}

	private void OnMouseDown(){
		triggerAnim();
	}
	private void triggerAnim() {
		string clip_name = clipToPlay.name;

		if (!animTriggered && so.doSnapToTarget) {
			linked_object_anim.wrapMode = WrapMode.Once;
			linked_object_anim.Play(clip_name);
			animTriggered = true;

			Debug.Log ("play anim " + linked_object_anim);
		}
	}
}
