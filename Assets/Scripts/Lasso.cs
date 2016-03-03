﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Lasso : MonoBehaviour 
{
	[Range(0, 100)]
	[SerializeField] float sensitivity = 50.0f;
	[Range(1, 50)]
	[SerializeField] int step = 10;

	[SerializeField] Color targetColor;
//	[SerializeField] MeshRenderer targetImage;
//	[SerializeField] Material targetMaterial;
	[SerializeField] RawImage targetImage;

	WebCamTexture sourceImage;

	float oldSensitivity;
	Vector3 colorA;
	Vector3 colorB;

	Texture2D tex;
	Color[] pixels;
	bool done = false;

	Camera cam;

	void Awake()
	{
		PhoneCamera.onInitCamera += OnInitCamera;

		oldSensitivity = sensitivity;
	}

	void OnInitCamera(Vector2 targetSize, WebCamTexture texture)
	{
		PhoneCamera.onInitCamera -= OnInitCamera;

		tex = new Texture2D((int)targetSize.x, (int)targetSize.y, TextureFormat.ARGB32, false);
		tex.filterMode = FilterMode.Bilinear;

		sourceImage = texture;
		cam = Camera.main;

		done = true;
	}

//	void OnGUI()
//	{
//		if (GUILayout.Button("Change Color"))
//		{
//			ChangeColor();
//		}
//	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			SelectColor();
		}

		if (done)
		{
			pixels = sourceImage.GetPixels();

//			Debug.Log(new Vector2(tex.width, tex.height).ToString() + " = " + (tex.width * tex.height).ToString() + "; " + pixels.Length.ToString());
			if (tex.width * tex.height > pixels.Length)
			{
				return;
			}

			done = false;

//			pixels = ((WebCamTexture)(targetImage.material.mainTexture)).GetPixels();
			Loom.RunAsync(ChangeColor);
			
			oldSensitivity = sensitivity;

		}
	}

	void SelectColor()
	{
		Vector3 point = cam.ScreenToViewportPoint(Input.mousePosition);

		targetColor = sourceImage.GetPixel((int)(point.x * sourceImage.width), (int)(point.y * sourceImage.height));

//		RaycastHit hit;
//		if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
//			return;
//		
//
//		Vector2 pixelUV = hit.textureCoord;
//		pixelUV.x *= sourceImage.width;
//		pixelUV.y *= sourceImage.height;
//
//
//		targetColor = sourceImage.GetPixel((int)pixelUV.x, (int)pixelUV.y);
	}

	void ChangeColor()
	{
		colorA = new Vector3(targetColor.r - sensitivity / 100.0f, 
		                     targetColor.g - sensitivity / 100.0f, 
		                     targetColor.b - sensitivity / 100.0f);
		
		colorB = new Vector3(targetColor.r + sensitivity / 100.0f, 
		                     targetColor.g + sensitivity / 100.0f, 
		                     targetColor.b + sensitivity / 100.0f);


		for (int i = 0; i < pixels.Length; i += step)
		{
			if (IsTargetColor(pixels[i]))
			{
				for (int j = i; j < i + step; j++)
				{
					if (j < pixels.Length)
					{
						pixels[j] = Color.clear;
					}
				}
//				Debug.Log("Set pixel");
			}
		}


		Loom.QueueOnMainThread(() =>
		                       {
			tex.SetPixels(pixels);
			tex.Apply();

//			targetImage.material.SetTexture("_MainTex", tex);
//			targetMaterial.SetTexture("_MainTex", tex);
			targetImage.texture = tex;
			done = true;
		});
	}

	bool IsTargetColor(Color color)
	{
		if (color.r >= colorA.x && color.r <= colorB.x &&
		    color.g >= colorA.y && color.g <= colorB.y &&
		    color.b >= colorA.z && color.b <= colorB.z)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

}
