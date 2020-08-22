using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    //add more types of rooms here and randomize room generator
    public GameObject[] availableRooms;
    public List<GameObject> currentRooms;
    private float screenWidthInPoints;

    public GameObject[] availableObjects;
    public List<GameObject> objects;

    public float objectsMinDistance = 5.0f;
    public float objectsMaxDistance = 10.0f;

    public float objectsMinY = -1.4f;
    public float objectsMaxY = 1.4f;

    public float objectsMinRotation = -45.0f;
    public float objectsMaxRotation = 45.0f;

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
            GenerateObjectsIfRequired();
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

    void AddObject(float lastObjectX)
    {
        //1 Generates a random index to select a random object from the array. This can be a laser or one of the coin packs.
        int randomIndex = Random.Range(0, availableObjects.Length);
        //2 Creates an instance of the object that was just randomly selected.
        GameObject obj = (GameObject)Instantiate(availableObjects[randomIndex]);
        //3 Sets the object's position, using a random interval and a random height. This is controlled by script parameters.
        float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
        float randomY = Random.Range(objectsMinY, objectsMaxY);
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);
        //4 Adds a random rotation to the newly placed objects.
        float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        //5 Adds the newly created object to the objects list for tracking and ultimately, removal (when it leaves the screen).*/
        objects.Add(obj);
    }

    void GenerateObjectsIfRequired()
    {
        //1 Calculates key points ahead and behind the player. If the laser or coin pack is to the left of removeObjectsX, then it has already left the screen and is far behind. You will have to remove it. If there is no object after addObjectX point, then you need to add more objects since the last of the generated objects is about to enter the screen.The farthestObjectX variable is used to find the position of the last (rightmost) object to compare it with addObjectX.
        float playerX = transform.position.x;
        float removeObjectsX = playerX - screenWidthInPoints;
        float addObjectX = playerX + screenWidthInPoints;
        float farthestObjectX = 0;
        //2 Since you cannot remove objects from an array or list while you iterate through it, you place objects that you need to remove in a separate list to be removed after the loop.
        List<GameObject> objectsToRemove = new List<GameObject>();
        foreach (var obj in objects)
        {
            //3 This is the position of the object (coin pack or laser).
            float objX = obj.transform.position.x;
            //4 By executing this code for each objX you get a maximum objX value in farthestObjectX at the end of the loop (or the initial value of 0, if all objects are to the left of origin, but not in our case).
            farthestObjectX = Mathf.Max(farthestObjectX, objX);
            //5 If the current object is far behind, it is marked for removal to free up some resources.
            if (objX < removeObjectsX)
            {
                objectsToRemove.Add(obj);
            }
        }
        //6 Removes objects marked for removal.
        foreach (var obj in objectsToRemove)
        {
            objects.Remove(obj);
            Destroy(obj);
        }
        //7 If the player is about to see the last object and there are no more objects ahead, call the method to add a new object.
        if (farthestObjectX < addObjectX)
        {
            AddObject(farthestObjectX);
        }
    }
}
