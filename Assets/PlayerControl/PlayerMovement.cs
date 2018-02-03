using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour {

    public float Speed;
    public float Accel;
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;
    public GameObject bullet;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis
    public GameObject maincam;

    PlayerHealth playerHealth;
    Rigidbody rigid; 

	// Use this for initialization
	void Start () {
        playerHealth = GetComponent<PlayerHealth>();
        rigid = GetComponent<Rigidbody>();
        playerHealth.Respawn();
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;

        if (!isLocalPlayer) return;
        Cursor.lockState = CursorLockMode.Locked;
        maincam.SetActive(true);
        GameObject.FindGameObjectWithTag("TempCam").SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * Time.deltaTime;
        rotX += mouseY * mouseSensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(0.0f, rotY, 0.0f);
        Quaternion camRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        
        transform.rotation = localRotation;
        maincam.transform.rotation = camRotation;

        if (rigid.velocity.magnitude < Speed)
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");
            Vector3 forw = transform.forward * ver;
            forw.y = 0;
            Vector3 right = transform.right * hor;
            right.y = 0;
            rigid.AddForce((forw + right).normalized * Accel);
        }

        if (Input.GetKey("space"))
        {
            CmdShoot();
        }
    }

    [Command]
    void CmdShoot()
    {
        GameObject spawnedBullet = Instantiate(bullet);
        spawnedBullet.transform.position = transform.position + transform.forward.normalized * 2;
        spawnedBullet.GetComponent<Rigidbody>().velocity = maincam.transform.forward * 20f;
        spawnedBullet.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        NetworkServer.Spawn(spawnedBullet);
    }
}
