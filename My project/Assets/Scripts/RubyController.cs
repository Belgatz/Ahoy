using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public float speed = 3.0f;
    public int maxHealth = 5;
    public int health{ get { return currentHealth; } }
    int currentHealth;


    public ParticleSystem hitParticle;

    AudioSource audioSource;
    public AudioClip WinSound;
    public AudioClip LoseSound;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioSource backgroundmusic;
    
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    //Cog Stuff
    public GameObject projectilePrefab;
    public int ammo { get { return currentAmmo; }}
    public int currentAmmo;
    public TextMeshProUGUI ammoText;

    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    

    // Fixed Robots TMP Integers
    public TextMeshProUGUI fixedText;
    private int scoreFixed = 0;

    // Win text
    public GameObject WinTextObject;
    //Lose text
    public GameObject LoseTextObject;
    bool gameOver;
    public static int level;
    

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        currentAmmo = 4;

        rigidbody2d = GetComponent<Rigidbody2D>();
        AmmoText();

         // Fixed Robot Text
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/6";
        
        // Win Text
        WinTextObject.SetActive(false);
        LoseTextObject.SetActive(false);
        gameOver = false;
        level = 1;
    }

        public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        //Projectile
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
            if (currentAmmo > 0)
            {
                ChangeAmmo(-1);
                AmmoText();
            }
        }
        
        //Dialogue
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    character.DisplayDialog();
                }
            }
            if (scoreFixed >= 4)
            {
                SceneManager.LoadScene("Level 2");
                level = 2;
            }
        } 
    
     // Close
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        // Restart
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (gameOver == true)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // this loads the currently active scene
            }
        }
    }
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {   
        if (amount < 0)
        {
            if (isInvincible)
                return;
            isInvincible = true;
            invincibleTimer = timeInvincible;
 

            animator.SetTrigger("Hit");
            audioSource.PlayOneShot(hitSound);
            Instantiate(hitParticle, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }


        // Ruby loses all hp, lose text appears and restart becomes true
        if (currentHealth <= 1)
        {
            LoseTextObject.SetActive(true);
            gameOver = true;
            speed = 0;

            transform.position = new Vector3(-5f, 0f, -100f);
            Destroy(gameObject.GetComponent<SpriteRenderer>());
            backgroundmusic.Stop();
            audioSource.clip = LoseSound;
             audioSource.Play();
        }

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth); 
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    
    public void ChangeAmmo(int amount)
    {
        currentAmmo = Mathf.Abs(currentAmmo + amount);
        Debug.Log("Ammo: " + currentAmmo);
    }
    public void AmmoText()
    {
        ammoText.text = "Ammo: " + currentAmmo.ToString();

    }

    
    void Launch()
    {
        if(currentAmmo > 0)
        {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        audioSource.PlayOneShot(throwSound);
        }
    } 
     public void FixedRobots(int amount)
    {
        scoreFixed += amount;
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/6";
        Debug.Log("Fixed Robots: " + scoreFixed);
        // Win Text Appears
        if (scoreFixed >= 6)
        {
            WinTextObject.SetActive(true);
            gameOver = true;
            transform.position = new Vector3(-5f, 0f, -100f);
            speed = 0;

           backgroundmusic.Stop();
            audioSource.clip = WinSound;
            audioSource.Play();

            Destroy(gameObject.GetComponent<SpriteRenderer>());

        }
    }
}