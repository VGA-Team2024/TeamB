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


        [SerializeField, Header("�w�i")]
        Image _backGround;
        [SerializeField, Header("�`���[�g���A���ŏ��ɏo��Text")]
        Text _messageText;
        [SerializeField, Header("���[�h�{�^��")]
        GameObject _modeButton;
        [SerializeField, Header("�K�E�Z�{�^��")]
        GameObject _specialMovesButton;
        [SerializeField, Header("�h��{�^��")]
        GameObject _defenseButton;
        [SerializeField, Header("�h�䃁�b�Z�[�W")]
        GameObject _defenseMessage;
        [SerializeField, Header("�A�N�V�����{�^��")]
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
                Debug.Log("�X�g�b�v");
                _messageText.text = "�����̌P�������Ă݂܂��傤";
                _backGround.gameObject.SetActive(true);
                _messageText.gameObject.SetActive(true);
                _count++;
                _isTutorial = true;
            }

            if (skillManager.GetCurrentHaveCost == skillManager.GetMaxCost && _isSpecial == true)
            {
                SpecialMovesTutorial();
                
            }

            if (GameStatics.Characters[(int)allyManager.GetAllies.GetFirstCharacterType].Hp == (int)allyManager.GetAllies.GetCurrentData.Hp)
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
            _messageText.text = "���̃{�^���������Ɩ��@���J��o���܂��B\n�U���A�h�䖂�@���J��o���܂��B";
            _backGround.gameObject.SetActive(true);
            _actionButton.gameObject.SetActive(true);
            _messageText.gameObject.SetActive(true);
            _isTutorial = true;
        }

        public void ModeTutorial()
        {
            exam.OnStartPose();
            _messageText.text = "���̃{�^���Ő퓬�̃��[�h�������A\n�蓮���[�h�ɐ؂�ւ����܂��B";
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
                _messageText.text = "���̃{�^���������Ėh������Ă��������B";
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
            _messageText.text = "���ɕK�E�Z�𔭓����Ă݂܂��傤�B";
            _backGround.gameObject.SetActive(true);
            _specialMovesButton.gameObject.SetActive(true);
            _messageText.gameObject.SetActive(true);
            _isTutorial = true;
            _isSpecial = false;
        }


        public void EndTutorial()
        {
            exam.OnStartPose();
            _messageText.text = "�G��|���܂����I\n����Ń`���[�g���A�����I�����܂��B";
            _backGround.gameObject.SetActive(true);
            _messageText.gameObject.SetActive(true);
            _isTutorial = true;
        }
    }
}


