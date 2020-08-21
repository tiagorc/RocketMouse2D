using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    //add more types of rooms here and randomize room generator
    public GameObject[] availableRooms;
    public List<GameObject> currentRooms;
    private float screenWidthInPoints;
    // Start is called before the first frame update
    void Start()
    {
        float height = 2.0f * Camera.main.orthographicSize;
        screenWidthInPoints = height * Camera.main.aspect;

        StartCoroutine(GeneratorCheck());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //The while loop will ensure any code will continue to be executed whilst the game is running and the GameObject is active. Operations involving List<> can be performance limiting; therefore, a yield statement is used to add a 0.25 second pause in execution between each iteration of the loop. GenerateRoomIfRequired is only executed as often as it is required.

    private IEnumerator GeneratorCheck()
    {
        while (true)
        {
            GenerateRoomIfRequired();
            yield return new WaitForSeconds(0.25f);
        }
    }

    void AddRoom(float farthestRoomEndX)
    {
        //1 Picks a random index of the room type (Prefab) to generate.
        int randomRoomIndex = Random.Range(0, availableRooms.Length);
        //2 Creates a room object from the array of available rooms using the random index chosen above.
        GameObject room = (GameObject)Instantiate(availableRooms[randomRoomIndex]);
        //3 
        //Since the room is just an Empty GameObject containing all the room parts, you cannot simply take its size. 
        //Instead, you get the size of the floor inside the room, which is equal to the room’s width
        float roomWidth = room.transform.Find("floor").localScale.x;
        //4 In order to set the new room to its correct location, you need to calculate where its center should be. 
        //Take the furthest edge of the level so far, and add half of the new room’s width. 
        //By doing this, the new room will start exactly where the previous room ended.
        float roomCenter = farthestRoomEndX + roomWidth * 0.5f;
        //5 This sets the position of the room. 
        //You need to change only the x-coordinate since all rooms have the same y and z coordinates equal to zero.
        room.transform.position = new Vector3(roomCenter, 0, 0);
        //6 Finally, you add the room to the list of current rooms. 
        //It will be cleared in the next method, which is why you need to maintain this list.
        currentRooms.Add(room);
    }

    private void GenerateRoomIfRequired()
    {
        //1 Creates a new list to store rooms that need to be removed. A separate list is required since you cannot remove items from the list while you are iterating through it.
        List<GameObject> roomsToRemove = new List<GameObject>();
        //2 This is a flag that shows if you need to add more rooms. By default it is set to true, but most of the time it will be set to false inside the first foreach loop.
        bool addRooms = true;
        //3 Saves player position. (You'll mostly only use the x-coordinate when working with the mouse's position though).
        float playerX = transform.position.x;
        //4 This is the point after which the room should be removed. If room position is behind this point (to the left), it needs to be removed. You need to remove rooms, since you cannot endlessly generate rooms without removing them after they are not needed. Otherwise you will simply run out of memory.
        float removeRoomX = playerX - screenWidthInPoints;
        //5 If there is no room after the addRoomX point, then you need to add a room, since the end of the level is closer than the screen width.
        float addRoomX = playerX + screenWidthInPoints;
        //6 In farthestRoomEndX, you store the point where the level currently ends. You will use this variable to add a new room if required, since a new room should start at that point to make the level seamless.
        float farthestRoomEndX = 0;
        foreach (var room in currentRooms)
        {
            //7 In the foreach loop you simply enumerate currentRooms. You use the floor to get the room width and calculate the roomStartX (the point where the room starts, i.e. the leftmost point of the room) and roomEndX (the point where the room ends, i.e. the rightmost point of the room).
            float roomWidth = room.transform.Find("floor").localScale.x;
            float roomStartX = room.transform.position.x - (roomWidth * 0.5f);
            float roomEndX = roomStartX + roomWidth;
            //8 If there is a room that starts after addRoomX then you don’t need to add rooms right now. However there is no break instruction here, since you still need to check if this room needs to be removed.
            if (roomStartX > addRoomX)
            {
                addRooms = false;
            }
            //9 If the room ends to the left of the removeRoomX point, then it is already off the screen and needs to be removed.
            if (roomEndX < removeRoomX)
            {
                roomsToRemove.Add(room);
            }
            //10 Here you simply find the rightmost point of the level. This is the point where the level currently ends. It is used only if you need to add a room.
            farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX);
        }
        //11 This removes rooms that are marked for removal. The mouse GameObject already flew through them and thus they need to be removed.
        foreach (var room in roomsToRemove)
        {
            currentRooms.Remove(room);
            Destroy(room);
        }
        //12 If at this point addRooms is still true then the level end is near. addRooms will be true if it didn’t find a room starting farther than the screen's width. This indicates that a new room needs to be added.
        if (addRooms)
        {
            AddRoom(farthestRoomEndX);
        }
    }
}
