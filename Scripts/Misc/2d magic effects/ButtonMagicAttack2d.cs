using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMagicAttack2d : MonoBehaviour {

	public GameObject Root;
	public void NumberAdd() {
		MagicEffectHandler.No++;
		MagicEffectHandler demo = Root.GetComponent<MagicEffectHandler>();
		if(MagicEffectHandler.No > demo.Effects.Length - 1){
			MagicEffectHandler.No = 0;
		}
		demo.TextChange();
  	}

	public void NumberMinus() {
		MagicEffectHandler.No--;
		MagicEffectHandler demo = Root.GetComponent<MagicEffectHandler>();
		if(MagicEffectHandler.No < 0){
			MagicEffectHandler.No = demo.Effects.Length - 1;
		}
		demo.TextChange();
  	}
}
