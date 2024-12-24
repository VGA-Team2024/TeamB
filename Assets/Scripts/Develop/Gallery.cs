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

    public GalleryMode _galleryMode;

    [SerializeField]
    Sprite[] _backGround;

    [SerializeField]
    Sprite[] _characterPortrait;

    [SerializeField]
    Sprite[] _still;

    [SerializeField] GameObject _tilePrefab;
    [SerializeField] private Transform canvasTransform;

    [SerializeField] Image _pickUpImage;
    [SerializeField] Image _pickUpImageBackGround;

    [SerializeField] Button _backgroundButton;
    [SerializeField] Button _portraitButton;
    [SerializeField] Button _stillButton;

    public int _gridSize = 3;

    private bool _isChoice;

    private List<GameObject> _tiles = new List<GameObject>(); 

    void Start()
    {
        
        GenerateTiles();
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
        RectTransform trans = _pickUpImage.gameObject.GetComponent<RectTransform>();
        trans.sizeDelta = new Vector2(image.sprite.texture.width, image.sprite.texture.height);
        trans.localScale = Vector3.one * 0.2f;
        _pickUpImageBackGround.gameObject.SetActive(true);
        _isChoice = true;
    }


    public void TypeChange(GalleryMode galleryMode)
    {
        _galleryMode = galleryMode;
        GenerateTiles();
    }

    public void GenerateTiles()
    {
        ClearTiles();

        int spriteIndex = 0;
        int startX = -45;
        int startY = -80;
        int offsetX = 165;
        int offsetY = 140;

        for (int y = 0; y < _gridSize; y++)
        {
            for (int x = 0; x < _gridSize; x++)
            {
                if (spriteIndex >= GetSpriteArray().Length) break;

                GameObject tile = Instantiate(_tilePrefab, canvasTransform);
                RectTransform rectTransform = tile.GetComponent<RectTransform>();
                Button button = tile.GetComponent<Button>();
                rectTransform.anchoredPosition = new Vector2(startX + x * offsetX, startY + y * offsetY);
                
                Image imageComponent = tile.GetComponent<Image>();
                if (imageComponent != null)
                {
                    imageComponent.sprite = GetSpriteArray()[spriteIndex];

                    rectTransform.sizeDelta = new Vector2(GetSpriteArray()[spriteIndex].texture.width, GetSpriteArray()[spriteIndex].texture.height);
                    rectTransform.localScale = Vector3.one * 0.08f;
                }

                button.onClick.AddListener(() => Choice(imageComponent));

                _tiles.Add(tile);

                spriteIndex++;
            }
        }
    }

    private void ClearTiles()
    {
        foreach (GameObject tile in _tiles)
        {
            Destroy(tile); 
        }
        _tiles.Clear();
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
