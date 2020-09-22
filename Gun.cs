using UnityEngine;
using System.Collections;

//Currently this script is set up for first person. Third person shooting is not yet implemented
public class Gun : MonoBehaviour
{
    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public Animator animator;

    public bool hasScope; //Not yet implemented
    public bool isAutomatic;
    public bool isScattered; //Not yet implemented
    public float damage;
    public float range;
    public float fireRate;
    public int maxAmmo;
    public float reloadTime;
    public float bloom; 
    public int shotCount; //Shotguns set this to more than one, all other guns use 1 for this

    private int currentAmmo;
    private float fireCountdown = 0f;
    private bool isReloading = false;
    private bool isAiming = false; //Not yet implemented
    private bool isHoldingBreath = false; //Not yet implemented

    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
    }

    //Prevents weapon from being stuck in "reloading" if weapon is switched during reloading
    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if(isReloading)
        {
            return;
        }

        if(currentAmmo <= 0 || (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo))
        {
            StartCoroutine(Reload());
            return;
        }

        if (isAutomatic && Input.GetButton("Fire1") && fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
        if (!isAutomatic && Input.GetButtonDown("Fire1") && fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        //Associated bool not used currently
        if(Input.GetButtonDown("Fire2"))
        {
            isAiming = true;
        }
        if(Input.GetButtonUp("Fire2"))
        {
            isAiming = false;
        }

        //Associated bool not used currently
        if(isAiming && Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isHoldingBreath = true;
        }
        else
        {
            isHoldingBreath = false;
        }

        fireCountdown -= Time.deltaTime;
    }

    private void Shoot()
    {
        muzzleFlash.Play();

        currentAmmo--;

        for (int i = 0; i < Mathf.Max(1, shotCount); i++)
        {
            //Bloom stuff
            Vector3 weaponBloom = fpsCam.transform.position + fpsCam.transform.forward * 1000f;
            weaponBloom += Random.Range(-bloom, bloom) * fpsCam.transform.up;
            weaponBloom += Random.Range(-bloom, bloom) * fpsCam.transform.right;
            weaponBloom -= fpsCam.transform.position;
            weaponBloom.Normalize();

            //Raycast stuff
            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, weaponBloom, out hit, range))
            {
                Debug.Log(hit.transform.name);
                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 1.5f);
            }
            
            //Aiming and shotgun logic not yet implemented
            /*
            else if (isAiming && isScattered)
            {
                if (Physics.Raycast(fpsCam.transform.position, weaponBloom, out hit, range))
                {
                    Debug.Log(hit.transform.name);
                    GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactGO, 1.5f);
                }
            }
            else if (isAiming && !isScattered)
            {
                if (Physics.Raycast(fpsCam.transform.position, weaponBloom / 10, out hit, range))
                {
                    Debug.Log(hit.transform.name);
                    GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactGO, 1.5f);
                }
            }
            else if (isAiming && isScattered && isHoldingBreath)
            {
                if (Physics.Raycast(fpsCam.transform.position, weaponBloom / 1.2, out hit, range))
                {
                    Debug.Log(hit.transform.name);
                    GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactGO, 1.5f);
                }
            }
            else if (isAiming && !isScattered && isHoldingBreath)
            {
                if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
                {
                    Debug.Log(hit.transform.name);
                    GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impactGO, 1.5f);
                }
            }
            */
        }
    }

    //Coroutine used so that reloading isn't instantaneous
    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime - .25f);
        animator.SetBool("Reloading", false);
        yield return new WaitForSeconds(.25f);

        currentAmmo = maxAmmo;
        isReloading = false;
    }
}
