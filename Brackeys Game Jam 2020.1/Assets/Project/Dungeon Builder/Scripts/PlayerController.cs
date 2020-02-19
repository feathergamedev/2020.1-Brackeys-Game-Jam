using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Actor
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                Move(Direction.Right);
            }
            else
            {
                Move(Direction.Left);
            }
        }
        else if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                Move(Direction.Down);
            }
            else
            {
                Move(Direction.Up);
            }
        }
    }

    void Move(Direction direction)
    {
        var targetpos = Coordinate + direction.ToVec2();
        MapManager.instance.MoveTo(this, targetpos);
    }
}
