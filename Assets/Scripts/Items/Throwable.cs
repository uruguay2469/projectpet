﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Throwable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 force;
    public float multiplier = 5;
    private Vector3 pastPosition;
    private Vector3 actualPosition;
    private bool isDragged = false;
    private float distance = 0;
    private float copyGravityScale;
    private float copyDrag;
    private float copyMass;
    private float copyAngularDrag;
    private Rigidbody2D rb2D;

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameManager gameManager = GameManager.instance;
        TakeOutPhysics();
        gameManager.BlockSwipe = true;
        actualPosition = transform.position; //Primeira vez que pega a posição atual
        isDragged = true;
        StartCoroutine(ForceApplied());
    }


    /// <summary>
    /// Retira as propriedades físicas do objeto
    /// </summary>
    private void TakeOutPhysics()
    {
        rb2D = GetComponent<Rigidbody2D>();
        copyGravityScale = rb2D.gravityScale;
        copyDrag = rb2D.drag;
        copyMass = rb2D.mass;
        copyAngularDrag = rb2D.angularDrag;
        rb2D.gravityScale = 0;
        rb2D.drag = 0;
        rb2D.mass = 0;
        rb2D.angularDrag = 0;
        rb2D.velocity = new Vector2(0,0);
        rb2D.angularVelocity = 0;
    }

    /// <summary>
    /// Devolve as propriedas físicas que foram retiradas no TakeOutPhysics()
    /// </summary>
    private void GiveAgainPhysics()
    {
        rb2D.drag = copyDrag;
        rb2D.gravityScale = copyGravityScale;
        rb2D.mass = copyMass;
        rb2D.angularDrag = copyAngularDrag;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        transform.position = objPosition; //Fica atualizando o objeto para ele seguir o mouse
    }

    /// <summary>
    /// Enquanto a flag "isDragged" está ativa, ele calcula a posição atual e a posição em que o objeto estava a x segundos (antiga posição atual).
    /// </summary>
    /// <returns></returns>
    public IEnumerator ForceApplied()
    {
        while (isDragged)
        {
            pastPosition = actualPosition;
            actualPosition = transform.position;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void OnEndDrag (PointerEventData eventData)
    {
        GameManager gameManager = GameManager.instance;
        gameManager.BlockSwipe = false;
        force = new Vector2((actualPosition.x - pastPosition.x), (actualPosition.y - pastPosition.y));
        GiveAgainPhysics();
        ForceApplied();
        gameObject.GetComponent<Rigidbody2D>().AddForce(force * multiplier, ForceMode2D.Impulse);
        isDragged = false;
    }

}
