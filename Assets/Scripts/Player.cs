﻿using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private static Player Singleton;
    static public Player Get() { return Singleton; }
    public Transform _playerTransform;

    private Tools _tools;

    private Player _player = null;

    public float HorizontalSpeed;
    public float VerticalSpeed;
    private float _step;
    private bool _moveDown = false;
    private bool _moveUp = false;
    private float _leftStop = -4.05f;
    private float _rightStop = 3.83f;

    private bool _keyUp() { return (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)); }
    private bool _keyDown() { return (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)); }
    private bool _keyShoot() { return Input.GetKeyDown(KeyCode.Space); }

    public Transform _rowA;
    public Transform _rowB;
    public Transform _rowC;
    public Transform _rowD;

    private float _lineA = -0.53f;
    private float _lineB = -1.525f;
    private float _lineC = -2.582f;

    public char Row;

    [SerializeField]
    private TextMesh _scoreGui = null;

    //Bullet Pool
    public GameObject[] Bullets;
    private List<GameObject> _bulletPool;
    public int bulletAmount = 100;

    public float FireRate;
    private float _nextFire;

    //Scoring Stuff
    private int _score = 100;
    public int GetScore()
    {
        return _score;
    }
    public void PickUpCash()
    {
        _score += 100;
    }
    public void FireLaserCash()
    {
        _score -= 25;
    }
    public void AnswerCorrent()
    {
        _score += 200;
    }
    public void AnswerWrong()
    {
        _score -= 200;
    }

    public void Awake()
    {
        Singleton = this;
    }

    private void Start()
    {
        _score = 100;
        _playerTransform = GetComponent<Transform>();
        _tools = new Tools();
        transform.position = new Vector2(-4,_rowB.position.y);
        Row = 'B';

        //Instantiate the BUllet pool at the begining of the lvl
        _bulletPool = new List<GameObject>();
        foreach (var bullet in Bullets)
        {
            for (int i = 0; i < bulletAmount; i++)
            {
                GameObject obj = Instantiate(bullet);
                obj.SetActive(false);
                _bulletPool.Add(obj);
            }
        }

        _player = Player.Get();
    }

    private void FixedUpdate()
    {
        _scoreGui.text = _score.ToString();
    }

    // Update is called once per frame
    void Update () {
        _step = VerticalSpeed * Time.deltaTime;
        UPdatePlayer();

        //Set active BUllet and Fire.
        if (_keyShoot() && (Time.time > _nextFire) && _score >= 25)
        {
            _nextFire = Time.time + FireRate;
            _tools.SpawnObjFromPool(_bulletPool, _playerTransform);
            FireLaserCash();
        }

        DeterminePlayerPosition();

        if (_score <= 0)
        {
            _score = 0;
        }
    }

    public void DeterminePlayerPosition()
    {
        if (transform.position.y > _lineA)
        {
            Row = 'A';
        }
        if (transform.position.y < _lineA && transform.position.y > _lineB)
        {
            Row = 'B';
        }
        if (transform.position.y < _lineB && transform.position.y > _lineC)
        {
            Row = 'C';
        }
        if (transform.position.y < _lineC)
        {
            Row = 'D';
        }
    }

    private void UPdatePlayer()
    {
        #region Move Up
        if (_keyUp() && (_moveDown == false) && (transform.position.y != _rowA.position.y))
        {
            _moveUp = true;
        }
        //B to A
        if (_moveUp && (transform.position.y >= _rowB.position.y && transform.position.y < _rowA.position.y) )
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, _rowA.position.y), _step);

            if (transform.position.y == _rowA.position.y)
            {
                _moveUp = false;
            }
        }
        //C To B
        if (_moveUp && (transform.position.y >= _rowC.position.y && transform.position.y < _rowB.position.y))
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, _rowB.position.y), _step);

            if (transform.position.y == _rowB.position.y)
            {
                _moveUp = false;
            }
        }
        //D to C
        if (_moveUp && (transform.position.y >= _rowD.position.y && transform.position.y < _rowC.position.y))
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, _rowC.position.y), _step);

            if (transform.position.y == _rowC.position.y)
            {
                _moveUp = false;
            }
        }
        #endregion

        #region Move Down
        if (_keyDown() && (_moveUp == false) && (transform.position.y != _rowD.position.y))
        {
            _moveDown = true;
        }
        //C to D
        if (_moveDown && (transform.position.y <= _rowC.position.y && transform.position.y > _rowD.position.y))
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, _rowD.position.y), _step);

            if (transform.position.y == _rowD.position.y)
            {
                _moveDown = false;
            }
        }
        //B to C
        if (_moveDown && (transform.position.y <= _rowB.position.y && transform.position.y > _rowC.position.y))
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, _rowC.position.y), _step);

            if (transform.position.y == _rowC.position.y)
            {
                _moveDown = false;
            }
        }
        //A to B
        if (_moveDown && (transform.position.y <= _rowA.position.y && transform.position.y > _rowB.position.y))
        {
            transform.position = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.y), new Vector2(transform.position.x, _rowB.position.y), _step);

            if (transform.position.y == _rowB.position.y)
            {
                _moveDown = false;
            }
        }
        #endregion

        #region Move Left and Right
        if (transform.position.x <= _leftStop)
        {
            _playerTransform.position = new Vector2(_leftStop, _playerTransform.position.y);
        }
        if (transform.position.x >= _rightStop)
        {
            _playerTransform.position = new Vector2(_rightStop, _playerTransform.position.y);
        }
        transform.Translate(-Vector3.left * Input.GetAxis("Horizontal") * HorizontalSpeed * Time.deltaTime);
        #endregion
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Cash")
        {
            PickUpCash();
        }
    }
}
