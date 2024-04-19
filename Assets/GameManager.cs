using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField name;
    public TMP_InputField whatsapp;
    public GameObject option1;
    public GameObject option2;
    public GameObject option3;
    public GameObject field1;
    public GameObject field2;
    public GameObject button;
    public GameObject optionimage;
    public GameObject bgimage;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void saveplayername()
    {
        PlayerPrefs.SetString("username",name.text);
    }
    public void savewanumber()
    {
        PlayerPrefs.SetString("number","91"+whatsapp.text);
    }
    public void options()
    {
        option1.SetActive(true);
        option2.SetActive(true);
        option3.SetActive(true);
        optionimage.SetActive(true);    
       
        button.SetActive(false);
        bgimage.SetActive(false);
    }
    public void scene2()
    {
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        optionimage.SetActive(false);
       
        button.SetActive(true);
        bgimage.SetActive(true);
        SceneManager.LoadScene(2);
    }
    public void scene3()
    {
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        optionimage.SetActive(false);
        
        button.SetActive(true);
        bgimage.SetActive(true);
        SceneManager.LoadScene(3);
    }
    public void scene4()
    {
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        optionimage.SetActive(false);
      
        button.SetActive(true);
        bgimage.SetActive(true);
        SceneManager.LoadScene(4);
    }
}
