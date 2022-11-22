using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDoor : MonoBehaviour
{
    public List<bool> torches = new List<bool>();

    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            torches.Add(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (torches[0] && torches[1] && torches[2])
        {
            gameObject.SetActive(false);
        }
    }

    public void ActivateTorch(int TorchNumber)
    {
        torches[TorchNumber-1] = true;    
    }
}
