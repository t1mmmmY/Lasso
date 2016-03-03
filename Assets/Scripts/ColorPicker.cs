using UnityEngine;
using System.Collections;

public class ColorPicker : MonoBehaviour 
{
	[SerializeField] Color targetColor;
	[Range(0.0f, 1.0f)]
	[SerializeField] float sensitivity = 0.5f;
	[SerializeField] Renderer targetRender;

	WebCamTexture sourceImage;
	Camera cam;


	void Awake()
	{
		PhoneCamera.onInitCamera += OnInitCamera;
	}

	void OnInitCamera(Vector2 targetSize, WebCamTexture texture)
	{
		PhoneCamera.onInitCamera -= OnInitCamera;
		
		sourceImage = texture;
		cam = Camera.main;
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			SelectColor();
		}
	}
	
	void SelectColor()
	{
		RaycastHit hit;
		if (!Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit))
			return;

		Vector2 pixelUV = hit.textureCoord;
		pixelUV.x *= sourceImage.width;
		pixelUV.y *= sourceImage.height;
		
		
		targetColor = sourceImage.GetPixel((int)pixelUV.x, (int)pixelUV.y);
		//		targetColor = sourceImage.GetPixel(-(int)Input.mousePosition.x, (int)Input.mousePosition.y);
	}
}
