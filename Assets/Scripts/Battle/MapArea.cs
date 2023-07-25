using System.Collections;
using System.Collections.Generic;
using NaughtyCharacter;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Pokemon> _wildPokemons;
    [HideInInspector]int _wildCounter = 0;
    public int WildsCounter => _wildCounter;
    int battleCount = 0;
    [SerializeField] BattleSystem _battleSystem;
    [SerializeField] RectTransform _MainMenu;
    [SerializeField] Camera GetCamera1, GetCamera2;
    [SerializeField] GameObject GetCameraRig,Ellen;
    [SerializeField] TestHelper _testHelper;
    //[SerializeField] NaughtyCharacter.PlayerInputComponent inputComponent;
    private void Awake()
    {
        Ellen.gameObject.SetActive(false);
    }
    public void CountReset()
    {
        battleCount = 0;
    }
    public Pokemon GetRandomWildPokemon()
    {
        //battleCount = 0;
        _wildCounter = Random.Range(0, _wildPokemons.Count);
        var wildpokemon = _wildPokemons[_wildCounter];
        wildpokemon.Init();
        return wildpokemon;
    }
    public void NextFightScene()
    {
        //GetCamera1.gameObject.SetActive(false);
        //GetCamera2.gameObject.SetActive(false);
        ////inputComponent.gameObject.SetActive(true);
        //_testHelper.gameObject.SetActive(true);
        //Ellen.gameObject.SetActive(true);
        //GetCameraRig.gameObject.SetActive(true);
        FindObjectOfType<UIManager>().OpenCardCollecter();
    }
    public bool MaxBattle()
    {
        if (battleCount >= 3)
        {
            return true;
        }
        else return false;
    }
    public int SetNewRandom()
    {
        int rdm=0;
        while(rdm != _wildCounter)
        {
            rdm = Random.Range(0, _wildPokemons.Count);
        }
        return rdm;
    }
    public void PrintBattle()
    {
        if (FindObjectOfType<BattleSystem>().isNullHP()) battleCount++;
        print(battleCount);
    }
    public Pokemon GetNextPokemon()
    {
        if (_wildPokemons.Count - 1 != _wildCounter)
        {
            if (MaxBattle()) { NextFightScene(); }
           
            _wildCounter = SetNewRandom();
            var wildpokemon = _wildPokemons[_wildCounter];
            wildpokemon.Init();
            //wildpokemon
            return wildpokemon;
        }
        else
        {
            if (MaxBattle()) { NextFightScene(); }
            //if (FindObjectOfType<BattleSystem>().isNullHP()) battleCount++;
            var wildpokemon = _wildPokemons[0];
            wildpokemon.Init();
            return wildpokemon;
        }
    }
}
