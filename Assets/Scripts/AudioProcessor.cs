using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioProcessor : MonoBehaviour
{
    [Range(100f,100000f)]
    public float scale = 600f; // scale of samples amplitude
    [Range(1f,300f)]
    public float BandScale = 100f; // scale of bands amplitude
     [Range(1f,10f)]
    public float BandEmissionScale = 5f; // scale of bands amplitude
    [Range(1f,10f)]
    public float smoothness = 5f; // smooth value for animation
    public GameObject animatingObject; // prefab for animation
    public Color DefaultEmissionColor;

    public bool UsingPredefinedBands = true;
    public Transform BandsParent;

    private static int m_samplesCount = 1024; // count of samples that contains amplitudes

    private float[] m_samples = new float[m_samplesCount]; // amplitudes array
    private GameObject[] m_animatingObjects = new GameObject[m_samplesCount]; // array of objects, animated by samples amplitude

    private float[] m_bands = new float[8]; // array of bands that contains average amplitude of frequency ranges
    private GameObject[] m_animatingBand = new GameObject[8]; // array of objects, animated by bands amplitude

    private AudioSource m_SoundSource; // audio source that contains input form default recording device

	// Use this for initialization
	void Start ()
	{
        if(!UsingPredefinedBands){
	    CreateBands();
        }
        else
        {
             for (int i = 0; i < 8; i++)
        {
            m_animatingBand[i] = BandsParent.GetChild(i).gameObject;
                m_animatingBand[i].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", DefaultEmissionColor);
                    }
        }

	    m_SoundSource = GetComponent<AudioSource>();
	    foreach (string device in Microphone.devices) {
	        Debug.Log("Name: " + device);
	    }

	    m_SoundSource.clip = Microphone.Start(Microphone.devices[0], true, 1, 48000);
	  //  m_SoundSource.loop = true;
	    while (!(Microphone.GetPosition(null) > 0)){}
        m_SoundSource.Play();
	}

     // Adding child objects to animate by samples
    void CreateSamples()
    {
         for (int i = 0; i < m_samplesCount; i++)
	    {
	        GameObject instance = Instantiate(animatingObject, transform);
	        instance.name = "Animating_" + i;
	        instance.transform.position = Vector3.forward * m_samplesCount/2f;
	        Vector3 direction = Vector3.Cross(instance.transform.position, transform.right) + instance.transform.position;
            instance.transform.LookAt(direction, instance.transform.position);
	        m_animatingObjects[i] = instance;
	        transform.eulerAngles = new Vector3(0, -((float)(360f/m_samplesCount))*i, 0);
	    }
    }

    // Adding child objects to animate by bands
    void CreateBands()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject instance = Instantiate(animatingObject, transform);
            instance.name = "Animating_" + i;
            instance.transform.position = Vector3.right * (i*3f);
            Vector3 direction = Vector3.Cross(instance.transform.position, transform.right) + instance.transform.position;
            m_animatingBand[i] = instance;
        }
    }

    // Updates band objects scale by sound input
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
            m_animatingBand[i].transform.localScale = new Vector3(1,Mathf.Lerp(m_animatingBand[i].transform.localScale.y, BandScale * m_bands[i] + 1, Time.deltaTime*smoothness),1);
            m_animatingBand[i].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor",DefaultEmissionColor*(BandEmissionScale * m_bands[i]+1f));
        }
    }

     // Updates sample objects scale by sound input
    void UpdateSamples()
    {
        for (int i = 0; i < m_samplesCount/10; i++)
	    {
	        m_samples[i] *= (i/(m_samplesCount/10f));
	    }

	    for (int i = 0; i < m_samplesCount; i++)
	    {
	        if (m_animatingObjects[i])
	        {
                m_animatingObjects[i].transform.localScale = new Vector3(1, Mathf.Lerp(m_animatingObjects[i].transform.localScale.y, scale * m_samples[i], Time.deltaTime*smoothness) + 1,  1);
	        }
	    }
    }


	// Update is called once per frame
	void Update ()
	{
	    m_SoundSource.GetSpectrumData(m_samples, 0, FFTWindow.Blackman);

        UpdateBands();
	}
}
