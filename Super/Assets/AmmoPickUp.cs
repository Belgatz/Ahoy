using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public AudioClip pickupClip;
    public GameObject AmmoUp;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            if (controller.ammo < controller.currentAmmo)
            {
                controller.ChangeAmmo(1);
                Instantiate(AmmoUp, transform.position, Quaternion.identity);
                Destroy(gameObject);

                controller.PlaySound(pickupClip);
            }
        }
    }
}