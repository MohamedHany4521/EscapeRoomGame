using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;


public class CylinderLockNumbersPuzzle : MonoBehaviour
{
    [Header ("Puzzle Info")]
    [SerializeField] private string _puzzleCode;
    [SerializeField] private UnityEvent _event;

    [Header ("Objects In Puzzle")]
    [SerializeField] private GameObject _lockInteractive;
    [SerializeField] private GameObject _lockPuzzle;
    [SerializeField] private GameObject _VC;

    private bool _puzzleStarts;
    private float _rotationStep = 36f;
    [SerializeField] private int _currentCylinder = 0;

    private int _cylinder01Step = 0;
    [SerializeField] private GameObject _cylinder01;
    private int _cylinder02Step = 0;
    [SerializeField] private GameObject _cylinder02;
    private int _cylinder03Step = 0;
    [SerializeField] private GameObject _cylinder03;
    private int _cylinder04Step = 0;
    [SerializeField] private GameObject _cylinder04;
    private string _cylinder01Leter = "";
    private string _cylinder02Leter = "";
    private string _cylinder03Leter = "";
    private string _cylinder04Leter = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EndPuzzle();
    }

    // Update is called once per frame
    void Update()
    {
        if (_puzzleStarts == true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                EndPuzzle();
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                _currentCylinder = Mathf.Min(_currentCylinder + 1, 3);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                _currentCylinder = Mathf.Max(_currentCylinder - 1, 0);
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_currentCylinder == 0)
                {
                    _cylinder01Step = (_cylinder01Step + 1) % 10;
                    _cylinder01.transform.localEulerAngles = new Vector3(_cylinder01Step * 36f, 0f, 0f);
                    Cylinder01Values();
                }
                if (_currentCylinder == 1)
                {
                    _cylinder02Step = (_cylinder02Step + 1) % 10;
                    _cylinder02.transform.localEulerAngles = new Vector3(_cylinder02Step * 36f, 0f, 0f);
                    Cylinder02Values();
                }
                if (_currentCylinder == 2)
                {
                    _cylinder03Step = (_cylinder03Step + 1) % 10;
                    _cylinder03.transform.localEulerAngles = new Vector3(_cylinder03Step * 36f, 0f, 0f);
                    Cylinder03Values();
                }
                if (_currentCylinder == 3)
                {
                    _cylinder04Step = (_cylinder04Step + 1) % 10;
                    _cylinder04.transform.localEulerAngles = new Vector3(_cylinder04Step * 36f, 0f, 0f);
                    Cylinder04Values();
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (_currentCylinder == 0)
                {
                    _cylinder01Step = (_cylinder01Step - 1 + 10) % 10;
                    _cylinder01.transform.localEulerAngles = new Vector3(_cylinder01Step * 36f, 0f, 0f);
                    Cylinder01Values();
                }
                if (_currentCylinder == 1)
                {
                    _cylinder02Step = (_cylinder02Step - 1 + 10) % 10;
                    _cylinder02.transform.localEulerAngles = new Vector3(_cylinder02Step * 36f, 0f, 0f);
                    Cylinder02Values();
                }
                if (_currentCylinder == 2)
                {
                    _cylinder03Step = (_cylinder03Step - 1 + 10) % 10;
                    _cylinder03.transform.localEulerAngles = new Vector3(_cylinder03Step * 36f, 0f, 0f);
                    Cylinder03Values();
                }
                if (_currentCylinder == 3)
                {
                    _cylinder04Step = (_cylinder04Step - 1 + 10) % 10;
                    _cylinder04.transform.localEulerAngles = new Vector3(_cylinder04Step * 36f, 0f, 0f);
                    Cylinder04Values();
                }
            }
        }
    }

    public void CheckPuzzle()
    {
        string enteredCode = _cylinder01Leter + _cylinder02Leter + _cylinder03Leter + _cylinder04Leter;
        if (enteredCode == _puzzleCode)
        {
            StartCoroutine("CompletePuzzle");
            Debug.Log("Puzzle Solved! Code: " + enteredCode);
        }
        else
        {
            Debug.Log("Code Incorrect! Entered Code: " + enteredCode + " Expected Code: " + _puzzleCode);
        }
    }
    public void Cylinder01Values()
    {
        switch (_cylinder01Step)
        {
            case 0:
                _cylinder01Leter = "1";
                break;
            case 1:
                _cylinder01Leter = "2";
                break;
            case 2:
                _cylinder01Leter = "3";
                break;
            case 3:
                _cylinder01Leter = "4";
                break;
            case 4:
                _cylinder01Leter = "5";
                break;
            case 5:
                _cylinder01Leter = "6";
                break;
            case 6:
                _cylinder01Leter = "7";
                break;
            case 7:
                _cylinder01Leter = "8";
                break;
            case 8:
                _cylinder01Leter = "9";
                break;
            case 9:
                _cylinder01Leter = "0";
                break;
            default:
                _cylinder01Leter = "1";
                break;
        }
        Debug.Log("Number is: " + _cylinder01Leter + "and case is " + _cylinder01Step);
        CheckPuzzle();
    }

    public void Cylinder02Values()
    {
        switch (_cylinder02Step)
        {
            case 0:
                _cylinder02Leter = "1";
                break;
            case 1:
                _cylinder02Leter = "2";
                break;
            case 2:
                _cylinder02Leter = "3";
                break;
            case 3:
                _cylinder02Leter = "4";
                break;
            case 4:
                _cylinder02Leter = "5";
                break;
            case 5:
                _cylinder02Leter = "6";
                break;
            case 6:
                _cylinder02Leter = "7";
                break;
            case 7:
                _cylinder02Leter = "8";
                break;
            case 8:
                _cylinder02Leter = "9";
                break;
            case 9:
                _cylinder02Leter = "0";
                break;
            default:
                _cylinder02Leter = "1";
                break;
        }
        Debug.Log("Number is: " + _cylinder02Leter + "and case is " + _cylinder02Step);
        CheckPuzzle();
    }
    public void Cylinder03Values()
    {
        switch (_cylinder03Step)
        {
            case 0:
                _cylinder03Leter = "1";
                break;
            case 1:
                _cylinder03Leter = "2";
                break;
            case 2:
                _cylinder03Leter = "3";
                break;
            case 3:
                _cylinder03Leter = "4";
                break;
            case 4:
                _cylinder03Leter = "5";
                break;
            case 5:
                _cylinder03Leter = "6";
                break;
            case 6:
                _cylinder03Leter = "7";
                break;
            case 7:
                _cylinder03Leter = "8";
                break;
            case 8:
                _cylinder03Leter = "9";
                break;
            case 9:
                _cylinder03Leter = "0";
                break;
            default:
                _cylinder03Leter = "1";
                break;
        }
        Debug.Log("Number is: " + _cylinder03Leter + "and case is " + _cylinder03Step);
        CheckPuzzle();
    }
    public void Cylinder04Values()
    {
        switch (_cylinder04Step)
        {
            case 0:
                _cylinder04Leter = "1";
                break;
            case 1:
                _cylinder04Leter = "2";
                break;
            case 2:
                _cylinder04Leter = "3";
                break;
            case 3:
                _cylinder04Leter = "4";
                break;
            case 4:
                _cylinder04Leter = "5";
                break;
            case 5:
                _cylinder04Leter = "6";
                break;
            case 6:
                _cylinder04Leter = "7";
                break;
            case 7:
                _cylinder04Leter = "8";
                break;
            case 8:
                _cylinder04Leter = "9";
                break;
            case 9:
                _cylinder04Leter = "0";
                break;
            default:
                _cylinder04Leter = "1";
                break;
        }
        Debug.Log("Number is: " + _cylinder04Leter + "and case is " + _cylinder04Step);
        CheckPuzzle();
    }

    public void StartPuzzle()
    {
        StartCoroutine("PuzzleStart");
    }
    public void EndPuzzle()
    {
        GameManger.Instance.UnpauseGame();
        _lockInteractive.SetActive(true);
        _lockPuzzle.SetActive(false);
        _VC.SetActive(false);
        _puzzleStarts = false;
    }
    IEnumerator CompletePuzzle()
    {
        GameManger.Instance.UnpauseGame();
        //open lock animation
        yield return new WaitForSeconds(1.0f);
        _lockInteractive.SetActive(true);
        EndPuzzle();
        _event.Invoke();
        this.gameObject.SetActive(false);
    }
    IEnumerator PuzzleStart()
    {
        yield return new WaitForSeconds(0.5f);
        GameManger.Instance.PauseGame();
        _lockInteractive.SetActive(false);
        _lockPuzzle.SetActive(true);
        _VC.SetActive(true);
        _puzzleStarts = true;
    }

}
