using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// TODO:- Add comments

public class SpawnManager : MonoBehaviour {

    List<Transform> teamOne;

    // Use this for initialization
    void Awake() {
        Transform[] teamOneArray = transform.FindChild("TeamOne").GetComponentsInChildren<Transform>();
        
        teamOne = GetList(teamOneArray);

        teamOne.Remove(transform.Find("TeamOne"));
    }

    void Update()
    {
        // Destroy if all positions have been filled
        if (teamOne.Count == 0)
            DestroyObject(gameObject);
    }

    // Place the character in the given team if possible
    public void SetSpawnPoint(Transform character)
    {
        if (teamOne.Count > 0)
        {
            character.position = teamOne[0].position;
            DestroyObject(teamOne[0].gameObject);
            teamOne.RemoveAt(0);
        }
        else
            Debug.Log("All character slots on team ONE have been used. Make sure there is enough spawn points on the scene.");        
    }

    public List<Transform> GetList(Transform[] array)
    {
        List<Transform> list = new List<Transform>();

        int elementCount = array.Length;

        for(int i = 0; i < elementCount; i++)
        {
            list.Add(array[i]);
        }

        return list;
    } 
}
