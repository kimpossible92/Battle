using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using TMPro;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, AboutToUse, BattleOver }
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }
//public class BattleSprite{ public string Named; public int sprited;}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit _playerUnit;
    [SerializeField] BattleUnit _enemyUnit;
    [SerializeField] BattleDialogBox _dialogBox;
    [SerializeField] PartyScreen _partyScreen;
    [SerializeField] Image _playerImage, _trainerImage;
    [SerializeField] GameObject _pokeballSprite;
    [SerializeField] GameObject firstEffect, secondEffect;
	//[SerializeField] List<BattleSprite> _battleSprites;
    public event Action<bool> OnBattleOver;
    BattleState state;
    BattleState? prevState;
    int _currentAction;
    int _currentMove;
    int _currentMember;
    bool _aboutToUseChoice = true;
    PokemonParty _playerParty;
    PokemonParty _trainerParty;
    Pokemon _wildPokemon;
    bool _isTrainerBattle = false;
    PlayerControll _player;
    TrainerController _trainer;
    [SerializeField] Text GetTextPlayer;
    [SerializeField] Image HandleActionSelectionPanel, HandleMoveSelectionPanel, HandleParySelectionPanel, HandleAboutToUsePanel;
    [SerializeField] GameObject FightBtns, FightBtns1, FightBtns2;
	public List<Sprite> _battleSprite;
    private void Start()
    {
        //_player = FindObjectOfType<PlayerControll>();
        //_player.OnOver();
        GameCC.Instance.SetCrtl();
        SetActionSelect();
    }
    private void OnEnable()
    {
        
    }
    public void SetPanel()
    {

    }

    int escapeAttempts;
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this._playerParty = playerParty;
        this._wildPokemon = wildPokemon;
        _player = playerParty.GetComponent<PlayerControll>();
        _isTrainerBattle = false;

        StartCoroutine(SetupBattle());

    }
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this._playerParty = playerParty;
        this._trainerParty = trainerParty;

        _isTrainerBattle = true;
        _player = playerParty.GetComponent<PlayerControll>();
        _trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }
    public IEnumerator SetupBattle()
    {
        _playerUnit.Clear();
        _enemyUnit.Clear();
        if (!_isTrainerBattle)
        {
            //wild pokemon battle
            _playerUnit.Setup(_playerParty.GetHealtyPokemon());
            _enemyUnit.Setup(_wildPokemon);
            _dialogBox.SetMoveNames(_playerUnit.Pokemon.Moves);
            // _dialogBox.SetDialog($"A wild {_playerUnit.Pokemon.Base.Name} appeared.  ");
            yield return _dialogBox.TypeDialog($"A wild {_enemyUnit.Pokemon.Base.Name} appeared.  ");
        }
        else
        {
            //Trainer Battle

            //show trainer and player sprites
            _playerUnit.gameObject.SetActive(false);
            _enemyUnit.gameObject.SetActive(false);

            _playerImage.gameObject.SetActive(true);
            _trainerImage.gameObject.SetActive(true);
            _playerImage.sprite = _player.Sprite;
            _trainerImage.sprite = _trainer.Sprite;

            yield return _dialogBox.TypeDialog($"{_trainer.Name} wants to Battle");

            Debug.Log($"<color=blue>send out first pokemon of the trainer </color>");

            _trainerImage.gameObject.SetActive(false);
            _enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = _trainerParty.GetHealtyPokemon();
            _enemyUnit.Setup(enemyPokemon);
            yield return _dialogBox.TypeDialog($"{_trainer.Name} send out {enemyPokemon.Base.Name}");
            //Send out fisrt pokemon of the player
            _playerImage.gameObject.SetActive(false);
            _playerUnit.gameObject.SetActive(true);
            var playerPokemon = _playerParty.GetHealtyPokemon();
            _playerUnit.Setup(playerPokemon);
            yield return _dialogBox.TypeDialog($"Go {playerPokemon.Base.Name} !");
            _dialogBox.SetMoveNames(_playerUnit.Pokemon.Moves);
        }


        escapeAttempts = 0;
        _partyScreen.Init();


        ActionSelection();
    }
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        _playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        _dialogBox.SetDialog("Choose an action");
        _dialogBox.EnableActionSelector(true);
    }
    void OpenPartyScreen()
    {
        state = BattleState.PartyScreen;
        _partyScreen.SetPartyData(_playerParty.Pokemons);
        _partyScreen.gameObject.SetActive(true);
    }
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        _dialogBox.EnableActionSelector(false);
        _dialogBox.EnableDialogText(false);
        _dialogBox.EnableMoveSelector(true);
    }
    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        state = BattleState.Busy;
        yield return _dialogBox.TypeDialog($"{_trainer.Name} is about to use {newPokemon.Base.Name} . Do you want to change pokemon?");

        state = BattleState.AboutToUse;
        _dialogBox.EnableChoiceBox(true);
    }
    private IEnumerator Reload(float cooldown,BattleUnit firstUnit)
    {
        firstEffect.SetActive(true);
		secondEffect.GetComponent<Image>().sprite = _playerUnit.Pokemon.CurrentMove.Base.MovedsSprite;
        firstUnit.GetComponent<Image>().sprite = firstUnit.Pokemon.Base.GetSprites[1];
        firstUnit.GetComponent<Animator>().enabled = false;
        Vector2 OldPos = firstUnit.GetComponent<Image>().rectTransform.anchoredPosition;
        firstUnit.GetComponent<Image>().rectTransform.anchoredPosition = firstEffect.GetComponent<RectTransform>().anchoredPosition - new Vector2(300, 0);
        yield return new WaitForSeconds(cooldown); 
        firstUnit.GetComponent<Image>().sprite = firstUnit.Pokemon.Base.GetSprites[0];
        firstUnit.GetComponent<Animator>().enabled = true;
        firstEffect.SetActive(false);
        firstUnit.GetComponent<Image>().rectTransform.anchoredPosition = OldPos;
    }
    public void SpawnNewEffect()
    {

    }
    Vector2 OldPos2;
    private void Reload2(float cooldown, BattleUnit firstUnit)
    {
        firstUnit.GetComponent<Image>().sprite = firstUnit.Pokemon.Base.GetSprites[1];
        firstUnit.GetComponent<Animator>().enabled = false;
        OldPos2 = firstUnit.GetComponent<Image>().rectTransform.anchoredPosition;
        firstUnit.GetComponent<Image>().rectTransform.anchoredPosition = firstUnit.GetComponent<Image>().rectTransform.anchoredPosition + new Vector2(300, 0);
        //yield return new WaitForSeconds(cooldown);

    }
    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;
        //print(playerAction);
        if (playerAction == BattleAction.Move)
        {
            _playerUnit.Pokemon.CurrentMove = _playerUnit.Pokemon.Moves[_currentMove];
            _enemyUnit.Pokemon.CurrentMove = _enemyUnit.Pokemon.GetRandomMove();

            int playerMovepriorty = _playerUnit.Pokemon.CurrentMove.Base.Priority;
            int enemyMovepriorty = _enemyUnit.Pokemon.CurrentMove.Base.Priority;

            //  Check who goes first

            
            bool playerGoesFirst = true;
            if (enemyMovepriorty > playerMovepriorty)
                playerGoesFirst = false;
            else if (enemyMovepriorty == playerMovepriorty)
                playerGoesFirst = _playerUnit.Pokemon.Speed >= _enemyUnit.Pokemon.Speed;
            playerGoesFirst = _playerUnit.Pokemon.Speed >= _enemyUnit.Pokemon.Speed;

            var firstUnit = (playerGoesFirst) ? _playerUnit : _enemyUnit;
            var secondUnit = (playerGoesFirst) ? _enemyUnit : _playerUnit;
            var secondPokemon = secondUnit.Pokemon;
            //print(playerAction+"moved2");
            //First Turn
            StartCoroutine(Reload(1f, firstUnit));
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;
            //firstUnit.GetComponent<Image>().sprite = firstUnit.Pokemon.Base.GetSprites[0];

            if (secondPokemon.HP > 0)
            {
                //Second Turn
                //print(secondPokemon.HP);
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = _playerParty.Pokemons[_currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                _dialogBox.EnableActionSelector(false);
                yield return ThrowPokeball();
            }
            else if (playerAction == BattleAction.Run)
            {


                yield return TryToEscape();
            }

            
            //enemyTurn
            var enemyMove = _enemyUnit.Pokemon.GetRandomMove();
            
            yield return RunMove(_enemyUnit, _playerUnit, enemyMove);
            yield return RunAfterTurn(_enemyUnit);
            if (state == BattleState.BattleOver) yield break;

        }
        if (state != BattleState.BattleOver)
            ActionSelection();

    }
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        if (sourceUnit == _enemyUnit)
        {
            Reload2(1f, _enemyUnit);
        }
        bool canRunMove = sourceUnit.Pokemon.OnBeforeMove();
        //sourceUnit.Pokemon.Base.GetSprites[0];
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Pokemon);
            yield return sourceUnit.Hud.UpdateHp();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        //print(move.Base.Name);
        //StartCoroutine(Reload(0.4f, _playerUnit));
        move.PP--;
        //yield return _dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");
        
        if (CheckIfMoveHits(move, sourceUnit.Pokemon, targetUnit.Pokemon))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();
            //print("CheckIfMoveHits");
            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffeckts(move.Base.Efeckts, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHp();
                yield return ShowDamageDetails(damageDetails);
            }
            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffeckts(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            }



            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
        }
        else
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.PlayHitAnimation();
            //print("CheckIfMoveHits");
            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffeckts(move.Base.Efeckts, sourceUnit.Pokemon, targetUnit.Pokemon, move.Base.Target);
            }
            else
            {
                var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
                yield return targetUnit.Hud.UpdateHp();
                yield return ShowDamageDetails(damageDetails);
            }
            if (move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Pokemon.HP > 0)
            {
                foreach (var secondary in move.Base.Secondaries)
                {
                    var rnd = UnityEngine.Random.Range(1, 101);
                    if (rnd <= secondary.Chance)
                        yield return RunMoveEffeckts(secondary, sourceUnit.Pokemon, targetUnit.Pokemon, secondary.Target);
                }
            }



            if (targetUnit.Pokemon.HP <= 0)
            {
                yield return HandlePokemonFainted(targetUnit);
            }
            //yield return _dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name}'s attack missed");
        }
        if (sourceUnit == _enemyUnit)
        {
            _enemyUnit.GetComponent<Image>().sprite = _enemyUnit.Pokemon.Base.GetSprites[0];
            _enemyUnit.GetComponent<Animator>().enabled = true; _enemyUnit.GetComponent<Image>().rectTransform.anchoredPosition = OldPos2;
        }
    }

    IEnumerator RunMoveEffeckts(MoveEfeckts efeckts, Pokemon source, Pokemon target, Movetarget moveTarget)
    {

        //Stat boosting
        if (efeckts.Boosts != null)
        {
            if (moveTarget == Movetarget.Self)
                source.ApplyBoosts(efeckts.Boosts);
            else
                target.ApplyBoosts(efeckts.Boosts);
        }
        //Status Condition
        if (efeckts.Status != ConditionID.none)
        {
            target.SetStatus(efeckts.Status);
        }
        //Volatile Status Condition
        if (efeckts.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(efeckts.VolatileStatus);
        }
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);
    }
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {
        if (state == BattleState.BattleOver) yield break;
        yield return new WaitUntil(() => state == BattleState.RunningTurn);


        //Statues like burn or psn will hurt the pokemon after the turn
        sourceUnit.Pokemon.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Pokemon);
        yield return sourceUnit.Hud.UpdateHp();
        if (sourceUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    bool CheckIfMoveHits(Move move, Pokemon source, Pokemon target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuary;
        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];


        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return UnityEngine.Random.Range(1, 101) <= moveAccuracy;
    }
    IEnumerator ShowStatusChanges(Pokemon pokemon)
    {
        while (pokemon.StatusChanges.Count > 0)
        {
            var message = pokemon.StatusChanges.Dequeue();
            yield return _dialogBox.TypeDialog(message);
        }
    }

    public bool isNullHP()
    {
        if(_playerUnit.Pokemon.HP <=1|| _enemyUnit.Pokemon.HP <= 1)
        {
            return true;
        }
        //print(_playerUnit.Pokemon.HP);
        //print(_enemyUnit.Pokemon.HP);
        return false;
    }
    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return _dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} Fainted");
        faintedUnit.PlayFaintAnimation();

        yield return new WaitForSeconds(2f);

        if (!faintedUnit.IsplayerUnit)
        {
            //Exp Gain 
            int expyield = faintedUnit.Pokemon.Base.ExpYield;
            int enemylevel = faintedUnit.Pokemon.Level;
            float trainerBonus = (_isTrainerBattle) ? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expyield * enemylevel * trainerBonus) / 7);
            _playerUnit.Pokemon.Exp += expGain;
            yield return _dialogBox.TypeDialog($"{_playerUnit.Pokemon.Base.Name} gainted  {expGain} exp");
            yield return _playerUnit.Hud.SetExpSmooth();
            //Check level up

            while (_playerUnit.Pokemon.CheckForLevelUp())
            {
                _playerUnit.Hud.SetLevel();
                yield return _dialogBox.TypeDialog($" {_playerUnit.Pokemon.Base.Name} grew to level {_playerUnit.Pokemon.Level}");
                yield return _playerUnit.Hud.SetExpSmooth(true);
            }
            yield return new WaitForSeconds(1f);
        }
        CheckForBattleOver(faintedUnit);
    }
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsplayerUnit)
        {
            var nextPokemon = _playerParty.GetHealtyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!_isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextPokemon = _trainerParty.GetHealtyPokemon();
                if (nextPokemon != null)
                {
                    StartCoroutine(AboutToUse(nextPokemon));
                }
                else
                    BattleOver(true);
            }

        }
    }
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return _dialogBox.TypeDialog("A critical Hit");
        if (damageDetails.TypeEffectiveness > 1f)
            yield return _dialogBox.TypeDialog("It's super effective");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return _dialogBox.TypeDialog("It's not very effective");

    }
    [SerializeField] GameObject FinishFight, FinishFight2;
    public void HandleUpdate()
    {
        //print(state);
        if (Input.GetKey(KeyCode.J)) { state = BattleState.ActionSelection; }
        //if (Input.GetKey(KeyCode.K)) { state = BattleState.MoveSelection; }
        if (Input.GetKey(KeyCode.L)) { state = BattleState.AboutToUse; }
        if (_playerUnit.Pokemon != null&&_playerUnit.Pokemon.HP <= 0)
        {
            FinishFight.gameObject.SetActive(false);
            FinishFight2.gameObject.SetActive(true);
        }
        else if (_enemyUnit.Pokemon != null&&_enemyUnit.Pokemon.HP <= 0)
        {
            FinishFight.gameObject.SetActive(true);
            FinishFight2.gameObject.SetActive(false);
        }
        else { FinishFight.gameObject.SetActive(false); }
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelectionPanel.gameObject.SetActive(true);
            HandleMoveSelectionPanel.gameObject.SetActive(false);
            HandleParySelectionPanel.gameObject.SetActive(false);
            HandleAboutToUsePanel.gameObject.SetActive(false);
            _partyScreen.gameObject.SetActive(false);
            FightBtns.gameObject.SetActive(true);
            FightBtns1.gameObject.SetActive(true);
            FightBtns2.gameObject.SetActive(true);
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleActionSelectionPanel.gameObject.SetActive(false);
            HandleMoveSelectionPanel.gameObject.SetActive(true);
            HandleParySelectionPanel.gameObject.SetActive(false);
            HandleAboutToUsePanel.gameObject.SetActive(false);
            _partyScreen.gameObject.SetActive(false);
            FightBtns.gameObject.SetActive(false);
            FightBtns1.gameObject.SetActive(false);
            FightBtns2.gameObject.SetActive(false);
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandleActionSelectionPanel.gameObject.SetActive(false);
            HandleMoveSelectionPanel.gameObject.SetActive(false);
            HandleParySelectionPanel.gameObject.SetActive(true);
            HandleAboutToUsePanel.gameObject.SetActive(false);
            HandleParySelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleActionSelectionPanel.gameObject.SetActive(false);
            HandleMoveSelectionPanel.gameObject.SetActive(false);
            HandleParySelectionPanel.gameObject.SetActive(false);
            HandleAboutToUsePanel.gameObject.SetActive(true);
            _partyScreen.gameObject.SetActive(false);
            FightBtns.gameObject.SetActive(false);
            FightBtns1.gameObject.SetActive(false);
            FightBtns2.gameObject.SetActive(false);
            HandleAboutToUse();
        }
    }
    public void ActSelect()
    {
        state = BattleState.ActionSelection;
    }
    public void RightArrowed()
    {
        ++_currentAction;
    }
    public void LeftArrowed()
    {
        --_currentAction;
    }
    public void SetInputZ()
    {
        InputZ = true;
    }
    public void SetInputX()
    {
        InputX = true;
    }
    bool InputZ = false;
    bool InputX = false;
    public void SetParyScreen()
    {
        state = BattleState.PartyScreen;
    }
    public void SeMoveSelect()
    {
        state = BattleState.MoveSelection;
    }
    public void SetActionSelect()
    {
        state = BattleState.ActionSelection;
    }
    public void SetAboutToUse()
    {
        state = BattleState.AboutToUse;
    }
    public void SetFight()
    {
        _currentAction = 0;
        InputZ = true;
    }
    public void SetBag()
    {
        _currentAction = 1;
        InputZ = true;
    }
    public void SetPokemons()
    {
        _currentAction = 2;
        InputZ = true;
    }
    public void SetRun()
    {
        _currentAction = 3;
        InputZ = true;
    }
    bool animate = false;
    public void SetZActionSelect()
    {
        _dialogBox.UpdateActionSelection(_currentAction);
        if (_currentAction == 0)
        {
            MoveSelection();
        }
        else if (_currentAction == 1)
        {
            StartCoroutine(RunTurns(BattleAction.UseItem));
        }
        else if (_currentAction == 2)
        {
            prevState = state;
            OpenPartyScreen();

        }
        else if (_currentAction == 3)
        {
            //run
            //GetTextPlayer.text = "run";
            StartCoroutine(RunTurns(BattleAction.Run));
        }
        if (InputZ) { InputZ = false; }
    }
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++_currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --_currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _currentAction -= 2;
        //print("HandleActionSelection");
        //_currentAction = Mathf.Clamp(_currentAction, 0, 3);
       
        _dialogBox.UpdateActionSelection(_currentAction);
        if (Input.GetKeyDown(KeyCode.Z)|| InputZ)
        {
            if (_currentAction == 0)
            {
                //fight
                //GetTextPlayer.text = "fight";
                MoveSelection();
            }
            else if (_currentAction == 1)
            {
                //Bag
                //GetTextPlayer.text = "Bag";
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if (_currentAction == 2)
            {
                //Pokemon
                //GetTextPlayer.text = "Pokemon";
                prevState = state;
                OpenPartyScreen();

            }
            else if (_currentAction == 3)
            {
                //run
                //GetTextPlayer.text = "run";
                StartCoroutine(RunTurns(BattleAction.Run));
            }
            if (InputZ) { InputZ = false; }
        }
    }
    public void SetZHandleMove()
    {
        //print(_playerUnit.Pokemon.Moves.Count);
        _currentMove = Mathf.Clamp(_currentMove, 0, _playerUnit.Pokemon.Moves.Count - 1);
        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Pokemon.Moves[_currentMove]);
        var move = _playerUnit.Pokemon.Moves[_currentMove];
        if (move.PP == 0) return;
        //_dialogBox.EnableMoveSelector(false);
        //_dialogBox.EnableDialogText(true);
        StartCoroutine(RunTurns(BattleAction.Move));
    }
    public void SetXHandleMove()
    {
        //_currentMove = Mathf.Clamp(_currentMove, 0, _playerUnit.Pokemon.Moves.Count - 1);
        //_dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Pokemon.Moves[_currentMove]);
        //_dialogBox.EnableMoveSelector(false);
        //_dialogBox.EnableDialogText(true);
        ActionSelection();
        InputX = false;
    }
    public void SetCurrentMove(int m1)
    {
        _currentMove = m1;
    }
    [SerializeField] Text GetText1, GetText2, GetText3, GetText4;
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++_currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --_currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _currentMove -= 2;
        //print(_playerUnit.Pokemon.Moves.Count);
        _currentMove = Mathf.Clamp(_currentMove, 0, _playerUnit.Pokemon.Moves.Count - 1);
        _dialogBox.UpdateMoveSelection(_currentMove, _playerUnit.Pokemon.Moves[_currentMove]);
        GetText1.text = _dialogBox.SetViewMove(0, _playerUnit.Pokemon.Moves[0]);
        GetText2.text = _dialogBox.SetViewMove(1, _playerUnit.Pokemon.Moves[1]);
        GetText3.text = _dialogBox.SetViewMove(2, _playerUnit.Pokemon.Moves[2]);
        GetText4.text = _dialogBox.SetViewMove(3, _playerUnit.Pokemon.Moves[3]); 
        _dialogBox.EnableMoveSelector(true);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var move = _playerUnit.Pokemon.Moves[_currentMove];
            if (move.PP == 0)
                return;


            _dialogBox.EnableMoveSelector(false);
            _dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            _dialogBox.EnableMoveSelector(false);
            _dialogBox.EnableDialogText(true);
            ActionSelection();
            InputX = false;
        }
    }
    public void SetZHandleParySelect()
    {
        _currentMember = Mathf.Clamp(_currentMember, 0, _playerParty.Pokemons.Count - 1);
        _partyScreen.UpdateMemberSelection(_currentMember);
        var selectedMember = _playerParty.Pokemons[_currentMember];
        if (selectedMember.HP <= 0)
        {
            _partyScreen.SetMessageText("You can't send out a fainted Pokemon");
            return;
        }
        if (selectedMember == _playerUnit.Pokemon)
        {
            _partyScreen.SetMessageText("You can't switch the same Pokemon");
            return;
        }
        _partyScreen.gameObject.SetActive(false);

        if (prevState == BattleState.ActionSelection)
        {
            prevState = null;
            StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
        }
        else
        {
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedMember));

        }
    }
    public void SetXHandleParySelect()
    {
        _currentMember = Mathf.Clamp(_currentMember, 0, _playerParty.Pokemons.Count - 1);
        _partyScreen.UpdateMemberSelection(_currentMember);
        if (_playerUnit.Pokemon.HP <= 0)
        {
            _partyScreen.SetMessageText("You have to choose a pokemon to continue");
            return;
        }

        _partyScreen.gameObject.SetActive(false);
        if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextTrainerPokemon());
        }
        else
            ActionSelection();
    }
    void HandleParySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++_currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --_currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            _currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            _currentMember -= 2;

        _currentMember = Mathf.Clamp(_currentMember, 0, _playerParty.Pokemons.Count - 1);
        _partyScreen.UpdateMemberSelection(_currentMember);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var selectedMember = _playerParty.Pokemons[_currentMember];
            if (selectedMember.HP <= 0)
            {
                _partyScreen.SetMessageText("You can't send out a fainted Pokemon");
                return;
            }
            if (selectedMember == _playerUnit.Pokemon)
            {
                _partyScreen.SetMessageText("You can't switch the same Pokemon");
                return;
            }
            _partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));

            }


        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (_playerUnit.Pokemon.HP <= 0)
            {
                _partyScreen.SetMessageText("You have to choose a pokemon to continue");
                return;
            }

            _partyScreen.gameObject.SetActive(false);
            if (prevState == BattleState.AboutToUse)
            {
                prevState = null;
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
                ActionSelection();
        }

    }
    public void SetZAbouteToUse()
    {
        print("HandleAboutToUse");
        _dialogBox.UpdateChoiceBox(_aboutToUseChoice);
        _dialogBox.EnableChoiceBox(false);
        if (_aboutToUseChoice)
        {
            //Yes Option,
            prevState = BattleState.AboutToUse;
            OpenPartyScreen();
        }
        else
        {
            //No option
            StartCoroutine(SendNextTrainerPokemon());
        }
    }
    public void SetXAboutToUse()
    {
        //print("HandleAboutToUse");
        _dialogBox.UpdateChoiceBox(_aboutToUseChoice);
        _dialogBox.EnableChoiceBox(false);
        //_dialogBox.EnableChoiceBox(false);
        StartCoroutine(SendNextTrainerPokemon());
    }
    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _aboutToUseChoice = !_aboutToUseChoice;
        }
        print("HandleAboutToUse");
        _dialogBox.UpdateChoiceBox(_aboutToUseChoice);
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _dialogBox.EnableChoiceBox(false);
            if (_aboutToUseChoice)
            {
                //Yes Option,
                prevState = BattleState.AboutToUse;
                OpenPartyScreen();
            }
            else
            {
                //No option
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            _dialogBox.EnableChoiceBox(false);
            StartCoroutine(SendNextTrainerPokemon());
        }
    }
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (_playerUnit.Pokemon.HP > 0)
        {
            yield return _dialogBox.TypeDialog($"Come Back {_playerUnit.Pokemon.Base.Name}");
            _playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        _playerUnit.Setup(newPokemon);
        _dialogBox.SetMoveNames(newPokemon.Moves);
        yield return _dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!.  ");

        if (prevState == null)
        {
            state = BattleState.RunningTurn;
        }
        else if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextTrainerPokemon());
        }

    }

    IEnumerator SendNextTrainerPokemon()
    {
        var nextPokemon = _trainerParty.GetHealtyPokemon();
        state = BattleState.Busy;
        _enemyUnit.Setup(nextPokemon);
        yield return _dialogBox.TypeDialog($" {_trainer.Name} send out {nextPokemon.Base.Name} !");

        state = BattleState.RunningTurn;
    }
    IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (_isTrainerBattle)
        {
            yield return _dialogBox.TypeDialog($" you can't steal the trainers Pokemon!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return _dialogBox.TypeDialog($" {_player.Name} used POKEBALL !");
        var pokeballObj = Instantiate(_pokeballSprite, _playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();

        //Animations
        yield return pokeball.transform.DOJump(_enemyUnit.transform.position + new Vector3(0, 2f), 2f, 1, 1f).WaitForCompletion();
        yield return _enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(_enemyUnit.transform.position.y - 1, 0.5f).WaitForCompletion();
        int shakeCount = TryToCatchPokemon(_enemyUnit.Pokemon);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }
        if (shakeCount == 4)
        {
            //pokemon is Caught
            yield return _dialogBox.TypeDialog($" {_enemyUnit.Pokemon.Base.Name} was caught !");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();
            _playerParty.AddPokemon(_enemyUnit.Pokemon);
            yield return _dialogBox.TypeDialog($" {_enemyUnit.Pokemon.Base.Name} has been added to your party !");
            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            //pokemon  broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return _enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return _dialogBox.TypeDialog($" {_enemyUnit.Pokemon.Base.Name} broke Free !");
            else
                yield return _dialogBox.TypeDialog("Almost caught it");

            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }

    }
    int TryToCatchPokemon(Pokemon pokemon)
    {
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * ConditionDB.GetStatusBonus(pokemon.Status) / (3 * pokemon.MaxHp);
        if (a >= 255)
            return 4;
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 4;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            shakeCount++;
        }
        return shakeCount;
    }
    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;
        if (_isTrainerBattle)
        {
            yield return _dialogBox.TypeDialog($"You Can't run from trainer battles");
            state = BattleState.RunningTurn;
            yield break;
        }
        ++escapeAttempts;
        int playerSpeed = _playerUnit.Pokemon.Speed;
        int enemySpeed = _enemyUnit.Pokemon.Speed;
        if (enemySpeed < playerSpeed)
        {
            yield return _dialogBox.TypeDialog($"Run away safely! ");
            BattleOver(true);

        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;
            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return _dialogBox.TypeDialog($"Run away safely! ");
                BattleOver(true);

            }
            else
            {
                yield return _dialogBox.TypeDialog($"Can't escape! ");
                state = BattleState.RunningTurn;
            }
        }
    }
}
