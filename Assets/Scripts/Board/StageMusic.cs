using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StageMusic : MonoBehaviour {

    public static AudioSource instance;
    private AudioSource aud;

	void Awake()
    {
        aud = GetComponent<AudioSource>();

        if (instance == null) instance = aud;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().buildIndex < 4 || SceneManager.GetActiveScene().buildIndex > 15) Destroy(gameObject);
    }
}
