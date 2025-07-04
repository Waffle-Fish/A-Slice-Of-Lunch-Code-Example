using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PourSauce : MonoBehaviour
{
    [Header("Drip Settings")]
    public ParticleSystem dripParticles;
    public GameObject sauceSplatPrefab;
    public LayerMask pourSurfaceLayer;
    public float splatCooldown = 0.1f;

    private float splatTimer = 0f;
    private Camera mainCam;
    private Mouse mouse;
    private bool isPouring = false;

    void Awake()
    {
        mainCam = Camera.main;
        mouse = Mouse.current;
    }

    void Update()
    {
        HandleMovement();
        HandlePouring();
    }

    void HandleMovement()
    {
        if (mouse != null)
        {
            Vector2 mousePosition = mouse.position.ReadValue();
            Vector3 worldPosition = mainCam.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0f;
            transform.position = worldPosition;
        }
    }

    void HandlePouring()
    {
        isPouring = mouse != null && mouse.leftButton.isPressed;

        if (dripParticles != null)
        {
            var emission = dripParticles.emission;
            emission.enabled = isPouring;

            if (isPouring && !dripParticles.isPlaying)
                dripParticles.Play();
            else if (!isPouring && dripParticles.isPlaying)
                dripParticles.Stop();
        }

        if (isPouring)
        {
            splatTimer -= Time.deltaTime;
            if (splatTimer <= 0f)
            {
                TrySpawnSplat();
                splatTimer = splatCooldown;
            }
        }
    }

    void TrySpawnSplat()
    {
        Vector2 checkPosition = transform.position + Vector3.down * 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down, 0.2f, pourSurfaceLayer);

        if (hit.collider != null)
        {
            // GameObject splat = Instantiate(sauceSplatPrefab, hit.point, Quaternion.Euler(0, 0, Random.Range(0, 360f)));
            // splat.transform.localScale = Vector3.one;

            Quaternion randomRot = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            SaucePool.Instance.GetSplat(hit.point, randomRot);
        }
    }
}
