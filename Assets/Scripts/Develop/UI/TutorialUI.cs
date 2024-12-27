using System.Collections;
using System.Collections.Generic;
using TeamB.Data;
using TeamB.Develop;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TeamB.GameSystem.Statics;

namespace TeamB.Develop
{
    public class TutorialUI : MonoBehaviour
    {
        private Exam exam;
        private SkillManager skillManager;
        private AllyManager allyManager;
        private EnemyManager enemyManager;
        private int _count = 0;
        public bool _isDefense;
        private bool _isMessage;
        private bool _isSpecial;


        [SerializeField] Image _backGround;
        [SerializeField] TMP_Text _messageText;
        [SerializeField] GameObject _modeButton;
        [SerializeField] GameObject _specialMovesButton;
        [SerializeField] GameObject _defenseButton;
        [SerializeField] GameObject _defenseMessage;
        [SerializeField] GameObject _actionButton;

        public bool _isStart;
        public bool _isTutorial;

        private void Awake()
        {
            allyManager = FindAnyObjectByType<AllyManager>();
            skillManager = FindAnyObjectByType<SkillManager>();
            enemyManager = FindAnyObjectByType<EnemyManager>();
            exam = FindAnyObjectByType<Exam>();
            exam.OnExamStarted += Initialize;
        }


        private void Initialize()
        {
            switch (GameStatics.ExamState)
            {
                case ExamState.Tutorial:

                    _isStart = false;
                    _isTutorial = false;
                    _isDefense = true;
                    _isMessage = true;
                    _isSpecial = true;


                    enemyManager.GetCurrentEnemyData.OnDeath += EndTutorial;
                    exam.OnExamUpdated += TutorialUpdate;
                    break;
            }
        }

        private void TutorialUpdate(float deltaTime)
        {
            if (_isStart == false)
            {
                exam.OnStartPose();
                _isStart = true;
                _messageText.text = "試験の訓練をしてみましょう。";
                _backGround.gameObject.SetActive(true);
                _messageText.gameObject.SetActive(true);
                _count++;
                _isTutorial = true;
            }

            if (Mathf.Approximately(skillManager.GetCurrentHaveCost, skillManager.GetMaxCost) &&
                _isSpecial == true)
            {
                SpecialMovesTutorial();
            }

            if (!Mathf.Approximately(
                    GameStatics.Characters[(int)allyManager.GetAllies.GetFirstCharacterType].Hp,
                    (int)allyManager.GetAllies.GetCurrentData.Hp))
            {
                DefenseTutorial();
            }
        }

        public void PanelInput()
        {
            if (_count == 1)
            {
                ActionButtonTutorial();
                _count++;
            }
            else if (_count == 2)
            {
                ModeTutorial();
                _count++;
            }
            else
            {
                _isTutorial = false;
                _backGround.gameObject.SetActive(false);
                _messageText.gameObject.SetActive(false);
                _modeButton.gameObject.SetActive(false);
                _specialMovesButton.gameObject.SetActive(false);
                _defenseButton.gameObject.SetActive(false);
                _defenseMessage.gameObject.SetActive(false);

                if (_count != 4)
                    exam.OnEndPose();
            }
        }


        public void ActionButtonTutorial()
        {
            exam.OnStartPose();
            _messageText.text = "このボタンを押すと魔法を繰り出せます。\n攻撃、防御魔法を繰り出せます。";
            _backGround.gameObject.SetActive(true);
            _actionButton.gameObject.SetActive(true);
            _messageText.gameObject.SetActive(true);
            _isTutorial = true;
        }

        public void ModeTutorial()
        {
            exam.OnStartPose();
            _messageText.text = "このボタンで戦闘のモードを自動、手動モードに切り替えられます。";
            _actionButton.gameObject.SetActive(false);
            _backGround.gameObject.SetActive(true);
            _modeButton.gameObject.SetActive(true);
            _messageText.gameObject.SetActive(true);
            _isTutorial = true;
        }

        public void DefenseTutorial()
        {
            if (_isDefense == true)
            {
                exam.OnStartPose();
                _messageText.text = "このボタンを押して防御をしてください。";
                _backGround.gameObject.SetActive(true);
                _defenseButton.gameObject.SetActive(true);
                _messageText.gameObject.SetActive(true);
                _isTutorial = true;
                _isDefense = false;
            }
        }

        public void DefenseMessage()
        {
            if (_isMessage == true && _isDefense == false)
            {
                exam.OnStartPose();
                _messageText.text = "";
                _backGround.gameObject.SetActive(true);
                _messageText.gameObject.SetActive(true);
                _defenseMessage.gameObject.SetActive(true);
                _isTutorial = true;
                _isMessage = false;
            }
        }

        public void SpecialMovesTutorial()
        {
            exam.OnStartPose();
            _messageText.text = "次に必殺技を発動してみましょう。";
            _backGround.gameObject.SetActive(true);
            _specialMovesButton.gameObject.SetActive(true);
            _messageText.gameObject.SetActive(true);
            _isTutorial = true;
            _isSpecial = false;
        }


        public void EndTutorial()
        {
            exam.OnStartPose();
            _messageText.text = "敵を倒しました！これでチュートリアルを終了します。";
            _backGround.gameObject.SetActive(true);
            _messageText.gameObject.SetActive(true);
            _isTutorial = true;
            _count++;
        }
    }
}