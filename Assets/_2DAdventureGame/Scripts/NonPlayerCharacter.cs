using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogueBubble;

    void Start()
    {
        if (dialogueBubble != null)
        {
            dialogueBubble.SetActive(false);
        }
    }
}