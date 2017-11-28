using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField]
    float rotateSpeed = 100f;
    [SerializeField]
    float thrustSpeed = 100f;

    int currentLevel = 0;
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    Collision collision;
    Rigidbody rigidBody;
    AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Rotate();
            Thrust();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
            return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                state = State.Transcending; //to make control unable
                Invoke("LoadNextLevel", 2f);//After 2 seconds load next scene
                break;
            default:
                audioSource.Stop();
                state = State.Dying; //to make control unable
                Invoke("LoadFirstLevel", 2f);
                break;

        }

    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(++currentLevel);
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;
        float rotateFPS = rotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(Vector3.forward * rotateFPS);
        else if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(-Vector3.forward * rotateFPS);
        rigidBody.freezeRotation = false;
    }
    private void Thrust()
    {
        float thrustFPS = thrustSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustFPS);
        }
        else
            audioSource.Stop();
        if (Input.GetKeyDown(KeyCode.UpArrow))
            audioSource.Play();
    }
}
