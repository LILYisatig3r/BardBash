using System;
namespace Assets._Scripts
{
    public enum E_Team
    {
        red = 0,
        blue = 1,
        green = 2,
        yellow = 3
    }

    public enum E_BattleState
    {
        nil = -1,
        cameraControl = 0,
        end = 1,
        intro = 2,
        overview = 3,
        playerControl = 4,
        preview = 5,
        transitioning = 6,
        playerMenu = 7,
        enemyControl = 8,
        turnEnded = 9
    }

    public enum E_CameraState
    {
        following = 0,
        overview = 1,
        controllable = 2
    }

    [Flags]
    public enum S_Key
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
        Q = 16,
        W = 32,
        E = 64,
        R = 128
    }

}
