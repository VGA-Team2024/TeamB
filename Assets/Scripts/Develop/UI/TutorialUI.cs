using System.Collections;
using System.Collections.Generic;
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


        [SerializeField, Header("背景")]
        Image _backGround;
        [SerializeField, Header("チュートリアル最初に出すText")]
        Text _messageText;
        [SerializeField, Header("モードボタン")]
        GameObject _modeButton;
        [SerializeField, Header("必殺技ボタン")]
        GameObject _specialMovesButton;
        [SerializeField, Header("防御ボタン")]
        GameObject _defenseButton;
        [SerializeField, Header("防御メッセージ")]
        GameObject _defenseMessage;
        [SerializeField, Header("アクションボタン")]
        GameObject _actionButton;

        public bool _isStart;
        public bool _isTutorial;
        private void Awake()
        {
            allyManager = FindAnyObjectByType<AllyManager>();
            skillManager = FindAnyObjectByType<SkillManager>();
            enemyManager = FindAnyObjectByType<EnemyManager>();
        }
        // Start is called before the first frame update
        void Start()
        {
            if (exam == null)
                exam = FindObjectOfType<Exam>();

            _isStart = false;
            _isTutorial = false;
            _isDefense = true;
            _isMessage = true;
            _isSpecial = true;


            enemyManager.GetCurrentEnemyData.OnDeath += EndTutorial;

        }

        // Update is called once per frame
        void Update()
        {
            if (_isStart == false)
            {
                exam.OnStartPose();
                _isStart = true;
                Debug.Log("ストップ");
                _messageText.text = "試験の訓練をしてみましょう";
                _backGround.gameObject.SetActive(true);
                _messageText.gameObject.SetActive(true);
                _count++;
                _isTutorial = true;
            }

            if (skillManager.GetCurrentHaveCost == skillManager.GetMaxCost && _isSpecial == true)
            {
                SpecialMovesTutorial();
                
            }

            if (GameStatics.Characters[(int)allyManager.GetAllies.GetCharacterType].Hp == (int)allyManager.GetAllies.GetCurrentData.Hp)
            {
                
            }
            else
            {
                DefenseTutorial();
                
            }

            if (Input.GetKeyDown(KeyCode.Mouse0) && _isTutorial == true)
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

                    exam.OnEndPose();
                }

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
            _messageText.text = "このボタンで戦闘のモードを自動、\n手動モードに切り替えられます。";
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
                _messageText.text = "ここに魔法を防いだ回数が表示させます\nこれは試験結果に反映させます";
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
            _messageText.text = "敵を倒しました！\nこれでチュートリアルを終了します。";
            _backGround.gameObject.SetActive(true);
            _messageText.gameObject.SetActive(true);
            _isTutorial = true;
        }
    }
}


