using UnityEngine;
using UnityEngine.Audio;

public class Checkpoint : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip checkPointSound;
    [Range(0f, 2f)] public float checkPointVolume = 1f; // Adjust volume slider in Inspector

    private AudioSource audioSource;
    int audioCheck = 0;

    private void Start()
    {
        // Automatically get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerRespawn.respawnPosition = transform.position;
            if (checkPointSound != null && audioCheck == 0)
            {
                audioSource.PlayOneShot(checkPointSound, checkPointVolume);
                audioCheck++;
            }
      
            Debug.Log("Checkpoint Updated!");
        }
    }
}