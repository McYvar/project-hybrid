using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicalMenuSelectorBall : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] float strength = 0;
    [SerializeField] float deadzone = 0;
    [SerializeField] float maxSpeed = 0;
    [SerializeField] float stoppingStrenght = 0;

    [SerializeField] Vector3 spawn;

    MenuItemTrigger currentMenuItem;

    Vector3 acceleration = Vector3.zero;

    LoadbarInstance targetLoadbar;

    float cooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        spawn = transform.position;
    }

    private void OnEnable()
    {
        transform.position = spawn;
        cooldownTimer = 4;
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }

    private void Update()
    {
        acceleration = new Vector3(SerialHandler.accX * strength, 0, SerialHandler.accZ * strength);
        if (acceleration.magnitude < deadzone) acceleration = Vector3.zero;

        if (targetLoadbar == null || currentMenuItem == null) return;
        if (targetLoadbar.confirm && cooldownTimer > 3)
        {
            currentMenuItem.OnBallCollsion.Invoke();
            targetLoadbar.confirm = false;
            targetLoadbar.isActive = false;
        }
        else if (cooldownTimer < 3)
        {
            cooldownTimer += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.1f) rb.AddForce(-rb.velocity * stoppingStrenght);
        if (rb.velocity.magnitude < maxSpeed) rb.AddForce(acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        MenuItemTrigger menuItem = other.GetComponent<MenuItemTrigger>();
        if (menuItem == null) return;

        targetLoadbar = menuItem.loadbar;
        targetLoadbar.isActive = true;
        targetLoadbar.waitTime = menuItem.waitTime;
        
        if (menuItem != currentMenuItem)
        {
            targetLoadbar.status = 0;
            currentMenuItem = menuItem;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other == null) return;
        MenuItemTrigger menuItem = other.GetComponent<MenuItemTrigger>();
        if (menuItem == null) return;

        targetLoadbar = menuItem.loadbar;
        targetLoadbar.isActive = false;
    }

}
