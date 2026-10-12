using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CylinderLockWordsPuzzle : MonoBehaviour
{
    [Header("Puzzle Info")]
    [SerializeField] private string _puzzleCode;
    [SerializeField] private UnityEvent _event;

    [Header("Objects In Puzzle")]
    [SerializeField] private GameObject _lockInteractivef;
    [SerializeField] private GameObject _lockPuzzle;
    [SerializeField] private GameObject _VC;

    private bool _puzzleStarts;
    private float _rotationSpeed = 45f;
    private int _letterDial01Step = 0;
    private int _currentLetterDial = 0;

    [SerializeField] private GameObject _letterDial01;
    [SerializeField] private GameObject _letterDial02;
    [SerializeField] private GameObject _letterDial03;
    [SerializeField] private GameObject _letterDial04;
    private string _letterDial01Letter = ""; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_puzzleStarts == true)
        {
            if (Input.GetMouseButtonDown(1))
            {
                EndPuzzle();
            }
            if (Input.GetKeyDown((KeyCode.W)))
            {
                if (_currentLetterDial == 0)
                {
                    _letterDial01Step = (_letterDial01Step + 1) % 16;
                    _letterDial01.transform.localEulerAngles = new Vector3(0, _letterDial01Step * 360f / 26, 0);
                    LetterDialValues();
                }
       
            }
            if (Input.GetKeyDown((KeyCode.S)))
            {
                if (_currentLetterDial == 0)
                {
                    _letterDial01Step = (_letterDial01Step - 1 + 16) % 16;
                    _letterDial01.transform.localEulerAngles = new Vector3(0, _letterDial01Step * 360f / 26, 0);
                    LetterDialValues();
                }
            }
        }
    }
    public void StartGame()
    {
        StartCoroutine("PuzzleStart");
    }
    public void EndPuzzle()
    {
        GameManger.Instance.UnpauseGame();
        _lockInteractivef.SetActive(true);
        _lockPuzzle.SetActive(false);
        _VC.SetActive(false);
        _puzzleStarts = false;
    }

    IEnumerable PuzzleStart ()
    {
        _VC.SetActive(true);
        yield return new WaitForSeconds(.5f);
        GameManger.Instance.PauseGame();
        _lockInteractivef.SetActive(false);
        _lockPuzzle.SetActive(true);
        _puzzleStarts = true;
    }

    public void LetterDialValues()
    {
        switch (_letterDial01Step) 
        {
            case 0:
                _letterDial01Letter = "A";
                break;

            case 1:
                _letterDial01Letter = "B";
                break;

            case 2:
                _letterDial01Letter = "C";
                break;
            
            case 3:
                _letterDial01Letter = "D";
                break;

            case 4:
                _letterDial01Letter = "E";
                break;

            case 5:
                _letterDial01Letter = "F";
            break;

            case 6:
                _letterDial01Letter = "G";
                break;  
            case 7:
                _letterDial01Letter = "H";
                break;

            case 8:
                _letterDial01Letter = "I";
                break;

            case 9:
                _letterDial01Letter = "J";
            break;

            case 10:
                _letterDial01Letter = "K";
                break;
            case 11:
                _letterDial01Letter = "L";
                break;

            case 12:
                _letterDial01Letter = "M";
                break;
            
            case 13:
                _letterDial01Letter = "N";
                break;

            case 14:
                _letterDial01Letter = "O";
                break;

            case 15:
                _letterDial01Letter = "P";
                break;

            Default:
                _letterDial01Letter = "A";
                break;
        }
        Debug.Log("Letter Dial 01 Letter: " + _letterDial01Letter);
    }
}
