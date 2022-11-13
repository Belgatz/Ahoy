using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardEnemyController : MonoBehaviour
{
    public float speed = 3.0f;
    public float changeTime = 3.0f;
    public bool horizontal;

    public ParticleSystem smokeEffect;
    public ParticleSystem fixedParticleEffect;


    public AudioClip hitSound;
	public AudioClip fixedSound;

    Rigidbody2D rigidbody2d;
    float timer;
    Vector2 direction = Vector2.right;
    bool broken = true;
    

    Animator animator;
    AudioSource audioSource;

    private RubyController rubyController;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        direction = horizontal ? Vector2.right : Vector2.down;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            timer += changeTime;
            direction *= -1;
        }
        
        if(!broken)
        {
            return;
        }
        animator.SetFloat("ForwardX", direction.x);
		animator.SetFloat("ForwardY", direction.y);
    }
    
    void FixedUpdate()
    {
        if(!broken)
        {
            return;
        }
        rigidbody2d.MovePosition(rigidbody2d.position + direction * speed * Time.deltaTime);

	}
    
    void OnCollisionEnter2D(Collision2D other)
    {

        RubyController player = other.gameObject.GetComponent<RubyController>();

        if (player != null)
        {
            player.ChangeHealth(-2);
        }
    }
    

    public void Fix()
    {

        animator.SetTrigger("Fixed");

        smokeEffect.Stop();
  

		Instantiate(fixedParticleEffect, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        
        rigidbody2d.simulated = false;


        audioSource.Stop();
		audioSource.PlayOneShot(hitSound);
		audioSource.PlayOneShot(fixedSound);
    }
}