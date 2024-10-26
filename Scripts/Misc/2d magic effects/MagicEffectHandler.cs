using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//let cannibaalize this from the original demo class
public class MagicEffectHandler : MonoBehaviour, IPointerClickHandler
{

	public GameObject[] Effects = new GameObject[0];

	//public Text EffectText;
	[System.NonSerialized] public static int No;

	private Vector3 clickPosition;
	private Vector3 effectPosition;

	public int effectNb;

	public bool isDemoScene;

	public Text EffectText;

	public GameObject noTimingFoeTarget;
	public GameObject noTimingHeroTarget;

	// Use this for initialization
	void Start () 
	{
		effectNb = 0;

		No = 0;
		TextChange();
	}

	public void TextChange()
	{
		if (isDemoScene == true)
		{
			EffectText.text = Effects[No].name;
		}
	}
	
	//dont rly need this either for now, but lets keep it as template
	 
	public void OnPointerClick (PointerEventData eventData)
	{
		if (isDemoScene == true)
		{
			clickPosition = Input.mousePosition;
			clickPosition.z = 10f;
			GameObject obj = Instantiate(Effects[No], Camera.main.ScreenToWorldPoint(clickPosition), Effects[No].transform.rotation);
			EffectText.text = Effects[No].name;
			Destroy(obj, 3f);
		}
	}
	
	//not used in demo scene?
	//replaced by coroutine
	//targetType 1=current foe target, 2=hero
	public void SpawnAttackEffect2D(int targetType)
	{
		//clickPosition = Input.mousePosition;
		//effectPosition.z = 1f;

		//GameObject obj = Instantiate(Effects[effectNumber], Camera.main.ScreenToWorldPoint(clickPosition), Effects[effectNumber].transform.rotation);

		if (GameManager.ins.references.targettingHandler.targettingEnabled == true)
		{
			effectPosition = GameManager.ins.references.targettingHandler.crosshair.transform.position;
			//effectPosition = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget -1].FoeImage.transform.position;

			GameObject obj = Instantiate(Effects[effectNb], effectPosition, GameManager.ins.references.targettingHandler.crosshair.transform.rotation);
			//EffectText.text = Effects[No].name;
			Destroy(obj, 3f);
		}
		else if (targetType == 1)//(CardHandler.ins.phaseNumber == 3 && GameManager.ins.references.targettingHandler.targettingEnabled == false)
		{
			//effectPosition = GameManager.ins.encounterHandler.GetComponent<EncounterHandler>().encounterIcon.gameObject.transform.position;
			//effectPosition = noTimingFoeTarget.gameObject.transform.position;

			//for v0.5.7.(not rly)
			float foeHeight = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].FoeImage.GetComponent<RectTransform>().rect.height *
				GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].FoeImage.GetComponent<RectTransform>().lossyScale.y;

			//Debug.Log("foe height is: " + foeHeight);
			effectPosition = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[GameManager.ins.exploreHandler.GetComponent<MultiCombat>().currentTarget - 1].FoeImage.transform.position;

			effectPosition.y = effectPosition.y + foeHeight / 2;

			//effectPosition = new Vector3(0, 0, 0);
			GameObject obj = Instantiate(Effects[effectNb], effectPosition, GameManager.ins.references.targettingHandler.crosshair.transform.rotation);
			//obj.transform.SetParent(noTimingFoeTarget.gameObject.transform, false);
			//obj.transform.parent = noTimingFoeTarget.gameObject.transform;
			//obj.transform.position = noTimingHeroTarget.gameObject.transform.position;

