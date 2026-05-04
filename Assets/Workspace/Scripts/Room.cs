using UnityEngine;

public class Room : MonoBehaviour
{
    public string RoomID;
    public bool TestMode = false;

    public void Start()
    {
        if (TestMode)
        {
            OnEnterRoom();
        }
    }

    public virtual void OnEnterRoom()
    {
        
    }

    public virtual void OnExitRoom()
    {
        
    }
}
