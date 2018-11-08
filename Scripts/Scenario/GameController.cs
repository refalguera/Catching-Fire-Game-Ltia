using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [HideInInspector]
    public bool startgame;

    [HideInInspector]
    public bool _gameover;

    [SerializeField]
    private Camera _cam;

    [SerializeField]
    private Transform[] _points;
    private List<Lines> _lines;
    
    [HideInInspector]
    public Queue<GameObject> _deadplayers;
  
    private void Start()
    {
        _lines = new List<Lines>();
        DrawLinesPoins();
    }

//Check start and end of game
    private void Update()
    {
        if (startgame)
        {
            if (_gameover)
            {
                //If you lose, it shows the end game screen
            }
            //Respawn player who died during the game
            RespwanPlayers();
        }
    }

    void DrawLinesPoins()
    {
        for (int i = 0; i < _points.Length; i++)
        {
            Lines line = new Lines();
            line.lines = Physics2D.Linecast(_points[i].position,_points[i++].position);
            line._passThrough = false;
            _lines.Add(line);
        }
    }
 
 //Respawn player who died during the game. Players who need to be re-placed in the game are in a queue
    void RespwanPlayers()
    {
        if(_deadplayers != null)
        {
            while(_deadplayers.Count > 0)
            {
                GameObject _player = _deadplayers.Dequeue();
                _player.transform.position = _cam.transform.position;
            }
        }
    }
    struct Lines
    {
        public RaycastHit2D lines;
        public bool _passThrough;
    }
}
