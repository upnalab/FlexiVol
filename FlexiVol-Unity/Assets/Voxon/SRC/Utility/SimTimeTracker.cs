using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Voxon.VXTextComponent))]
public class SimTimeTracker : MonoBehaviour
{
	double TimePassed = 0;
	Voxon.VXTextComponent text;
	// Start is called before the first frame update
	void Start()
    {
		text = GetComponent<Voxon.VXTextComponent>();
    }

    // Update is called once per frame
    void Update()
    {
		TimePassed += Time.deltaTime;
		int minutes = (int)(TimePassed / 60);
		int seconds = (int)(TimePassed % 60);
		text.SetString($"SimTime: {minutes.ToString("D2")}:{seconds.ToString("D2")}");

	}
}
