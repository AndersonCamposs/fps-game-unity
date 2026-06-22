using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float reloadTime = 1f;
    public float fireRate = 0.15f;
    public int magSize = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject bullet;
    public Transform bulletSpawnPoint;

    public GameObject weaponFlash;
    public GameObject droppedWeapon;

    public float recoilDistance = 0.1f;
    public float recoilSpeed = 15f;

    public AudioClip shootingSFX;
    

    private int currentAmmo = 0;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    private Quaternion initalRotation;
    private Vector3 initalPosition;
    private Vector3 reloadRotationOffset = new Vector3(66, 50, 50);

    void Start()
    {
        currentAmmo = magSize;
        initalRotation = transform.localRotation;
        initalPosition = transform.localPosition;
        UIManager.Instance.municaoTexto.text = currentAmmo.ToString();
    }

    public void Shoot()
    {
        if (isReloading) return;
        if (Time.time < nextTimeToFire) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        nextTimeToFire = Time.time + fireRate;
        currentAmmo--;
        if (UIManager.Instance != null && UIManager.Instance.municaoTexto != null)
        {
            UIManager.Instance.municaoTexto.text = currentAmmo.ToString();
        }
        else
        {
            Debug.LogError("UIManager ou municaoTexto está faltando na cena!");
        }

        AudioManager.Instance.PlaySFX(shootingSFX, 0.25f);

        Quaternion adjustRotation = bulletSpawnPoint.rotation * Quaternion.Euler(-6f, -3f, 0f);

        Instantiate(
            bullet,
            bulletSpawnPoint.position,
            adjustRotation
        );

        Instantiate(
            weaponFlash,
            bulletSpawnPoint.position,
            bulletSpawnPoint.rotation
        );

        StopCoroutine(nameof(Recoil));
        StartCoroutine(nameof(Recoil));
    }

    IEnumerator Reload()
    {
        isReloading = true;

        Quaternion targetRotation = Quaternion.Euler(initalRotation.eulerAngles + reloadRotationOffset);
        float halfReload = reloadTime / 2f;
        float t = 0f;

        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(
                initalRotation,
                targetRotation,
                t / halfReload
            );

            yield return null;
        }

        t = 0f;

        while (t < halfReload)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Slerp(
                targetRotation,
                initalRotation,
                t / halfReload
            );

            yield return null;
        }

        currentAmmo = magSize;
        UIManager.Instance.municaoTexto.text = currentAmmo.ToString();
        isReloading = false;
    }

    public void TryReload()
    {
        if (isReloading) return;
        if (currentAmmo == magSize) return;

        StartCoroutine(Reload());
    }

    private IEnumerator Recoil()
    {
        Vector3 recoilTarget = initalPosition + new Vector3(recoilDistance, 0, 0);
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed;
            transform.localPosition = Vector3.Lerp(initalPosition, recoilTarget, t);
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * recoilSpeed; ;
            transform.localPosition = Vector3.Lerp(recoilTarget, initalPosition, t);
            yield return null;
        }

        transform.localPosition = initalPosition;
    }
    
    public void Drop()
    {
        Instantiate(droppedWeapon, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
