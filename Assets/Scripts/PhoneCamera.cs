using UnityEngine;
using System.Collections;

public class PhoneCamera : MonoBehaviour 
{
//	[SerializeField] Renderer targetRender;
//	[SerializeField] Material targetMaterial;
	[SerializeField] Vector2 targetSize = new Vector2(640, 400);
	[SerializeField] int targetFrameRate = 60;

	WebCamTexture webcamTexture;

	public static System.Action<Vector2, WebCamTexture> onInitCamera;

	void Start()
	{
		Application.targetFrameRate = targetFrameRate;
		webcamTexture = new WebCamTexture((int)targetSize.x, (int)targetSize.y, targetFrameRate);
		Debug.Log(webcamTexture.deviceName);

//		targetRender.material.mainTexture = webcamTexture;
//		targetMaterial.mainTexture = webcamTexture;
		webcamTexture.Play();

		if (onInitCamera != null)
		{
			onInitCamera(targetSize, webcamTexture);
		}
	}
}
