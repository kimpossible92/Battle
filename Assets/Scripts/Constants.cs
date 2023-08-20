﻿using UnityEngine;
using System.Collections;

public static class Constants 
{

    public static readonly int MaxRows = 4;
    public static readonly int MaxColumns = 4;
    public static readonly int MaxSize = MaxRows * MaxColumns;
}

enum GameStated1
{
    Awaked,
    Start,
    Playing,
    Animating,
    End
}
