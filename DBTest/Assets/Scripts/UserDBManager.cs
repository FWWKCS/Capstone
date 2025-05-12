using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class UserDBManager : MonoBehaviour
{
    // Create
    InputField _createUID;
    InputField _createName;
    InputField _createEmail;
    InputField _createPassword;
    Button _createButton;
    Text _createResult;

    // Delete
    InputField _deleteUID;
    Button _deleteButton;
    Text _deleteResult;

    // Update
    InputField _updateUID;
    InputField _updateName;
    InputField _updateEmail;
    InputField _updatePassword;
    Button _updateButton;
    Text _updateResult;

    // GetAll
    Button _getAllButton;
    Text _getAllResult;

    class CreateUserTemplate
    {
        // public int _uid;
        public string _name;
        public string _email;
        public string _password;
    }


    class UpdateUserTemplate
    {
        // public int _uid;
        public string _name;
        public string _email;
        public string _password;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Create
        _createUID = GameObject.Find("CreateInputUID").GetComponent<InputField>();
        _createName = GameObject.Find("CreateInputName").GetComponent<InputField>();
        _createEmail = GameObject.Find("CreateInputEmail").GetComponent<InputField>();
        _createPassword = GameObject.Find("CreateInputPassword").GetComponent<InputField>();
        _createButton = GameObject.Find("CreateButton").GetComponent<Button>();
        _createButton.onClick.AddListener(CreateUser);
        _createResult = GameObject.Find("CreateResult").GetComponent<Text>();

        // Delete
        _deleteUID = GameObject.Find("DeleteInputUID").GetComponent<InputField>();
        _deleteButton = GameObject.Find("DeleteButton").GetComponent<Button>();
        _deleteButton.onClick.AddListener(DeleteUser);
        _deleteResult = GameObject.Find("DeleteResult").GetComponent<Text>();

        // // Update
        _updateUID = GameObject.Find("UpdateInputUID").GetComponent<InputField>();
        _updateName = GameObject.Find("UpdateInputName").GetComponent<InputField>();
        _updateEmail = GameObject.Find("UpdateInputEmail").GetComponent<InputField>();
        _updatePassword = GameObject.Find("UpdateInputPassword").GetComponent<InputField>();
        _updateButton = GameObject.Find("UpdateButton").GetComponent<Button>();
        _updateButton.onClick.AddListener(UpdateUser);
        _updateResult = GameObject.Find("UpdateResult").GetComponent<Text>();

        // // GetAll
        // _getAllButton = GameObject.Find("GetAllButton").GetComponent<Button>();
        // _getAllButton.onClick.AddListener(GetAllObject);
        // _getAllResult = GameObject.Find("GetAllResult").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void CreateUser()
    {
        Debug.Log("CreateUser 호출됨");

        var data = new CreateUserTemplate
        {
            // _uid = int.Parse(_createUID.text),
            _name = _createName.text,
            _email = _createEmail.text,
            _password = _createPassword.text
        };

        string jsonData = JsonConvert.SerializeObject(data);

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("http://localhost:5000/user/create", content); // Post
                _createResult.text = response.Content.ToString();
                Debug.Log("CreateUser 응답: " + _createResult.text);
            }
            catch (HttpRequestException e)
            {
                Debug.Log(e.Message);
                _createResult.text = e.Message;
            }
        }
    }

    async void DeleteUser()
    {
        Debug.Log("DeleteUser 호출됨");

        int uid = int.Parse(_deleteUID.text);
        string url = $"http://localhost:5000/user/{uid}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new System.Uri(url)
                };

                HttpResponseMessage response = await client.SendAsync(request);

                string responseBody = await response.Content.ReadAsStringAsync();
                _deleteResult.text = responseBody;

                Debug.Log("DeleteUser 응답: " + responseBody);
            }
            catch (HttpRequestException e)
            {
                Debug.Log(e.Message);
                _deleteResult.text = e.Message;
            }
        }
    }

    async void UpdateUser()
    {
        Debug.Log("UpdateUser 호출됨");
        int uid = int.Parse(_updateUID.text);

        var data = new UpdateUserTemplate
        {
            // _uid = int.Parse(_updateUID.text),
            _name = _updateName.text,
            _email = _updateEmail.text,
            _password = _updatePassword.text
        };

        string jsonData = JsonConvert.SerializeObject(data);

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                // 경로에 uid 포함
                string requestUri = $"http://localhost:5000/user/{uid}";

                // PUT 요청
                HttpResponseMessage response = await client.PutAsync(requestUri, content);

                string result = await response.Content.ReadAsStringAsync();
                _updateResult.text = result;
            }
            catch (HttpRequestException e)
            {
            Debug.Log(e.Message);
                _updateResult.text = e.Message;
            }
        }
    }
}

