using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSSet : MonoBehaviour 
{
	public int TargetFPS;

	protected virtual void Start()
	{
		Application.targetFrameRate = TargetFPS;
	}		
}
