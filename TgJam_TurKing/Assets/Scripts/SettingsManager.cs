using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

	public float callMauseTime = 0.2f;
	public float timeMouseOver = 0;

	private bool mouseCalled = false;


	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("Навели мышкой на Canvas!");
		
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Debug.Log("Убрали мышку с Canvas!");
	}
}
