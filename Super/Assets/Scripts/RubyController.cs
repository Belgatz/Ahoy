using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public static int level = 1;
    public int maxHealth = 5;
    public int health{ get { return currentHealth; } }
    int currentHealth;
    int cogs;

    public ParticleSystem hitParticle;


// Speed Boost Timer
    public float timeBoosting = 4.0f;
    float speedBoostTimer;
    bool isBoosting;

//Audio
    AudioSource audioSource;
    public AudioClip WinSound;
    public AudioClip LoseSound;
    public AudioClip throwSound;
    public AudioClip hitSound;
    public AudioClip tickingSound;
    public AudioSource backgroundmusic;
    
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;

    //Cog Stuff Projectile
    public GameObject projectilePrefab;
    public int ammo { get { return currentAmmo; }}
    public int currentAmmo;
    public TextMeshProUGUI ammoText;

    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    public float speed = 3.0f;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    

    // Fixed Robots TMP Integers
    public TextMeshProUGUI fixedText;
    private int scoreFixed = 0;

    // Win text
    public GameObject WinTextObject;
    //Lose text
    public GameObject LoseTextObject;
    public bool gameOver= false;
    //private bool gameOver= false;
       
        // Timer Variables
    [SerializeField] private TextMeshProUGUI timerUI; // Attach TMP object to this slot
    [SerializeField] private float mainTimer; // Change this value to your liking in Unity

    private float timer;
    private bool canCount = false;
    private bool doOnce = false;
    private bool hasPressedKey = false;
    private bool hasMoved = false;

    public GameObject TimerObject; // This is your Timer text in the Canvas


    

    // Start is called before the first frame update
    void Start()
    {

        // Framerate and VSync Code
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        
        //Ammo
        rigidbody2d = GetComponent<Rigidbody2D>();
        AmmoText();

         // Fixed Robot Text
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/4";
        
        // Win Text
        WinTextObject.SetActive(false);
        LoseTextObject.SetActive(false);
        gameOver = false;

    

        // Timer
        timer = mainTimer;
        TimerObject.SetActive(false);
       
       //Level
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

              hasMoved = true;
        }

         // Speed Boost Timer
        if (isBoosting == true)
        {
            speedBoostTimer -= Time.deltaTime; // Once speed boost activates, it counts down
            speed = 8;
        
            if (speedBoostTimer < 0)
            {
                isBoosting = false;
                speed = 5; 
            }
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

        //AmmoPickup


        //Dialogue
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {

                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if (scoreFixed >= 4)
                    {
                        SceneManager.LoadScene("Level 2");
                        level = 2;
                    }
                
                    else
                    {
                    character.DisplayDialog();
                    }
                }
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

            if (gameOver == true && level == 1)
            {
                SceneManager.LoadScene("Level1");
                level = 1;
            }
        }

                if (hasMoved == false)
        {
            if (hasPressedKey == false)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    timer = mainTimer;
                    canCount = true;
                    doOnce = false;
                    TimerObject.SetActive(true);

                    // Ticking Sound Starts
                    PlaySound(tickingSound);
            
                    hasPressedKey = true;
                }
            }
        }

        // Timer functionality
        if (timer >= 0.0f && canCount)
        {
            timer -= Time.deltaTime;
            timerUI.text = "Time: " + timer.ToString("F");
        }

        else if (timer <= 0.0f && !doOnce)
        {
            canCount = false;
            doOnce = true;
            timerUI.text = "Time: " + timer.ToString("0.00");

            // Lose State
            LoseTextObject.SetActive(true);

            transform.position = new Vector3(-5f, 0f, -100f);
            speed = 0;
            Destroy(gameObject.GetComponent<SpriteRenderer>());

            gameOver = true;

            // BackgroundMusicManager is turned off
            backgroundmusic.Stop();

            audioSource.clip = LoseSound;
             audioSource.Play();

            // Ticking Sound Stops
            StopSound(tickingSound);
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
            if (level == 1)
            {
                WinTextObject.SetActive(false);
            }
            gameOver = true;
            speed = 0;

            transform.position = new Vector3(-5f, 0f, -100f);
            Destroy(gameObject.GetComponent<SpriteRenderer>());
            backgroundmusic.Stop();
            audioSource.clip = LoseSound;
             audioSource.Play();


            // Disables time attack on level
            canCount = false;
            doOnce = true;

            // Ticking Sound Stops
            StopSound(tickingSound);
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

        public void StopSound(AudioClip clip)
    {
        audioSource.Stop();
    }
    
 public void FixedRobots(int amount)
    {
        scoreFixed += amount;
        fixedText.text = "Fixed Robots: " + scoreFixed.ToString() + "/4";

        if (scoreFixed == 4 && level == 1)
        {
             WinTextObject.SetActive(true);

            // Disables time attack on level
            canCount = false;
            doOnce = true;

        
        // Ticking Sound Stops
            StopSound(tickingSound);
          //  NewScene.SetActive(true);
        }


        // Win Text Appears ONLY if on Level 2
        else if (scoreFixed == 4 && level == 2)
        {
            WinTextObject.SetActive(true);
            gameOver = true;
            gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Destroy(gameObject.GetComponent<BoxCollider2D>());

            backgroundmusic.Stop();
            PlaySound(WinSound);


            // Disables time attack on level
            canCount = false;
            doOnce = true;

            // Ticking Sound Stops
            StopSound(tickingSound);
        }
    }

    public void SpeedBoost(int amount)
    {
        if (amount > 0)
        {
            speedBoostTimer = timeBoosting;
            isBoosting = true;
        }
    }
}