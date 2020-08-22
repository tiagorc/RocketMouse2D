using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour
{
    //1 The Laser has two states: On and Off, and there is a separate image for each state.
    // You will specify each state image in just a moment.
    public Sprite laserOnSprite;
    public Sprite laserOffSprite;
    //2These properties allow you to add a bit of random fluctuation. 
    //You can set a different toggleInterval so that all lasers on the level don’t work exactly the same.
    // By setting a low interval, you create a laser that will turn on and off quickly, 
    //and by setting a high interval you will create a laser that will stay in one state for some time. 
    //The rotationSpeed variable serves a similar purpose and specifies the speed of the laser 
    public float toggleInterval = 0.5f;
    public float rotationSpeed = 0.0f;
    //3 These two private variables are used to toggle the laser’s state.
    private bool isLaserOn = true;
    private float timeUntilNextToggle;
    //4 These two private variables hold references to the laser collider and renderer so that their properties can be adjusted.
    private Collider2D laserCollider;
    private SpriteRenderer laserRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //1 This will set the time until the laser should toggle its state for the first time.
        timeUntilNextToggle = toggleInterval;
        //2 Here we save references to the collider and renderer as you will need to adjust their properties during their lifetime.
        laserCollider = gameObject.GetComponent<Collider2D>();
        laserRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //1 Decreases the time left until next toggle.
        timeUntilNextToggle -= Time.deltaTime;
        //2 If timeUntilNextToggle is equal to or less then zero, it is time to toggle the laser state.
        if (timeUntilNextToggle <= 0)
        {
            //3 Sets the correct state of the laser in the private variable.
            isLaserOn = !isLaserOn;
            //4 The laser collider is enabled only when the laser is on. This means that mouse can fly through the laser freely if it is off.
            laserCollider.enabled = isLaserOn;
            //5 Set the correct laser sprite depending on the laser state. This will display the laser_on sprite when the laser is on, and the laser_off sprite when the laser is Off.
            if (isLaserOn)
            {
                laserRenderer.sprite = laserOnSprite;
            }
            else
            {
                laserRenderer.sprite = laserOffSprite;
            }
            //6 Resets the timeUntilNextToggle variable since the laser has just been toggled.
            timeUntilNextToggle = toggleInterval;
        }
        //7 Rotates the laser around the z-axis using its rotationSpeed.
        transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);

        //Note: To disable rotation, you can simply set rotationSpeed to zero.
    }
}
