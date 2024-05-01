using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UISettingPopup : UIPopupBase
{
    public bool itemOn
    {
        set => _itemOn = value;
    }

    bool _itemOn;
    SoundSave _soundSave;
    Slider _masterSlider;
    Slider _BGMSlider;
    Slider _SEFSlider;
    Button _closeButton;
    Button _exitButton;

    string filePath;

    protected override void Awake()
    {
        base.Awake();

        filePath = Path.Combine(Application.persistentDataPath, "SoundSave.json");
        if (File.Exists(filePath))
        {
            ParseJsonFile();
        }
        else
        {
            CreateAndParseJsonFile();
        }


        _masterSlider = transform.Find("Panel/Image - SettingPopup/Slider - Master").GetComponent<Slider>();
        _BGMSlider = transform.Find("Panel/Image - SettingPopup/Slider - BGM").GetComponent<Slider>();
        _SEFSlider = transform.Find("Panel/Image - SettingPopup/Slider - SEF").GetComponent<Slider>();
        _closeButton = transform.Find("Panel/Image - SettingPopup/Button - Close").GetComponent<Button>();
        _exitButton = transform.Find("Panel/Image - SettingPopup/Button - Exit").GetComponent<Button>();


        _masterSlider.onValueChanged.AddListener(value =>
        {
            SoundManager.instance.master = value;
        });

        _BGMSlider.onValueChanged.AddListener(value =>
        {
            SoundManager.instance.bgm = value;
        });

        _SEFSlider.onValueChanged.AddListener(value =>
        {
            SoundManager.instance.sef = value;            
        });

        SoundManager.instance.masterEvent += value =>
        {
            _masterSlider.value = value;
            _soundSave.master = value;
        };
        SoundManager.instance.bgmEvent += value =>
        {
            _BGMSlider.value = value;
            _soundSave.BGM = value;
        };
        SoundManager.instance.sefEvent += value =>
        {
            _SEFSlider.value = value;
            _soundSave.SEF = value;
        };

        _closeButton.onClick.AddListener(() =>
        {
            if (_canvas.enabled)
            {
                Hide();
            }
        });
        _exitButton.onClick.AddListener(()=> Application.Quit());
    }

    private void ParseJsonFile()
    {
        string jsonData = File.ReadAllText(filePath);

        // JSON 파싱
        _soundSave = JsonUtility.FromJson<SoundSave>(jsonData);
    }

    private void CreateAndParseJsonFile()
    {
        _soundSave = new SoundSave
        {
            master = 1f,
            BGM = 1f,
            SEF = 1f
        };

        // JSON 문자열로 직렬화
        string jsonData = JsonUtility.ToJson(_soundSave);

        // 파일 생성
        File.WriteAllText(filePath, jsonData);

        // 생성된 파일 파싱
        ParseJsonFile();
    }

    private void Start()
    {
        _masterSlider.value = _soundSave.master;
        _BGMSlider.value = _soundSave.BGM;
        _SEFSlider.value = _soundSave.SEF;
    }

    public override void Show()
    {
        base.Show();
        sortingOrder = 99;
    }


    private void Update()
    {
        if(_itemOn)
        {
            return;
        }

        /*if (UIManager.instance.Get<UIKeyInfo>().inputActionEnable)
            return;*/

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_canvas.enabled)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
    }

    private void OnApplicationQuit()
    {
        // 변경된 _soundSave 객체를 JSON으로 다시 직렬화
        string jsonData = JsonUtility.ToJson(_soundSave);

        // 파일에 변경된 JSON 데이터 쓰기
        File.WriteAllText(filePath, jsonData);
    }
}

