using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room : MonoBehaviour
{
    public Room[] RoomConnections;
    public int People = 0;
    public int MaxPeople = 1;
    
    public Vector2 location = new Vector2 (0, 0);
    // Start is called before the first frame update
    void Start()
    {
        location = new Vector2(this.transform.position.x, this.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTeacherStay(Professor_Default Prof)
    {
        
    }
    public void OnTeacherEnter(Professor_Default Prof)
    {

    }
}
