using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public float speed;
    public float changeTime = 3.0f;
    public bool horizontal;

	public ParticleSystem smokeEffect;

	public AudioClip hitSound;
	public AudioClip fixedSound;
	
	Rigidbody2D rigidbody2d;
	float timer;
	Vector2 direction = Vector2.right;
    bool broken = true;
	
	Animator animator;
	AudioSource audioSource;

    
	
	private RubyController rubyController;


	void Start ()
	{
		rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
		direction = horizontal ? Vector2.right : Vector2.down;
		animator = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
        GameObject rubyControllerObject = GameObject.FindWithTag("Player");
        rubyController = rubyControllerObject.GetComponent<RubyController>();
	}
	
	void Update()
	{

        if(!broken)
        {
            return;
        }
        timer -= Time.deltaTime;
		
        if (timer < 0)
        {
            timer += changeTime;
            direction *= -1;
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

	void OnCollisionStay2D(Collision2D other)
	{
        //if(!broken)
        //return;
		RubyController controller = other.collider.GetComponent<RubyController>();
		
		if(controller != null)
			controller.ChangeHealth(-1);
	}

	public void Fix()
	{
        broken = false;
		animator.SetTrigger("Fixed");
        smokeEffect.Stop();

        if (rubyController != null)
        {   
            rubyController.FixedRobots(1);
        }
		//we don't want that enemy to react to the player or bullet anymore, remove its reigidbody from the simulation
		rigidbody2d.simulated = false;
		
		audioSource.Stop();
		audioSource.PlayOneShot(hitSound);
		audioSource.PlayOneShot(fixedSound);
	}
}
