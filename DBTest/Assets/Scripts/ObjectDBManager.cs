using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDBManager : MonoBehaviour
{
    // Create
    InputField _createUID;
    InputField _createOID;
    InputField _createBigTag;
    InputField _createSmallTag;
    InputField _createType;
    InputField _createCost;
    InputField _createExpireCount;
    InputField _createStat;
    InputField _createGrade;
    Button _createButton;
    Text _createResult;

    // Remove
    InputField _removeOID;
    Button _removeButton;
    Text _removeResult;

    // Update
    InputField _updateUID;
    InputField _updateSellState;
    Button _updateButton;
    Text _updateResult;

    // GetAll
    Button _getAllButton;
    Text _getAllResult;

    class CreateObjectTemplate
    {
        public int _oid;
        public int _uid;
        public string _bigTag;
        public string _smallTag;
        public string _type;
        public bool _sellState;
        public int _cost;
        public int _expireCount;
        public int _stat;
        public string _grade;
    }

    class RemoveObjectTemplate
    {
        public int _oid;
    }

    class UpdateObjectTemplate
    {
        public int _uid;
        public bool _sellState;
    }


    void Start()
    {
        // Create
        _createUID = GameObject.Find("CreateInputUID").GetComponent<InputField>();
        _createOID = GameObject.Find("CreateInputOID").GetComponent<InputField>();
        _createBigTag = GameObject.Find("CreateInputBigTag").GetComponent<InputField>();
        _createSmallTag = GameObject.Find("CreateInputSmallTag").GetComponent<InputField>();
        _createType = GameObject.Find("CreateInputType").GetComponent<InputField>();
        _createCost = GameObject.Find("CreateInputCost").GetComponent<InputField>();
        _createExpireCount = GameObject.Find("CreateInputExpireCount").GetComponent<InputField>();
        _createStat = GameObject.Find("CreateInputStat").GetComponent<InputField>();
        _createGrade = GameObject.Find("CreateInputGrade").GetComponent<InputField>();
        _createButton = GameObject.Find("CreateButton").GetComponent<Button>();
        _createButton.onClick.AddListener(CreateObject);
        _createResult = GameObject.Find("CreateResult").GetComponent<Text>();

        // Remove
        _removeOID = GameObject.Find("RemoveInputOID").GetComponent<InputField>();
        _removeButton = GameObject.Find("RemoveButton").GetComponent<Button>();
        _removeButton.onClick.AddListener(RemoveObject);
        _removeResult = GameObject.Find("RemoveResult").GetComponent<Text>();

        // Update
        _updateUID = GameObject.Find("UpdateInputUID").GetComponent<InputField>();
        _updateSellState = GameObject.Find("UpdateInputSellState").GetComponent<InputField>();
        _updateButton = GameObject.Find("UpdateButton").GetComponent<Button>();
        _updateButton.onClick.AddListener(UpdateObject);
        _updateResult = GameObject.Find("UpdateResult").GetComponent<Text>();

        // GetAll
        _getAllButton = GameObject.Find("GetAllButton").GetComponent<Button>();
        _getAllButton.onClick.AddListener(GetAllObject);
        _getAllResult = GameObject.Find("GetAllResult").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void CreateObject()
    {

        var data = new CreateObjectTemplate
        {
            _oid = int.Parse(_createOID.text),
            _uid = int.Parse(_createUID.text),
            _bigTag = _createBigTag.text,
            _smallTag = _createSmallTag.text,
            _type = _createType.text,
            _sellState = false,
            _cost = int.Parse(_createCost.text),
            _expireCount = int.Parse(_createExpireCount.text),
            _stat = int.Parse(_createStat.text),
            _grade = _createGrade.text
        };

        string jsonData = JsonConvert.SerializeObject(data);

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("localhost:5000/instance/create", content); // Post
                string result = await response.Content.ReadAsStringAsync();
                Debug.Log("CreateObject 응답: " + result);
                _createResult.text = result;
            }
            catch (HttpRequestException e)
            {
                Debug.Log(e.Message);
                _createResult.text = e.Message;
            }
        }
    }

    async void RemoveObject()
    {
        var data = new RemoveObjectTemplate
        {
            _oid = int.Parse(_removeOID.text)
        };

        string jsonData = JsonConvert.SerializeObject(data);

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "appication/json");
                HttpResponseMessage response = await client.PostAsync("localhost:port/RemoveObject", content); // Post �޼���, Delete �޼��� ������ ��� ��ū ���� �ʿ�
                _removeResult.text = response.Content.ToString();
            }
            catch (HttpRequestException e)
            {
                Debug.Log(e.Message);
                _removeResult.text = e.Message;
            }
        }
    }

    async void UpdateObject()
    {
        bool updateSellState = false;
        if (_updateSellState.text == "Y")
        {
            updateSellState = true;
        }
        var data = new UpdateObjectTemplate
        {
            _uid = int.Parse(_updateUID.text),
            _sellState = updateSellState
        };

        string jsonData = JsonConvert.SerializeObject(data);

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "appication/json");
                HttpResponseMessage response = await client.PutAsync("localhost:5000/UpdateObject", content); // PUT 
                _updateResult.text = response.Content.ToString();
            }
            catch (HttpRequestException e)
            {
                Debug.Log(e.Message);
                _updateResult.text = e.Message;
            }
        }
    }

    async void GetAllObject()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("localhost:port/GetObject"); // GET �޼ҵ�
                _getAllResult.text = response.Content.ToString();
            }
            catch (HttpRequestException e)
            {
                Debug.Log(e.Message);
                _getAllResult.text = e.Message;
            }
        }
    }
}
