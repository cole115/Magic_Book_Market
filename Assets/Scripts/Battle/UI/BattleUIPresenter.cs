using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

// ui와 전투 흐름을 이어주는 클래스

// UI들 보다는 상위, BattleCardContorller 보다는 하위. 

// 내 메인 턴이 아닐 시 조작 계열 ui(카드 이동, 턴 종료 버튼 등) 을 막아야 함. 지금은 테스트니 나중에
public class BattleUIPresenter : MonoBehaviour
{
    [Header("Hand")]
    [SerializeField] UI_HandCard[] handCardsUI = new UI_HandCard[4];

    [Header("Canvas")]
    [SerializeField] UI_ResultCanvas resultCanvas;
    [SerializeField] UI_DescriptionCanvas descriptionCanvas;

    [Header("Btn")]
    [SerializeField] UI_EndTurnBtn endTurnBtn;
    [SerializeField] UI_RerollBtn rerollBtn;

    [Header("Text")]
    [SerializeField] UI_ManaText manaText;
    [SerializeField] UI_ShowCurrentTurn showTurnText;
    [SerializeField] UI_RerollText rerollText;
    [SerializeField] UI_EffectTextGenerator effectText;

    [Header("PlayerVFX")]
    [SerializeField] UI_PlayerAttackEffect playerAttackEffect;
    [SerializeField] UI_MiasmaOverflow miasmaOverflow;

    [Header("EnemyVFX")]
    [SerializeField] UI_EnemyAttackEffect enemyAttackEffect;
    [SerializeField] UI_EnemyAttackResult enemyAttackResult;

    [Header("Etc")]
    [SerializeField] UI_MiasmaBar miasmaBar;
    [SerializeField] UI_EnemyHpBar enemyHpBar;
    [SerializeField] UI_EnemySE enemySE;
    [SerializeField] UI_AttackPreviewArrows previewArrows;
    [SerializeField] Sprite cardPlaceHolder;


    public bool CanPlayCards;

    public bool HasEndedTurn { get; private set; }



    private void Awake()
    {
        for (int i = 0; i < handCardsUI.Length; i++)
        {
            if (handCardsUI[i] == null)
            {
                Debug.Log($"{handCardsUI[i + 1]}번 슬롯의 패 UI가 연결되지 않았습니다");
                continue;
            }
            handCardsUI[i].Init(i, cardPlaceHolder);
            
        }

        if (endTurnBtn != null)
        {
            endTurnBtn.OnEndTurnPressed += HandleEndTurn;

        }
        if (rerollBtn != null)
        {
            rerollBtn.OnRerollPressed += HandleReroll;

        }
    }


    // 싱글톤이 초기화 되기 이전에 실행되는 걸 막기 위해 start
    public void InitUIPresenter()
    {
        ShowCurrentTurn();
        SubscribeHandCards();
        SubscribeAttackPreview();
        SubscribeUIs();
        MasterBattleManager.Instance.CardController.HandCards.OnHandCardChanged += RefreshAllHandUi;
        MasterBattleManager.Instance.CardController.HandCards.OnHandCardBreak += ShowCardBreak;


    }


    // 현재 진행 중인 턴 텍스트로 표시
    // 임시

    private void ShowCurrentTurn()
    {
        MasterBattleManager.Instance.TurnManager.OnPlayerStart += () =>
            showTurnText.SetTurnText("Player Turn");
        MasterBattleManager.Instance.TurnManager.OnPlayerEnd += () =>
        {
            showTurnText.SetTurnText("Enemy Turn");
            return EmptyCoroutine();

            IEnumerator EmptyCoroutine()
            {
                yield break;
            }
        };
    }

    // 카드 플립을 onCardFilpAttempt에 구독
    // 람다 캡쳐 현상 주의
    private void SubscribeHandCards()
    {
        for (int i = 0; i < 4; i++)
        {
            int index = i;
            UI_HandCard cardUI = handCardsUI[index];


            cardUI.OnTryFlipCard += (slotIndex) =>
            {
                bool canFlip = MasterBattleManager.Instance.CardController.TryFlipCard(slotIndex);
                return canFlip;
            };

            cardUI.OnTryUseCard += (slotIndex) =>
            {
                bool canUse = MasterBattleManager.Instance.CardController.TryUseCard(slotIndex);
                return canUse;
            };

            cardUI.OnDescriptionOpen += (card) =>
            {
                descriptionCanvas.ShowDescription(card);
            };

            cardUI.OnDescriptionClose += () =>
            {
                descriptionCanvas.HideDescription();
            };
        }
    }

    private void SubscribeAttackPreview()
    {
        var processor = MasterBattleManager.Instance.Processor;


        processor.OnEnemyAttackPreview
            += previewArrows.PreviewEnemyAttacks;

        processor.OnEnemyPatternCleared
            += previewArrows.ResetPreview;


        //processor.OnEnemyAttackExecuted
        //    += 
    }



