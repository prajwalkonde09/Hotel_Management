using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    #region Singleton
    public static RoomManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    #endregion

    #region Variables
    public List<Transform> LockedRooms = new List<Transform>();
    public List<Transform> AvailableRoomList = new List<Transform>();
    public List<Transform> OccupiedRoomList = new List<Transform>();
    #endregion

    #region Room Management
    public void InitializeRooms(List<Transform> lockedRooms)
    {
        LockedRooms = new List<Transform>(lockedRooms);
        AvailableRoomList.Clear();
        OccupiedRoomList.Clear();
    }

    public Transform GetAvailableRoom()
    {
        if (AvailableRoomList.Count > 0)
        {
            return AvailableRoomList[0]; // Return the first available room
        }
        return null; // No available rooms
    }

    public void AddRoom(Transform room)
    {
        if (LockedRooms.Contains(room))
        {
            LockedRooms.Remove(room); // Remove from locked list
        }

        if (!AvailableRoomList.Contains(room) && !OccupiedRoomList.Contains(room))
        {
            AvailableRoomList.Add(room); // Add to available list
        }
    }

    public void AssignRoom(Transform room)
    {
        if (AvailableRoomList.Contains(room))
        {
            Debug.Log("room Assigned");
            AvailableRoomList.Remove(room); // Remove from available list
            OccupiedRoomList.Add(room); // Add to occupied list
        }
    }

    public void ReleaseRoom(Transform room)
    {
        if (OccupiedRoomList.Contains(room))
        {
            OccupiedRoomList.Remove(room); // Remove from occupied list
            AvailableRoomList.Add(room); // Add back to available list
        }
    }
    #endregion
}