﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Invisible : MonoBehaviour
{
    GameObject pet;
    Animator petAnimator;

    private void Awake()
    {
        pet = gameObject.transform.GetChild(0).gameObject;
        petAnimator = pet.GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Verifica se a imagem do pet deveria estar sendo mostrada. Caso não, desliga a imagem.
    /// </summary>
    public void StatusVerify()
    {
        if (pet.GetComponent<Pet>().GetPetLocation().sceneName.Equals(SceneManager.GetActiveScene().name))
        {
            pet.GetComponent<SpriteRenderer>().enabled = true;
            BackToOpaque(pet);
        }

        else
        {
            pet.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public IEnumerator PetChangeLocation(string scene)
    {
        pet.GetComponent<Pet>().SetPetScene(scene);
        petAnimator.Play("Invisible");
        yield return new WaitForSeconds(0.45f);
        petAnimator.SetTrigger("endInvisibleAnimation");
        pet.GetComponent<SpriteRenderer>().enabled = false; //.SetActive(false);
        StatusVerify();
    }

    private void BackToOpaque(GameObject pet)
    {
        Color petColor = pet.GetComponent<SpriteRenderer>().color;
        Color color = new Color(petColor.r, petColor.g, petColor.b, 1f);
        //Debug.Log(color);
        //Debug.Log(petColor);
        pet.GetComponent<SpriteRenderer>().color = color;
        
    }
}
