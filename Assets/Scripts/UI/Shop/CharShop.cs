using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharShop : MonoBehaviour
{
    public void CloseScene()
    {
        SceneManager.UnloadSceneAsync("CharShop");
        LoadoutState loadoutState = GameManager.instance.topState as LoadoutState;
        if (loadoutState != null)
        {
            loadoutState.Refresh();
        }
    }
}
