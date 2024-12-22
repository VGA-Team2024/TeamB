using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{

    public enum GalleryMode
    {
        BackGround,
        CharacterPortrait,
        Still
    }

    [Header("ギャラリーモード")]
    public GalleryMode _galleryMode;

    [SerializeField, Header("背景")]
    Sprite[] _backGround;

    [SerializeField, Header("立ち絵")]
    Sprite[] _characterPortrait;

    [SerializeField, Header("スチル")]
    Sprite[] _still;

    [SerializeField] GameObject _tilePrefab; // 作成したPrefab
    [SerializeField] Transform canvasTransform; // 親になるCanvasのTransform

    [SerializeField] Image _pickUpImage;
    [SerializeField] Image _pickUpImageBackGround;

    [SerializeField,Header("背景button")] Button _backgroundButton;
    [SerializeField, Header("立ち絵button")] Button _portraitButton;
    [SerializeField, Header("スチルbutton")] Button _stillButton;

    public int _gridSize = 3; // デフォルトのグリッドサイズ（3x3）

    private bool _isChoice;

    private List<GameObject> _tiles = new List<GameObject>(); // タイルをリストで管理

    void Start()
    {
        
        GenerateTiles();
        // 各ボタンにTypeChangeメソッドをラムダ式で設定
        _backgroundButton.onClick.AddListener(() => TypeChange(Gallery.GalleryMode.BackGround));
        _portraitButton.onClick.AddListener(() => TypeChange(Gallery.GalleryMode.CharacterPortrait));
        _stillButton.onClick.AddListener(() => TypeChange(Gallery.GalleryMode.Still));

        _isChoice = false;
    }

    private void Update()
    {
        if (_isChoice! && Input.GetKeyDown(KeyCode.Mouse0))
        {
            _pickUpImageBackGround.gameObject.SetActive(false);
        }
    }

    public void SceneChange(string sceneName)
    {
        SceneLoader.LoadScene(sceneName);
    }


    public void Choice(Image image)
    {   
        _pickUpImage.sprite = image.sprite;
        _pickUpImageBackGround.gameObject.SetActive(true);
        _isChoice = true;
    }


    public void TypeChange(GalleryMode galleryMode)
    {
        _galleryMode = galleryMode; // モードを更新
        GenerateTiles(); // タイルを再生成
    }

    public void GenerateTiles()
    {
        // 既存のタイルを削除
        ClearTiles();

        int spriteIndex = 0;
        int startX = -25;
        int startY = -140;
        int offsetX = 165;
        int offsetY = 140;

        // グリッドにタイルを配置
        for (int y = 0; y < _gridSize; y++)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                if (spriteIndex >= GetSpriteArray().Length) break;

                // タイルを生成し、位置を設定
                GameObject tile = Instantiate(_tilePrefab, canvasTransform);
                RectTransform rectTransform = tile.GetComponent<RectTransform>();
                Button button = tile.GetComponent<Button>();
                rectTransform.anchoredPosition = new Vector2(startX + x * offsetX, startY + y * offsetY);
                rectTransform.sizeDelta = new Vector2(150, 100);
                // スプライトを設定
                Image imageComponent = tile.GetComponent<Image>();
                if (imageComponent != null)
                {
                    imageComponent.sprite = GetSpriteArray()[spriteIndex];
                }

                button.onClick.AddListener(() => Choice(imageComponent));

                // タイルリストに追加
                _tiles.Add(tile);

                spriteIndex++;
            }
        }
    }

    private void ClearTiles()
    {
        foreach (GameObject tile in _tiles)
        {
            Destroy(tile); // 既存のタイルを削除
        }
        _tiles.Clear(); // リストをクリア
    }

    public Sprite[] GetSpriteArray()
    {
        switch (_galleryMode)
        {
            case GalleryMode.BackGround:
                return _backGround;
            case GalleryMode.CharacterPortrait:
                return _characterPortrait;
            case GalleryMode.Still:
                return _still;
            default:
                return new Sprite[0];
        }
    }
}
