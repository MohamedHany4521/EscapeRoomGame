using System.Collections;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;
public class CylinderLockSymbolsPuzzle : MonoBehaviour
{
    [Header("Puzzle Info")]
    [SerializeField] private string _puzzleCode;
    [SerializeField] private UnityEvent _event;

    [Header("Objects In Puzzle")]
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
                    _cylinder01Step = (_cylinder01Step + 1) % 5;
                    _cylinder01.transform.localEulerAngles = new Vector3(_cylinder01Step * 72f, 0f, 0f);
                    Cylinder01Values();
                }
                if (_currentCylinder == 1)
                {
                    _cylinder02Step = (_cylinder02Step + 1) % 5;
                    _cylinder02.transform.localEulerAngles = new Vector3(_cylinder02Step * 72f, 0f, 0f);
                    Cylinder02Values();
                }
                if (_currentCylinder == 2)
                {
                    _cylinder03Step = (_cylinder03Step + 1) % 5;
                    _cylinder03.transform.localEulerAngles = new Vector3(_cylinder03Step * 72f, 0f, 0f);
                    Cylinder03Values();
                }
                if (_currentCylinder == 3)
                {
                    _cylinder04Step = (_cylinder04Step + 1) % 5;
                    _cylinder04.transform.localEulerAngles = new Vector3(_cylinder04Step * 72f, 0f, 0f);
                    Cylinder04Values();
                }
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (_currentCylinder == 0)
                {
                    _cylinder01Step = (_cylinder01Step - 1 + 5) % 5;
                    _cylinder01.transform.localEulerAngles = new Vector3(_cylinder01Step * 72f, 0f, 0f);
                    Cylinder01Values();
                }
                if (_currentCylinder == 1)
                {
                    _cylinder02Step = (_cylinder02Step - 1 + 5) % 5;
                    _cylinder02.transform.localEulerAngles = new Vector3(_cylinder02Step * 72f, 0f, 0f);
                    Cylinder02Values();
                }
                if (_currentCylinder == 2)
                {
                    _cylinder03Step = (_cylinder03Step - 1 + 5) % 5;
                    _cylinder03.transform.localEulerAngles = new Vector3(_cylinder03Step * 72f, 0f, 0f);
                    Cylinder03Values();
                }
                if (_currentCylinder == 3)
                {
                    _cylinder04Step = (_cylinder04Step - 1 + 5) % 5;
                    _cylinder04.transform.localEulerAngles = new Vector3(_cylinder04Step * 72f, 0f, 0f);
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
                _cylinder01Leter = "*";
                break;
            case 1:
                _cylinder01Leter = "☆";
                break;
            case 2:
                _cylinder01Leter = "♤";
                break;
            case 3:
                _cylinder01Leter = "♧";
                break;
            case 4:
                _cylinder01Leter = "◇";
                break;
            default:
                _cylinder01Leter = "*";
                break;
        }
        Debug.Log("Symbol is: " + _cylinder01Leter + "and case is " + _cylinder01Step);
        CheckPuzzle();
    }

    public void Cylinder02Values()
    {
        switch (_cylinder02Step)
        {
            case 0:
                _cylinder02Leter = "♧";
                break;
            case 1:
                _cylinder02Leter = "◇";
                break;
            case 2:
                _cylinder02Leter = "*";
                break;
            case 3:
                _cylinder02Leter = "☆";
                break;
            case 4:
                _cylinder02Leter = "♤";
                break;
            default:
                _cylinder02Leter = "♧";
                break;
        }
        Debug.Log("Symbol is: " + _cylinder02Leter + "and case is " + _cylinder02Step);
        CheckPuzzle();
    }
    public void Cylinder03Values()
    {
        switch (_cylinder03Step)
        {
            case 0:
                _cylinder03Leter = "♤";
                break;
            case 1:
                _cylinder03Leter = "♧";
                break;
            case 2:
                _cylinder03Leter = "◇";
                break;
            case 3:
                _cylinder03Leter = "*";
                break;
            case 4:
                _cylinder03Leter = "☆";
                break;
            default:
                _cylinder03Leter = "♤";
                break;
        }
        Debug.Log("Symbol is: " + _cylinder03Leter + "and case is " + _cylinder03Step);
        CheckPuzzle();
    }
    public void Cylinder04Values()
    {
        switch (_cylinder04Step)
        {
            case 0:
                _cylinder04Leter = "◇";
                break;
            case 1:
                _cylinder04Leter = "*";
                break;
            case 2:
                _cylinder04Leter = "☆";
                break;
            case 3:
                _cylinder04Leter = "♤";
                break;
            case 4:
                _cylinder04Leter = "♧";
                break;
            default:
                _cylinder04Leter = "◇";
                break;
        }
        Debug.Log("Symbol is: " + _cylinder04Leter + "and case is " + _cylinder04Step);
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
