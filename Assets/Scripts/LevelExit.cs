using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    private BoxCollider2D _boxCollider;
    private bool _canExitLevel = false;

    private void Start()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.isTrigger = false;

        GameManager.Instance.onCollectedArtefacts += EnableLevelExit;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(ConstsEnums.PlayerTag))
            return;

        if (_canExitLevel)
        {
            GameManager.Instance.onCollectedArtefacts -= EnableLevelExit;

            gameObject.SetActive(false);
            //Finalize the code...
            Debug.Log("Level Complete!");
            Debug.Log("Next level loading...");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag(ConstsEnums.PlayerTag))
            return;

        Debug.Log("You have to collect all artefacts!");
    }

    public void EnableLevelExit()
    {
        _boxCollider.isTrigger = true;
        _canExitLevel = true;
    }
}