			Destroy(obj, 3f);
		}
		else if (targetType == 2)//(CardHandler.ins.phaseNumber == 4 && GameManager.ins.references.targettingHandler.targettingEnabled == false)
		{
			//effectPosition = GameManager.ins.characterDisplays.GetComponent<CharacterDisplays>().characters[GameManager.ins.avatars[GameManager.ins.turnNumber].GetComponentInChildren<Character>().heroNumber].gameObject.transform.position;
			
			//actually lets use bottom character icon as target for now
			//effectPosition = GameManager.ins.characterIcons[0].gameObject.transform.position;
			effectPosition = noTimingHeroTarget.gameObject.transform.position;

			//effectPosition = new Vector3(0, 0, 0);
			GameObject obj = Instantiate(Effects[effectNb], effectPosition, GameManager.ins.references.targettingHandler.crosshair.transform.rotation);
			//obj.transform.SetParent(noTimingHeroTarget.gameObject.transform, false);
			//obj.transform.position = noTimingHeroTarget.gameObject.transform.position;

			//Debug.Log("should create effect: " + effectNb);
			Destroy(obj, 3f);
		}
	}

	public void AttackThreeTimesOneSecInterval(int effectNumber)
    {
		effectNb = effectNumber;

		SpawnAttackEffect2D(1);

		Invoke("SpawnAttackEffect2D", 1f);
		Invoke("SpawnAttackEffect2D", 2f);
	}

	public void AttackThreeTimesHalfSecInterval(int effectNumber, int target)
	{
		//effectNb = effectNumber;
		//SpawnAttackEffect2D(1);
		//Invoke("SpawnAttackEffect2D", 0.5f);
		//Invoke("SpawnAttackEffect2D", 1f);
		//SpawnEffect(effectNumber, 0, Effects, noTimingHeroTarget.gameObject.transform.position);
		//StartCoroutine(SpawnEffect(effectNumber, 0, Effects, noTimingHeroTarget.gameObject.transform.position));

		//this is technically target 4
		Vector3 effectTarget = noTimingHeroTarget.gameObject.transform.position;

		//this should technically work? (might be janky tho)
		if(target == 1 || target == 2 || target == 3)
        {
			float foeHeight = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[target - 1].FoeImage.GetComponent<RectTransform>().rect.height *
				GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[target - 1].FoeImage.GetComponent<RectTransform>().lossyScale.y;

			Vector3 effectPos = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[target - 1].FoeImage.transform.position;

			effectPos.y = effectPos.y + foeHeight / 2;

			effectTarget = effectPos;
		}


		StartCoroutine(SpawnEffect(effectNumber, 0, effectTarget));
		StartCoroutine(SpawnEffect(effectNumber, 0.3f, effectTarget));
		StartCoroutine(SpawnEffect(effectNumber, 0.6f, effectTarget));
	}

	public void AttackOnce(int effectNumber, int target)
	{
		//this is technically target 4
		Vector3 effectTarget = noTimingHeroTarget.gameObject.transform.position;

		//this should technically work? (might be janky tho)
		if (target == 1 || target == 2 || target == 3)
		{
			float foeHeight = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[target - 1].FoeImage.GetComponent<RectTransform>().rect.height *
				GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[target - 1].FoeImage.GetComponent<RectTransform>().lossyScale.y;

			Vector3 effectPos = GameManager.ins.exploreHandler.GetComponent<MultiCombat>().BattlefieldFoes[target - 1].FoeImage.transform.position;

			effectPos.y = effectPos.y + foeHeight / 2;

			effectTarget = effectPos;
		}

		StartCoroutine(SpawnEffect(effectNumber, 0, effectTarget));
	}

	//position 1= hero icon, 2=timer 
	//is used separately from combat spawns for now?
	public void SpawnEffectOnce(int effectNumber, int position)
	{
		effectNb = effectNumber;

		SpawnEffect2D(position);
	}

	//could be used for non-combat effects?
	public void SpawnEffect2D(int position)
	{
		//hero display position
		if (position == 1)
		{
			effectPosition = GameManager.ins.characterIcons[0].gameObject.transform.position;

			GameObject obj = Instantiate(Effects[effectNb], effectPosition, GameManager.ins.references.targettingHandler.crosshair.transform.rotation);
			Destroy(obj, 3f);
		}
		//timer position
		else if (position == 2)
		{
			effectPosition = Clock.clock.dayToken.gameObject.transform.position;
			GameObject obj = Instantiate(Effects[effectNb], effectPosition, GameManager.ins.references.targettingHandler.crosshair.transform.rotation);

			Destroy(obj, 3f);
		}
		//encounter display foe position
		else if (position == 3)
		{
			effectPosition = GameManager.ins.characterDisplays.GetComponent<MagicEffectHandler>().noTimingFoeTarget.gameObject.transform.position;
			GameObject obj = Instantiate(Effects[effectNb], effectPosition, GameManager.ins.references.targettingHandler.crosshair.transform.rotation);

			Destroy(obj, 3f);
		}
	}

	//target 1=foe1, 2=foe2, 3=foe3, 4=hero
	IEnumerator SpawnEffect(int effectNumber, float delay, Vector3 effectTarget)
	{
		float currentTime = 0;
		//float currentVol = audioSource.volume;
		//audioSource.GetFloat(exposedParam, out currentVol);
		//float currentVol = Mathf.Pow(10, audioSource.volume / 20);
		//float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

		while (currentTime < delay)
		{
			currentTime += Time.deltaTime;
			//float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
			//audioSource.volume = newVol;
			//audioSource.volume = Mathf.Log10(newVol) * 20;


			yield return null;
		}

		if (currentTime >= delay)
		{
			GameObject obj = Instantiate(Effects[effectNumber], effectTarget, GameManager.ins.references.targettingHandler.crosshair.transform.rotation);

			Destroy(obj, 3f);
		}
		yield break;
	}
	/*
	//target 1=foe1, 2=foe2, 3=foe3, 4=hero
	public static IEnumerator SpawnEffect(int effectNumber, float delay, GameObject[] Effects, Vector3 effectTarget)
	{
		Debug.Log("calls coroutine");

		float currentTime = 0;
		//float currentVol = audioSource.volume;
		//audioSource.GetFloat(exposedParam, out currentVol);
		//float currentVol = Mathf.Pow(10, audioSource.volume / 20);
		//float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

		while (currentTime < delay)
		{
			currentTime += Time.deltaTime;
			//float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
			//audioSource.volume = newVol;
			//audioSource.volume = Mathf.Log10(newVol) * 20;


			yield return null;
		}

		if (currentTime >= delay)
		{
			Debug.Log("current time exceeds delay");

			GameObject obj = Instantiate(Effects[effectNumber], effectTarget, GameManager.ins.references.targettingHandler.crosshair.transform.rotation);

			Destroy(obj, 3f);

		}
		yield break;
	}
	*/
}
