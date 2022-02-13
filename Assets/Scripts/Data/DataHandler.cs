using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public static DataHandler instance;
    public GameData GameData;

    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        GameEvents.instance.onSelectPlayerEvents += SetSelectedCharacter;
    }
    private void OnDestroy()
    {
        GameEvents.instance.onSelectPlayerEvents -= SetSelectedCharacter;
    }

    public void SetSelectedCharacter(int i)
    {
        GameData.SetSelectedPlayer(i);
    }
    public int GetSelectedCharacter()
    {
        return GameData.GetSelectedPlayer();
    }

}
