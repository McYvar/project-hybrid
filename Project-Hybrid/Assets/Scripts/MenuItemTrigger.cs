using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuItemTrigger : MonoBehaviour
{
    public UnityEvent OnBallCollsion = null;
    public float waitTime = 0;

    public LoadbarInstance loadbar = null;
    [SerializeField] int loadSceneIndex = 0;

    public void StartGame()
    {
        Debug.Log("STARTED GAME!!!");
        SceneManager.LoadScene(loadSceneIndex);
        loadbar.confirm = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quited GAME!!!");
        loadbar.confirm = false;
    }

    public void IncorrectBody()
    {
        loadbar.confirm = false;
    }

    public void CorrectBody(int index)
    {
        TelescopeSeekingPuzzlePiece.BodyFound(index);
        loadbar.confirm = false;
        gameObject.SetActive(false);
    }

}

public enum CorrectBodies { Mars = 0, Uranus = 1, StarCluster = 2, Mercurius = 3 }
