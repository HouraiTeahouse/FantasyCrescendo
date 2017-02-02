using System;

namespace HouraiTeaHouse
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OptionCategory : Attribute
    {
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
        public float Master { get; set; }
        [Option("BGM")]
        public float Bgm { get; set; }
        [Option("SFX")]
        public float Sfx { get; set; }
    }

    [OptionCategory("Some Options")]
    public class SomeOptions
    {
        [Option("Some Float")]
        public float SomeFloat { get; set; }
        [Option("Some Bool")]
        public bool SomeBool { get; set; }
        [Option("Some Int")]
        public int SomeInt { get; set; }
        [Option("Some String")]
        public string SomeString { get; set; }
    }
}