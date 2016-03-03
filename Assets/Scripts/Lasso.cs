using UnityEngine;
using System.Collections;

public class Lasso : MonoBehaviour 
{
	[Range(0, 100)]
	[SerializeField] float sensitivity = 50.0f;
	[Range(1, 50)]
	[SerializeField] int step = 10;

	[SerializeField] Color targetColor;
	[SerializeField] MeshRenderer targetImage;
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
			done = false;
			pixels = sourceImage.GetPixels();
//			pixels = ((WebCamTexture)(targetImage.material.mainTexture)).GetPixels();
			Loom.RunAsync(ChangeColor);
			
			oldSensitivity = sensitivity;
		}
	}

	void SelectColor()
	{

		RaycastHit hit;
		if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
			return;
		
//		Renderer rend = hit.transform.GetComponent<Renderer>();
//		MeshCollider meshCollider = hit.collider as MeshCollider;
//		if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
//			return;

		Vector2 pixelUV = hit.textureCoord;
		pixelUV.x *= sourceImage.width;
		pixelUV.y *= sourceImage.height;


		targetColor = sourceImage.GetPixel((int)pixelUV.x, (int)pixelUV.y);
//		targetColor = sourceImage.GetPixel(-(int)Input.mousePosition.x, (int)Input.mousePosition.y);
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

			targetImage.material.SetTexture("_MainTex", tex);
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
