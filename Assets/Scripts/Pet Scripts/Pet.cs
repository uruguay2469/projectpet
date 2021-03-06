﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Pet : MonoBehaviour
{
    public ElementLocation petCurrentLocation;
    private GameObject poop;
    private GameObject pee;
    private PetMovement petAnimationScript;
    private PetBasicAI petMovement;

    public WarningsList[] warningsLists;

    [Tooltip("Indica quantas vezes o pet bebe água em cada periodo")]
    public int[] drinkTimes;

    public void Start()
    {
        petAnimationScript = gameObject.GetComponentInChildren<PetMovement>();
        petMovement = gameObject.GetComponentInParent<PetBasicAI>();
    }

    public ElementLocation GetPetLocation()
    {
        return petCurrentLocation;
    }

    public void SetPetLocation(ElementLocation newPetLocation)
    {
        petCurrentLocation = newPetLocation;
    }

    public void SetPetScene(string sceneName)
    {
        Debug.Log(sceneName);
        petCurrentLocation.sceneName = sceneName;
    }

    /// <summary>
    /// Função que toca a animação de comida e depois chama a função Eat
    /// </summary>
    /// <returns></returns>
    public IEnumerator Eat()
    {
        ///Play Animation

        SaveManager saveManager = SaveManager.instance;
        Eat(saveManager.player.foodPot.content);
        // Informa que o pet acabou de fazer sua ação
        StartCoroutine(petMovement.IsPetDoingSometingSetFalse());
        petMovement.SetHungryOnDelegateBool(false);

        PopUpWarning.instance.SolveWarning("CleanFoodPot");
        saveManager.player.SetFlagsTimeHelper();
        saveManager.player.SetTimeLastMeal();
        yield return new WaitUntil(() => (saveManager.player.timeHelper.betweenMealAndPeriod && saveManager.player.timeHelper.betweenMealAndTimeLimit));
        TimeManager.instance.PeriodProcess();

        yield return new WaitForSeconds(0);
    }

    /// <summary>
    /// Adiciona os valores de food em poop e hungry
    /// </summary>
    /// <param name="food"></param>
    public void Eat(Food food)
    {
        SaveManager.instance.player.health.PutInCleanFoodPot(false);
        //SaveManager.instance.player.health.PutInPoop(food.GetNutrionalValor() / 2);
        Debug.Log("comi");
    }

    /// <summary>
    /// Função que toca a animação de comida e depois chama a função Drink
    /// </summary>
    /// <returns></returns>
    public IEnumerator Drink()
    {
        ///Play Animation
        Drink(SaveManager.instance.player.waterPot.content);
        // Informa que o pet acabou de fazer sua ação
        StartCoroutine(petMovement.IsPetDoingSometingSetFalse());
        petMovement.SetThirstyOnDelegateBool(false);
        yield return new WaitForSeconds(0);
    }

    /// <summary>
    /// Adiciona os valores de food em pee e thirsty
    /// </summary>
    /// <param name="food"></param>
    public void Drink(Food food)
    {
        SaveManager.instance.player.health.PutInCleanWaterPot(true); // food.GetNutrionalValor());
        //SaveManager.instance.player.health.PutInPee(food.GetNutrionalValor() / 2);
        Debug.Log("bebi");
    }

    /// <summary>
    /// É chamada para o pet ir atrás da bolinha
    /// </summary>
    public void Play()
    {

    }

    /// <summary>
    /// Instancia o pee do pet no mesmo lugar em que ele está no momento
    /// </summary>
    /// <returns></returns>
    public IEnumerator PeeOnLocation()
    {
        pee = Resources.Load("Prefabs/Items/Pee") as GameObject;
        Vector3 position = new Vector3(transform.position.x, transform.position.y - GetComponent<Renderer>().bounds.size.y / 2, transform.position.z - 5); //Eixo Z tem que ser menor para ficar mais perto da câmera e ativar o OnMouseDown()
        if (PetIsInScene())
        {
            Instantiate(pee, position, Quaternion.identity);
        }
        SaveManager.instance.player.health.PutInPee(-0.5f); //Esvazia pela metade a vontade do animal de fazer xixi
        SaveManager.instance.player.peeLocation.Add(petCurrentLocation.sceneName, position);
        // Informa que o pet acabou de fazer sua ação
        StartCoroutine(gameObject.GetComponentInParent<PetBasicAI>().IsPetDoingSometingSetFalse());
        yield return new WaitForSeconds(0);
    }

    /// <summary>
    /// Instancia o poop do pet no mesmo lugar em que ele está no momento
    /// </summary>
    /// <returns></returns>
    public IEnumerator PoopOnLocation()
    {
        poop = Resources.Load("Prefabs/Items/Poop") as GameObject;
        Vector3 position = new Vector3(transform.position.x, transform.position.y - GetComponent<Renderer>().bounds.size.y / 2, transform.position.z - 5);
        if (PetIsInScene())
        {
            Instantiate(poop, position, Quaternion.identity);
        }
        SaveManager.instance.player.health.PutInPoop(-0.5f); //Esvazia pela metade a vontade do animal de fazer cocô
        SaveManager.instance.player.poopLocation.Add(petCurrentLocation.sceneName, position);
        // Informa que o pet acabou de fazer sua ação
        StartCoroutine(gameObject.GetComponentInParent<PetBasicAI>().IsPetDoingSometingSetFalse());
        yield return new WaitForSeconds(0);
    }

    /// <summary>
    /// Retorna se o pet está na mesma scene que o jogador
    /// </summary>
    /// <returns>Retorna verdadeiro se está, e falso caso não esteja</returns>
    public bool PetIsInScene()
    {
        return petCurrentLocation.sceneName.Equals(SceneManager.GetActiveScene().name);
    }
}
