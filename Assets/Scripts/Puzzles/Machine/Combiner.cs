﻿using System.Collections;
using UnityEngine;

// Takes 4 objects and combines them to create a key for
// the archives.
public class Combiner : MonoBehaviour
{
    public Creator creator;
    public bool isActive = false;
    public Material pressedMaterial;
    public MachineSensor[] sensors = new MachineSensor[4];

    private MachineKey[] currentKeys = new MachineKey[4];
    private Material defaultMaterial;
    private bool isPressed = false;

    public Rigidbody extractorDoor;
    public Transform doorOpenPos;
    public Transform doorClosePos;


    public Rigidbody smallDoor;
    public Transform smallDoorOpen;
    public Transform smallDoorClosed;

    public AudioSource SFXSource;
    public AudioSource doorSource;
    public AudioSource jarFillSource;
    public AudioClip processingSound;
    

    // Returns true if all sensors are populated, false otherwise
    // If true, currentKeys contains the keys on the sensors
    public bool CheckKeys()
    {
        for (int i = 0; i < 4; i++)
        {
            if (!sensors[i].ContainsKey())
            {
                return false;
            }

            currentKeys[i] = sensors[i].currentKey;
        }

        return true;
    }

    // Returns a string comprised of 4 characters
    private string CombineKeys()
    {
        string combined = "";

        foreach (MachineKey key in currentKeys)
        {
            combined += key.ID;
        }

        return combined;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isActive && !isPressed && other.gameObject.tag == "Player")
        {
            StartCoroutine("PressButton");
        }
    }

    public IEnumerator ProcessSamples()
    {
        SFXSource.clip = processingSound;
        SFXSource.Play();
        yield return new WaitForSeconds(processingSound.length);

        jarFillSource.Play();
    }

    // Makes button unable to be pressed again for 3 seconds after it is
    // initially pressed. (We can make this the animation time)
    public IEnumerator PressButton()
    {
        doorSource.Play();
        StartCoroutine(Movement.SmoothMove(doorClosePos.position, 1.5f, extractorDoor));
        StartCoroutine(Movement.SmoothRotate(doorClosePos.rotation, 1.5f, extractorDoor));

        StartCoroutine(Movement.SmoothMove(smallDoorClosed.position, 1.5f, smallDoor));
        StartCoroutine(Movement.SmoothRotate(smallDoorClosed.rotation, 1.5f, smallDoor));
        Renderer renderer = GetComponent<Renderer>();

        // "Press" the button
        isPressed = true;
        renderer.material = pressedMaterial;

        // Wait for animation
        yield return new WaitForSeconds(processingSound.length + jarFillSource.clip.length);
        if (CheckKeys())
        {
            creator.CreateKey(CombineKeys());

            StartCoroutine(ProcessSamples());
           

        }


        // button is no longer pressed
        isPressed = false;
        renderer.material = defaultMaterial;
        doorSource.Play();
        StartCoroutine(Movement.SmoothMove(doorOpenPos.position, 2f, extractorDoor));

        StartCoroutine(Movement.SmoothRotate(doorOpenPos.rotation, 2f, extractorDoor));


        StartCoroutine(Movement.SmoothMove(smallDoorOpen.position, 1.5f, smallDoor));
        StartCoroutine(Movement.SmoothRotate(smallDoorOpen.rotation, 1.5f, smallDoor));
    }

    private void Start()
    {
        defaultMaterial = GetComponent<Renderer>().material;
    }
}