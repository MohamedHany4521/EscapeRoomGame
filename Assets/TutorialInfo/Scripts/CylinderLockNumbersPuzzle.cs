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
    private int _cylinder01Step = 0;
    private int _cylinder02Step = 0;
    private int _cylinder03Step = 0;
    private int _cylinder04Step = 0;

    [SerializeField] private GameObject _cylinder01;
    [SerializeField] private GameObject _cylinder02;
    [SerializeField] private GameObject _cylinder03;
    [SerializeField] private GameObject _cylinder04;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
            if (Input.GetKeyDown(KeyCode.W))
            {
                _cylinder01Step = (_cylinder01Step + 1) % 10;
                _cylinder01.transform.localEulerAngles = new Vector3(_cylinder01Step * 36f, 0f, 0f);
            }
        }
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
