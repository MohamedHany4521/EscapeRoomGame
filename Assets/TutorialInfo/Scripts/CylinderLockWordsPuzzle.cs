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
    private int _currentLetterDial = 0;

    private int _letterDial01Step = 0;
    [SerializeField] private GameObject _letterDial01;
    private int _letterDial02Step = 0;
    [SerializeField] private GameObject _letterDial02;
    private int _letterDial03Step = 0;
    [SerializeField] private GameObject _letterDial03;
    private int _letterDial04Step = 0;
    [SerializeField] private GameObject _letterDial04;
    private string _letterDial01Letter = ""; 
    private string _letterDial02Letter = "";
    private string _letterDial03Letter = "";
    private string _letterDial04Letter = "";
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

            if (Input.GetKeyDown((KeyCode.D)))
            {
                _currentLetterDial = Mathf.Min(_currentLetterDial + 1, 2);
            }

            if (Input.GetKeyDown((KeyCode.A)))
            {
                _currentLetterDial = Mathf.Max(_currentLetterDial - 1, 0);
            }
            if (Input.GetKeyDown((KeyCode.W)))
            {
                if (_currentLetterDial == 0)
                {
                    _letterDial01Step = (_letterDial01Step + 1) % 16;
                    _letterDial01.transform.localEulerAngles = new Vector3(0, _letterDial01Step * 360f / 26, 0);
                    LetterDial01Values();
                }

                if (_currentLetterDial == 1)
                {
                    _letterDial02Step = (_letterDial02Step + 1) % 16;
                    _letterDial02.transform.localEulerAngles = new Vector3(0, _letterDial02Step * 360f / 26, 0);
                    LetterDial02Values();
                }

                if (_currentLetterDial == 2)
                {
                    _letterDial03Step = (_letterDial03Step + 1) % 16;
                    _letterDial03.transform.localEulerAngles = new Vector3(0, _letterDial03Step * 360f / 26, 0);
                    LetterDial03Values();
                }

                if (_currentLetterDial == 3)
                {
                    _letterDial04Step = (_letterDial04Step + 1) % 16;
                    _letterDial04.transform.localEulerAngles = new Vector3(0, _letterDial04Step * 360f / 26, 0);
                    LetterDial04Values();
                }
            }
            if (Input.GetKeyDown((KeyCode.S)))
            {
                if (_currentLetterDial == 0)
                {
                    _letterDial01Step = (_letterDial01Step - 1 + 16) % 16;
                    _letterDial01.transform.localEulerAngles = new Vector3(0, _letterDial01Step * 360f / 26, 0);
                    LetterDial01Values();
                }

                if (_currentLetterDial == 1)
                {
                    _letterDial02Step = (_letterDial02Step + 1) % 16;
                    _letterDial02.transform.localEulerAngles = new Vector3(0, _letterDial02Step * 360f / 26, 0);
                    LetterDial02Values();
                }

                if (_currentLetterDial == 2)
                {
                    _letterDial03Step = (_letterDial03Step + 1) % 16;
                    _letterDial03.transform.localEulerAngles = new Vector3(0, _letterDial03Step * 360f / 26, 0);
                    LetterDial03Values();
                }

                if (_currentLetterDial == 3)
                {
                    _letterDial04Step = (_letterDial04Step + 1) % 16;
                    _letterDial04.transform.localEulerAngles = new Vector3(0, _letterDial04Step * 360f / 26, 0);
                    LetterDial04Values();
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

    public void LetterDial01Values()
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
    public void LetterDial02Values()
    {
        switch (_letterDial02Step)
        {
            case 0:
                _letterDial02Letter = "A";
                break;

            case 1:
                _letterDial02Letter = "B";
                break;

            case 2:
                _letterDial02Letter  = "C";
                break;

            case 3:
                _letterDial02Letter = "D";
                break;

            case 4:
                _letterDial02Letter = "E";
                break;

            case 5:
                _letterDial02Letter = "F";
                break;

            case 6:
                _letterDial02Letter = "G";
                break;
            case 7:
                _letterDial02Letter = "H";
                break;

            case 8:
                _letterDial02Letter = "I";
                break;

            case 9:
                _letterDial02Letter = "J";
                break;

            case 10:
                _letterDial02Letter = "K";
                break;
            case 11:
                _letterDial02Letter = "L";
                break;

            case 12:
                _letterDial02Letter = "M";
                break;

            case 13:
                _letterDial02Letter = "N";
                break;

            case 14:
                _letterDial02Letter = "O";
                break;

            case 15:
                _letterDial02Letter = "P";
                break;

            Default:
                _letterDial02Letter = "A";
                break;
        }
        Debug.Log("Letter Dial 02 Letter: " + _letterDial02Letter);
    }

    public void LetterDial03Values()
    {
        switch (_letterDial03Step)
        {
            case 0:
                _letterDial03Letter = "A";
                break;

            case 1:
                _letterDial03Letter = "B";
                break;

            case 2:
                _letterDial03Letter = "C";
                break;

            case 3:
                _letterDial03Letter = "D";
                break;

            case 4:
                _letterDial03Letter = "E";
                break;

            case 5:
                _letterDial03Letter = "F";
                break;

            case 6:
                _letterDial03Letter = "G";
                break;
            case 7:
                _letterDial03Letter = "H";
                break;

            case 8:
                _letterDial03Letter = "I";
                break;

            case 9:
                _letterDial03Letter = "J";
                break;

            case 10:
                _letterDial03Letter = "K";
                break;
            case 11:
                _letterDial03Letter = "L";
                break;

            case 12:
                _letterDial03Letter = "M";
                break;

            case 13:
                _letterDial03Letter = "N";
                break;

            case 14:
                _letterDial03Letter = "O";
                break;

            case 15:
                _letterDial03Letter = "P";
                break;

            default:
                _letterDial03Letter = "A";
                break;
        }
        Debug.Log("Letter Dial 03 Letter: " + _letterDial03Letter);
    }

    public void LetterDial04Values()
    {
        switch (_letterDial04Step)
        {
            case 0:
                _letterDial04Letter = "A";
                break;

            case 1:
                _letterDial04Letter = "B";
                break;

            case 2:
                _letterDial04Letter = "C";
                break;

            case 3:
                _letterDial04Letter = "D";
                break;

            case 4:
                _letterDial04Letter = "E";
                break;

            case 5:
                _letterDial04Letter = "F";
                break;

            case 6:
                _letterDial04Letter = "G";
                break;
            case 7:
                _letterDial04Letter = "H";
                break;

            case 8:
                _letterDial04Letter = "I";
                break;

            case 9:
                _letterDial04Letter = "J";
                break;

            case 10:
                _letterDial04Letter = "K";
                break;
            case 11:
                _letterDial04Letter = "L";
                break;

            case 12:
                _letterDial04Letter = "M";
                break;

            case 13:
                _letterDial04Letter = "N";
                break;

            case 14:
                _letterDial04Letter = "O";
                break;

            case 15:
                _letterDial04Letter = "P";
                break;

            default:
                _letterDial04Letter = "A";
                break;
        }
        Debug.Log("Letter Dial 04 Letter: " + _letterDial04Letter);
    }
}