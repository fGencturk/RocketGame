using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float loadLevelDelay = 2f;
    [SerializeField] float rotateSpeed = 100f, thrustSpeed = 100f;
    [SerializeField] AudioClip audioThrust, audioDead, audioWin;
    [SerializeField] bool collisionDebugOn = false;
    [SerializeField] ParticleSystem particleThrust, particleDead, particleWin;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

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
            RespondToRotateInput();
            RespondToThrustInput();
            if(Debug.isDebugBuild)//When the game is published, it will be false so that debug keys won't be used
                RespondToDebugInput();
        }
    }    

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionDebugOn)
            return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                Transcending();
                break;
            default:
                Dying();
                break;
        }
    }

    private void RespondToDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.C))
            collisionDebugOn = !collisionDebugOn;
        else if (Input.GetKeyDown(KeyCode.L))
            LoadNextLevel();
    }
    private void Transcending()
    {
        audioSource.Stop();
        particleThrust.Stop();
        audioSource.PlayOneShot(audioWin);
        particleWin.Play();
        state = State.Transcending; //to make control unable
        Invoke("LoadNextLevel", loadLevelDelay);
    }
    private void Dying()
    {
        rigidBody.constraints = RigidbodyConstraints.None;
        audioSource.Stop();
        particleThrust.Stop();
        audioSource.PlayOneShot(audioDead);
        particleDead.Play();
        state = State.Dying; //to make control unable
        Invoke("LoadFirstLevel", loadLevelDelay);
    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }
    private void LoadNextLevel()
    {
        //SceneManager.GetActiveScene().buildIndex + 1 is the index of next level
        /*
        if (SceneManager.GetActiveScene().buildIndex + 1 != SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(0);        */

        //The better way is to divide index of next level by number of levels
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true;
        float rotateFPS = rotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftArrow))
            transform.Rotate(Vector3.forward * rotateFPS);
        else if (Input.GetKey(KeyCode.RightArrow))
            transform.Rotate(-Vector3.forward * rotateFPS);
        rigidBody.freezeRotation = false;
    }
    private void RespondToThrustInput()
    {
        float thrustFPS = thrustSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            audioSource.PlayOneShot(audioThrust);
            particleThrust.Play();
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustFPS);
        }
        else
        {
            particleThrust.Stop();
            audioSource.Stop();
        }
        
    }
}
