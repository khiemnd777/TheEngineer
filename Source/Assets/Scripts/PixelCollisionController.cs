using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelCollisionController : MonoBehaviour
{
	public Pixel pixel;

	void OnCollisionEnter2D(Collision2D collision)
	{
		// var fromPixel = collision.gameObject.GetComponent<Pixel>();
		// if(fromPixel.IsNotNull())
		// 	return;
		// var fromPixelCollisionController = collision.gameObject.GetComponent<PixelCollisionController>();
		// if(fromPixelCollisionController.IsNull())
		// 	return;
		Debug.Log(collision.gameObject.name);
	}
}