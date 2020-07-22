using UnityEngine;
using System.Collections;

public class LEDFlicker : MonoBehaviour
{
	public float frequency = 1.5F;
	public float amplitude = 1.0F;

	private Color originalColor;
	private float initial = 0.0F;
	private float phase = 0.0F;
	private Light LED;

	void Start()
	{
		LED = GetComponent<Light>();
		originalColor = LED.color;
	}
	
	void Update()
	{
		LED.color = originalColor * (EvalWave());
	}
	
	float EvalWave()
	{
		float x = (Time.time + phase) * frequency;
		float y;
		
		x -= Mathf.Floor(x);
		y = 1 - (Random.value * 2);

		return (y * amplitude) + initial;
	}
}
