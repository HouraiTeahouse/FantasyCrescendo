using System;

namespace HouraiTeahouse
{
    [AttributeUsage(AttributeTargets.Class)]
    public class OptionCategory : Attribute
    {
        private string name;
        public OptionCategory(string name, params string[] args)
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

    public class UISlider : Option
    {
        float min;
        public float Min
        {
            get
            {
                return min;
            }
        }

        float max;
        public float Max
        {
            get
            {
                return max;
            }
        }

        public UISlider(string name, float min, float max) : base(name)
        {
            this.min = min;
            this.max = max;
        }
    }

    public class UIToggle : Option
    {
        public UIToggle(string name) : base(name)
        {
        }
    }

    public class UIIntField : Option
    {
        public UIIntField(string name) : base(name)
        {
        }
    }

    public class UITextField : Option
    {
        public UITextField(string name) : base(name)
        {
        }
    }

    [OptionCategory("Audio Options")]
    public class AudioOptions
    {
        [UISlider("Master Volume", 0.0f, 1.0f)]
        public float Master { get; set; }
        [UISlider("Background Music", 0.0f, 1.0f)]
        public float Bgm { get; set; }
        [UISlider("Sound Effects", 0.0f, 1.0f)]
        public float Sfx { get; set; }
    }

    [OptionCategory("Some Options")]
    public class SomeOptions
    {
        [UISlider("Some Float", -100.0f, 100.0f)]
        public float SomeFloat { get; set; }
        [UIToggle("Some Bool")]
        public bool SomeBool { get; set; }
        [UIIntField("Some Int")]
        public int SomeInt { get; set; }
        [UITextField("Some String")]
        public string SomeString { get; set; }
    }
}