    private void SubscribeUIs()
    {
        MasterBattleManager.Instance.CurrEnemy.OnEnemyHealthChanged += ShowEnemyHp;
        MasterBattleManager.Instance.CurrEnemy.OnSERefreshed += ShowEnemySE;
        MasterBattleManager.Instance.Processor.OnManaChanged += ShowCurrentMana;
        MasterBattleManager.Instance.Processor.OnMiasmaChanged += ShowCurrentMiasma;
        MasterBattleManager.Instance.Processor.OnRerollChanged += ShowCurrentReroll;
        MasterBattleManager.Instance.Processor.OnEffectActivated += ShowEffect;
        MasterBattleManager.Instance.Processor.OnEnemyTryAttack += ShowEnemyTryAttack;
        MasterBattleManager.Instance.Processor.OnAttackEvaded += ShowEvaded;
        MasterBattleManager.Instance.Processor.OnAttackCorrupted += ShowCorrupted;
        MasterBattleManager.Instance.Processor.OnMiasmaOverflow += ShowOverflow;
        MasterBattleManager.Instance.CurrEnemy.OnEnemySEActivated += ShowSEText;

        MasterBattleManager.Instance.TurnManager.OnBattleOver += OpenBattleOverCanvas;
    }


    // 패의 모든 카드의 ui를 갱신.
    // 카드 스프라이트, 카드 방향, 카드 설명 가져옴

    private void RefreshAllHandUi()
    {
        var currHand = MasterBattleManager.Instance.CardController.HandCards.HandSlots;
        for (int i = 0; i < currHand.Length; i++)
        {
            var slot = currHand[i];
            if (slot == null)
            {
                handCardsUI[i].ShowPlaceHolder();
                continue;
            }

            string cardID = slot.Card.CardID;
            string spriteCode = cardID[^3..];
            if (!CardDatabase.IsSpriteCached(spriteCode))
            {
                handCardsUI[i].ShowPlaceHolder();
            }

            _ = LoadCardSprite(slot, handCardsUI[i]);


            // 카드 설명문. 아직 못 넣음
            //string description = currHand[i].cardFace == CardFace.Upright ?
            //    currHand[i].card.Effects.UprDescription : currHand[i].card.Effects.RevDescription;

        }

    }

    
    private void ShowCardBreak(int dir)
    {
        Debug.Log("파괴");
        var targetCard = handCardsUI[dir];

        StartCoroutine(enemyAttackResult.OnBreak(targetCard));
    }

    // 적 공격 결과 처리. 카드 파괴 시에는 ShowCardBreak 쪽으로
    private void ShowEvaded(int dir)
    {
        var targetSlot = handCardsUI[dir];
        
        enemyAttackResult.OnEvade(targetSlot.transform.position);
    }

    private void ShowCorrupted(int slotNum, int damage)
    {
        var targetSlot = handCardsUI[slotNum];

        enemyAttackResult.OnCorrupted(targetSlot.transform.position, damage);
    }



    // 카드 스프라이트 로드
    private async Task LoadCardSprite(GameCard c, UI_HandCard ui)
    {
        string cardID = c.Card.CardID;
        string spriteCode = cardID.Substring(cardID.Length - 3);
        if (!int.TryParse(spriteCode, out int num))
        {
            Debug.LogError($"스프라이트 로드 에러 : CardID가 잘못됨 {cardID}");
            return;

        }
        Sprite sprite = await CardDatabase.GetSprite(spriteCode);
        c.Card.CardSprite = sprite;
        ui.ShowCard(c);
    }



    // 턴 종료 버튼 눌렀을 시 실행
    private void HandleEndTurn()
    {
        MasterBattleManager.Instance.TurnManager.OnPressEndButton();
        Debug.Log("EndTurn");


    }

    // 패 리롤 눌렀을 시 실행
    private void HandleReroll()
    {
        MasterBattleManager.Instance.CardController.TryRerollCard();

        Debug.Log("Reroll");
    }


    // 현재 마나 표시
    private void ShowCurrentMana(int currMana, int maxMana)
    {
        manaText.ShowCurrentMana(currMana, maxMana);
    }

    // 현재 오염도 표시
    private void ShowCurrentMiasma(int currMiasma, int maxMiasma)
    {
        miasmaBar.ShowCurrentMiasma(currMiasma, maxMiasma);
    }

    // 현재 적 체력 표시
    private void ShowEnemyHp(int currHp, int maxHp)
    {
        enemyHpBar.ShowEnemyHp(currHp, maxHp);
    }

    private void ShowEnemySE(Dictionary<StatusEffectType, int> effects)
    {
        enemySE.ShowSE(effects);
    }

    private void ShowCurrentReroll(int reroll)
    {
        rerollText.ShowCurrentReroll(reroll);
    }

    private void ShowEffect(EffectType type, int amount, CardElement? element)
    {
        effectText.ShowEffectText(type, amount, element ?? CardElement.None);
        if(type == EffectType.DealDamage)
        {
            if(element.HasValue)
                playerAttackEffect.PlayAttackEffect(element.Value);
        }
    }

    private void ShowEnemyTryAttack(int dir)
    {
        enemyAttackEffect.PlayEnemyAttack(dir);
    }

    private void ShowOverflow()
    {
        miasmaOverflow.PlayOverflow();
    }


private void ShowSEText(StatusEffectType type, int amount)
    {
        effectText.ShowSEText(type, amount);
    }

    private void OpenBattleOverCanvas(bool isWin, int gold)
    {
        resultCanvas.ShowResult(isWin, gold);
        Debug.Log("결과창");
    }
 
}
