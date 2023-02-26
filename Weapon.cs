using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //ADS
    private Vector3 hipPosition;
    public Vector3 aimPosition;
    public float aodSpeed;

    //Shooting
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15;
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;

    //blood effect
    public ParticleSystem bloodEffect;

    public GameObject impactEffect;
    public float impactForce = 30f;
    private float nextTimeToFire = 0f;
    private bool aiming;

    //Recoil
    public Recoil recoil;
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstraints;
    public Vector2 recoilPattern;


    //Procedural Recoil (cams)
    public Recoil Recoil_Script;
    public GameObject cameras;
    

    //Mouse
    public float mouseSensitivity = 1;
    Vector2 currentRotation;

    //enable weapon camera if equipped
    public Camera weaponCamera;
    public Camera mainCamera;

    //Different Game Objects GUNS
    public GameObject weaponOne;
   
  
    // Start is called before the first frame update
    void Start()
    {
        
        
            Cursor.visible = false;
           // hipPosition = transform.localPosition;

            //Procedural Recoil
            cameras = GameObject.Find("CameraRecoil");
            Recoil_Script = cameras.transform.GetComponent<Recoil>();

        

    }

    // Update is called once per frame
    void Update()
    {

        InHand();

        //button call for shooting, check if gun is child of camera
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && transform.parent != null)
        {
            
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            Recoil_Script.RecoilFire();
        }

    }

    


    public void AimDownSights()
    {
        if (Input.GetButton("Fire2"))
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimPosition, Time.deltaTime * aodSpeed);

        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, hipPosition, Time.deltaTime * aodSpeed);

        }
    }

    void Shoot()
    {
        //muzzle flash effect
        muzzleFlash.Play();

        //recoil.recoil();
        Recoil();
        
        //hitting and force
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            
            EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();
            
           
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                muzzleFlash.Play();
            }

            //if (dummy != null)
            {
                //dummy.TakeDamage(damage);
                //muzzleFlash.Play();
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));

            Destroy(impactGO, 2f);

        }
    }


    void Recoil()
    {

        transform.localPosition -= Vector3.forward * 0.1f;

        if (randomizeRecoil)
        {

            float recoilX = Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
            float recoilY = Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);

            Vector2 recoil = new Vector2(recoilX, recoilY);
            currentRotation += recoil;
        }
    }

    void InHand()
    {
        //check if gun is child of camera
        if (weaponOne.transform.parent != null)
        {

            //removes weapon from culling mask of normal camera and enables weapon camera
            mainCamera.cullingMask &= ~(1 << 6);
            weaponCamera.enabled = true;

            AimDownSights();
        }
        else
        {
            mainCamera.cullingMask |= (1 << 6);
            weaponCamera.enabled = false;
        }



    }

    

}

