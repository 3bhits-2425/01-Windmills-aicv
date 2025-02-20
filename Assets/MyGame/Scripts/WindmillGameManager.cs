using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WindmillGameManager : MonoBehaviour
{
    private List<GameObject> windmills = new List<GameObject>();
    private List<GameObject> rotorHubs = new List<GameObject>();
    private List<Slider> windmillSliders = new List<Slider>();
    private List<Button> lockButtons = new List<Button>();

    [SerializeField] private Renderer colorTarget;

    private List<float> windmillSpeeds = new List<float>();
    private int currentWindmillIndex = 0;
    private bool[] isLocked;
    private bool allLocked = false;
    private float maxRotationSpeed = 255f;
    private float decreaseRate = 100f;

    private void Start()
    {
        // Find all windmills, sliders, and lock buttons in the scene
        GameObject[] windmillObjects = GameObject.FindGameObjectsWithTag("Windmill");
        foreach (GameObject windmill in windmillObjects)
        {
            rotorHubs.Add(windmill.transform.Find("RotorHub").gameObject);
            windmillSliders.Add(windmill.GetComponentInChildren<Slider>());
            lockButtons.Add(windmill.GetComponentInChildren<Button>());
            Debug.Log("Object found");
        }
        Debug.Log(windmillObjects.Length);

        // Initialize windmill speeds and locked status
        windmillSpeeds = new List<float>(new float[windmills.Count]);
        isLocked = new bool[windmills.Count];

        // Set up lock button listeners
        for (int i = 0; i < lockButtons.Count; i++)
        {
            int index = i; // Capture the index for the closure
            lockButtons[i].onClick.AddListener(() => LockAndApplyCurrentWindmill(index));
        }
        EnableCurrentWindmill();
    }


    private void Update()
    {
        if (!allLocked && currentWindmillIndex < windmills.Count)
        {
            if (!isLocked[currentWindmillIndex])
            {
                if (Input.GetKey(KeyCode.Space))
                {
                    IncreaseWindmillValue(Time.deltaTime);
                }
                else
                {
                    DecreaseWindmillValue(Time.deltaTime);
                }
            }
        }
        RotateWindmills();
    }

    private void EnableCurrentWindmill()
    {
        for (int i = 0;i < rotorHubs.Count; i++)
        {
            rotorHubs[i].SetActive(enabled);
        }
    }

    private void IncreaseWindmillValue(float deltaTime)
    {
        windmillSpeeds[currentWindmillIndex] = Mathf.Clamp(windmillSpeeds[currentWindmillIndex] + (deltaTime * 100f), 0, maxRotationSpeed);
        windmillSliders[currentWindmillIndex].value = windmillSpeeds[currentWindmillIndex];
    }

    private void DecreaseWindmillValue(float deltaTime)
    {
        if (!isLocked[currentWindmillIndex])
        {
            windmillSpeeds[currentWindmillIndex] = Mathf.Clamp(windmillSpeeds[currentWindmillIndex] - (deltaTime * decreaseRate), 0, maxRotationSpeed);
            windmillSliders[currentWindmillIndex].value = windmillSpeeds[currentWindmillIndex];
        }
    }

    private void RotateWindmills()
    {
        for (int i = 0; i < windmills.Count; i++)
        {
            windmills[i].transform.Rotate(0, 0, windmillSpeeds[i] * Time.deltaTime);
        }
    }

    private void LockAndApplyCurrentWindmill(int index)
    {
        if (index < windmills.Count && !isLocked[index])
        {
            isLocked[index] = true;

            if (index < windmills.Count - 1)
            {
                currentWindmillIndex++;
            }
            else
            {
                allLocked = true;
            }
            ApplyColor();
        }
    }

    private void ApplyColor()
    {
        float r = windmillSpeeds.Count > 0 ? windmillSpeeds[0] / maxRotationSpeed : 0;
        float g = windmillSpeeds.Count > 1 ? windmillSpeeds[1] / maxRotationSpeed : 0;
        float b = windmillSpeeds.Count > 2 ? windmillSpeeds[2] / maxRotationSpeed : 0;

        Color newColor = new Color(r, g, b);
        colorTarget.GetComponent<Renderer>().material.color = newColor;
    }
}