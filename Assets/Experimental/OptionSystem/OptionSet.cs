using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class)]
public class OptionCategory : Attribute {
    private string name;
    public OptionCategory(string name)
    {
        this.name = name;
    }

    public string Name
    {
        get
        {
            return name;
        }
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class Option : Attribute
{
    private string name;
    public Option(string name)
    {
        this.name = name;
    }

    public string Name
    {
        get
        {
            return name;
        }
    }
}

[OptionCategory("Audio Options")]
public class AudioOptions
{
    [Option("Master")]
    public float master { get; set; }
    [Option("BGM")]
    public float bgm { get; set; }
    [Option("SFX")]
    public float sfx { get; set; }

    public void Print()
    {
        Debug.Log(master);
        Debug.Log(bgm);
        Debug.Log(sfx);
    }
}

[OptionCategory("Some Options")]
public class SomeOptions
{
    [Option("Some Float")]
    public float someFloat { get; set; }
    [Option("Some Bool")]
    public bool someBool { get; set; }
    [Option("Some Int")]
    public int someInt { get; set; }
    [Option("Some String")]
    public string someString { get; set; }

    public void Print()
    {
        Debug.Log(someFloat);
        Debug.Log(someBool);
        Debug.Log(someInt);
        Debug.Log(someString);
    }
}