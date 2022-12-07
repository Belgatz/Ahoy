using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class currentAmmo : MonoBehaviour
{
    public AudioClip pickupClip;


    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            //if (controller.ammo < controller.currentAmmo)
            {
                controller.ChangeAmmo(1);
                Destroy(gameObject);

                controller.PlaySound(pickupClip);
            }
        }
    }
}