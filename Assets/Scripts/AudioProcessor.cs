using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioProcessor : MonoBehaviour
{
    [Range(100f,100000f)]
    public float scale = 6000f;
    [Range(1f,300f)]
    public float BandScale = 100f;
    public GameObject animatingObject;

    private static int m_samplesCount = 1024;
    private GameObject[] m_animatingObjects = new GameObject[m_samplesCount];
    private float[] m_samples = new float[m_samplesCount];
    private float[] m_bands = new float[8];
    private GameObject[] m_animatingBand = new GameObject[8];
    private AudioSource m_audioSource;

	// Use this for initialization
	void Start ()
	{
	  /*  for (int i = 0; i < m_samplesCount; i++)
	    {
	        GameObject instance = Instantiate(animatingObject, transform);
	        instance.name = "Animating_" + i;
	        instance.transform.position = Vector3.forward * m_samplesCount/2f;
	        Vector3 direction = Vector3.Cross(instance.transform.position, transform.right) + instance.transform.position;
            instance.transform.LookAt(direction, instance.transform.position);
	        m_animatingObjects[i] = instance;
	        transform.eulerAngles = new Vector3(0, -((float)(360f/m_samplesCount))*i, 0);
	    }*/

	    CreateBands();

	    m_audioSource = GetComponent<AudioSource>();
	    foreach (string device in Microphone.devices) {
	        Debug.Log("Name: " + device);
	    }

	    m_audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, 44100);
	    m_audioSource.loop = true;
	    while (!(Microphone.GetPosition(null) > 0)){}
        m_audioSource.Play();


	}

    void CreateBands()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject instance = Instantiate(animatingObject, transform);
            instance.name = "Animating_" + i;
            instance.transform.position = Vector3.forward * (i*5f);
            Vector3 direction = Vector3.Cross(instance.transform.position, transform.right) + instance.transform.position;
           // instance.transform.LookAt(direction, instance.transform.position);
            m_animatingBand[i] = instance;
        }
    }

    void UpdateBands()
    {
        int currentIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0f;
            int samplesCount = (int) Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                samplesCount += 2;
            }
            for (int j = 0; j < samplesCount; j++)
            {
                average += m_samples[currentIndex] * (currentIndex + 1);
                currentIndex++;
            }

            average /= currentIndex;
            m_bands[i] = average * 10f;
            m_animatingBand[i].transform.localScale = new Vector3(1,Mathf.Lerp(m_animatingBand[i].transform.localScale.y, BandScale * m_bands[i] + 1, Time.deltaTime*5f),1);
        }
    }


	// Update is called once per frame
	void Update ()
	{
	    m_audioSource.GetSpectrumData(m_samples, 0, FFTWindow.Blackman);

	  /*  for (int i = 0; i < m_samplesCount/10; i++)
	    {
	        m_samples[i] *= (i/(m_samplesCount/10f));
	    }

	    for (int i = 0; i < m_samplesCount; i++)
	    {
	        if (m_animatingObjects[i])
	        {
                m_animatingObjects[i].transform.localScale = new Vector3(1, Mathf.Lerp(m_animatingObjects[i].transform.localScale.y, scale * m_samples[i], Time.deltaTime*5f) + 1,  1);
	        }
	    }*/

        UpdateBands();
	}
}
