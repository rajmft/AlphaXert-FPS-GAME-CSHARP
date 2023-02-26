using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerMotor : MonoBehaviour
{

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;

    //Player UI
    public float health;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;

    //Player Stats
    public float speed = 7f;
    public float gravity = -15f;
    public float jumpHeight = 2f;
    public bool crouching = false;
    public float crouchTimer = 1;
    public bool lerpCrouch = false;
    public bool sprinting = false;


    //ShowingUI for pick up
    public LayerMask weaponLayerMask;
    public LayerMask pistolLayerMask;
    public Transform playerCameraTransform;
    public float hitRange = 3;
    private RaycastHit hit;
    public GameObject pickUpUI;

    //Weapon pickup
    public Transform pickUpParent;
    public Transform pickUpParentTwo;   
    public GameObject inHandItem;

   
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Clamp Health
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();
        if (Input.GetKeyDown(KeyCode.A))
        {
            TakeDamage(Random.Range(5,10));
        }



        //Movement
        isGrounded = controller.isGrounded;
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p += p;
            if (crouching)
                controller.height = Mathf.Lerp(controller.height, 1, p);
            else
                controller.height = Mathf.Lerp(controller.height, 2, p);

            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }

        }

        //raycasts if object is not present
        if (hit.collider != null)
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(false);
            pickUpUI.SetActive(false);
            

        }

        if (inHandItem != null)
        {
            return;
        }

        //raycasts if pistol is present
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, pistolLayerMask))
        {
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            pickUpUI.SetActive(true);
            return;
        }
        //raycasts if rifle is present
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out hit, hitRange, weaponLayerMask))
        {          
            hit.collider.GetComponent<Highlight>()?.ToggleHighlight(true);
            pickUpUI.SetActive(true);
        }

    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void Sprint()
    {
        sprinting = !sprinting;
        if (sprinting)
            speed = 15;
        else
            speed = 5;

    }


    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);

    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }

    }

    public void Interact()
    {
        if(hit.collider != null)
        {
            Debug.Log(hit.collider.name);
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (hit.collider.GetComponent<Weapon>())
            {
                inHandItem = hit.collider.gameObject;
                inHandItem.transform.position = Vector3.zero;
                inHandItem.transform.rotation = Quaternion.identity;
                inHandItem.transform.SetParent(pickUpParent, false);
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
                return;
            }
            if (hit.collider.GetComponent<WeaponPistol>())
            {
                inHandItem = hit.collider.gameObject;
                inHandItem.transform.position = Vector3.zero;
                inHandItem.transform.rotation = Quaternion.identity;
                inHandItem.transform.SetParent(pickUpParentTwo, false);
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
                return;
            }

        }

    }

    public void Drop()
    {
        if(inHandItem != null)
        {
            inHandItem.transform.SetParent(null);
            inHandItem = null;
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            return;

        }
    }

    public void UpdateHealthUI()
    {

        Debug.Log(health);
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
    
        }
    }


    public void TakeDamage(float amountDamage)
    {
        health -= amountDamage;
        lerpTimer = 0f;

        if (health < 0)
        {
            Die();

        }

    }

    void Die()
    {
        Debug.Log("You are dead");
    }

}

    
    
