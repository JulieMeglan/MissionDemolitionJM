using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    
    [Header("Set in Inspector")]
    public GameObject prefabProjectile;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public AudioClip snapSound;

    [Header("Set dynamically")]
    public GameObject launchPoint;

    public Vector3 launchPos;
    public GameObject projectile; 
    public bool aimingMode; 
    private Rigidbody projectileRigidbody;
    private LineRenderer lineRenderer;
    private AudioSource audioSource;


    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint"); 
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false); 
        launchPos = launchPointTrans.position;


        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.2f;
        lineRenderer.enabled = false;

        Color brown = new Color(0.65f, 0.16f, 0.16f, 1f);
        lineRenderer.startColor = brown;
        lineRenderer.endColor = brown;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = snapSound; // Set your snapping sound here
        audioSource.playOnAwake = false; // Don't play automatically
    }

    void Update () {

		if (!aimingMode) return;

		Vector3 mousePos2D = Input.mousePosition;
		mousePos2D.z = -Camera.main.transform.position.z;
		Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

		Vector3 mouseDelta = mousePos3D - launchPos;
		float maxMagnitude = this.GetComponent<SphereCollider>().radius;
		if (mouseDelta.magnitude > maxMagnitude) {
			mouseDelta.Normalize();
			mouseDelta *= maxMagnitude;
		}

		Vector3 projPos = launchPos + mouseDelta;
		projectile.transform.position = projPos;

        lineRenderer.SetPosition(0, launchPos); 
        lineRenderer.SetPosition(1, projectile.transform.position); 


        if (Input.GetMouseButtonUp(0)){
			aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;

            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot); 

            FollowCam.POI = projectile; 


            Instantiate<GameObject>(projLinePrefab, projectile.transform);
			projectile = null;
            MissionDemolition.SHOT_FIRED();

            lineRenderer.enabled = false;

            audioSource.Play();


        }

    }


    void OnMouseDown() {
        //the player has pressed the mouse button while over the slingshot
        aimingMode = true; 
        //instantiate a projectile
        projectile = Instantiate(prefabProjectile) as GameObject;
        //start at launchPoint
        projectile.transform.position = launchPos;
        // set to isKinematic for now
        // projectile.GetComponent<Rigidbody>().isKinematic = true; 
        projectileRigidbody = projectile.GetComponent<Rigidbody>(); 
        projectileRigidbody.isKinematic = true;

        lineRenderer.enabled = true;

    }
    // Start is called before the first frame update
    void OnMouseEnter()
    {
        launchPoint.SetActive(true);
        // print("Slingshot:OnMouseEnter()");
    }

    // Update is called once per frame
    void OnMouseExit()
    {
        launchPoint.SetActive(false);
        // print("Slingshot:OnMouseExit()");
    }
}
