using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckCharacter : MonoBehaviour
{

    private Vector3 m_spawnPos;
    public bool isSwimmingDuck;

    public float WalkSpeed, RunSpeed;
    [Min(0.01f)]
    public float stopRunningUntilThisDist;
    public bool singleSprite, controlRotation, controlPosition, facingRight;

    public bool orient_towards_marker = false;
    public GameObject endMarkerPrefab;
    string check;
    float speed, distance;
    float distanceToMove = 0.5f;
    GameObject endMarker;
    bool idle;
    Animator animator;
    FiducialController endMarkerFC, fidu;
    SpriteRenderer spriteRenderer;
    bool debugMode = false;
    Vector3 startScale;

    void Start()
    {
        m_spawnPos = transform.position;

       if(singleSprite) spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        endMarker = SetEndMarker();
        fidu = GetComponent<FiducialController>();
        if(endMarker.name == "testMarker")
            debugMode = true;
        startScale = transform.localScale;
    }

    void EnableRenderersRecursively(Transform currentTransform, bool _enabled)
    {
        Renderer currentRenderer = currentTransform.gameObject.GetComponent<Renderer>();
        if (currentRenderer != null)
        {
            currentRenderer.enabled = _enabled;
        }

        foreach (Transform child in currentTransform)
        {
            EnableRenderersRecursively(child, _enabled);
        }
    }

    void Check_WaterLine()
    {
        if(endMarker == null)
            return;

        bool show = (isSwimmingDuck && endMarker.transform.position.y <= (m_spawnPos.y - 0.7f)) || (!isSwimmingDuck && endMarker.transform.position.y > (m_spawnPos.y - 0.7f));
        EnableRenderersRecursively(transform, show);

        if(!isSwimmingDuck)
        {
            float posY = Mathf.Clamp(transform.position.y, m_spawnPos.y - 0.3f, m_spawnPos.y);
            transform.position = new Vector3(transform.position.x, posY, transform.position.z);
        }
    }

    void Update()
    {
        Check_WaterLine();

        if ((debugMode || fidu.m_IsVisible) && animator == null) GetAnimator();
        CheckGrounded();
        if(orient_towards_marker) 
            OrientTowardsMarker();
        else
            Flip();
    }

    private void OrientTowardsMarker()
    {
        Vector3 newScale = startScale;
        transform.right = endMarker.transform.position - transform.position;
        if (endMarker.transform.position.x < transform.position.x)
            newScale.y*= -1.0f;
        transform.localScale = newScale;
        transform.Rotate(new Vector3(0, +180, 0));
    }

    private void CheckGrounded()
    {
        if (Vector3.Distance(transform.position, endMarker.transform.position) > distanceToMove) idle = false;
        if (transform.position == endMarker.transform.position) idle = true;
        if (!idle) transform.position = Vector3.MoveTowards(transform.position, endMarker.transform.position, speed * Time.deltaTime); //move to target

        SetAnimation(idle);
    }

    private void SetAnimation(bool idle)
    {
        distance = Vector3.Distance(transform.position, endMarker.transform.position);

        if (idle)
        {
            SetAnimationAndSpeed("idle", 0f);
            return;
        }

        if (distance > 0.1f && distance < stopRunningUntilThisDist) SetAnimationAndSpeed("walk", WalkSpeed);
        else if (distance > stopRunningUntilThisDist) SetAnimationAndSpeed("run", RunSpeed);
    }

    private void GetAnimator()
    {
        if (animator == null)
        {
            if (GetComponentInChildren<Animator>()) animator = GetComponentInChildren<Animator>();
            else animator = GetComponent<Animator>();
        }
    }

    private GameObject SetEndMarker()
    {
        GameObject tempEndMarker = Instantiate(endMarkerPrefab);
        tempEndMarker.name = gameObject.name + "_MARKER";
        endMarkerFC = tempEndMarker.GetComponent<FiducialController>();
        endMarkerFC.MarkerID = GetComponentInParent<FiducialController>().MarkerID;
        endMarkerFC.IsRotationMapped = controlRotation;
        endMarkerFC.IsPositionMapped = controlPosition;
        return tempEndMarker;
    }


    public void SetAnimationAndSpeed(string animation, float setSpeed)
    {
        if (animation != check)
        {
            speed = setSpeed;
            if (!animator) return;
            if(debugMode || fidu.m_IsVisible) 
            {
                animator.Play(animation);
            }
            check = animation;
        }
    }

    private void Flip()
    {
        if (endMarker.transform.position.x - 0.05f >= transform.position.x && facingRight) Rotate();
        else if (endMarker.transform.position.x + 0.05f <= transform.position.x && !facingRight) Rotate();
    }
    void Rotate()
    {
        if (singleSprite)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
            return;
        }
        facingRight = !facingRight;
        transform.Rotate(new Vector3(0, +180, 0));
    }
}
