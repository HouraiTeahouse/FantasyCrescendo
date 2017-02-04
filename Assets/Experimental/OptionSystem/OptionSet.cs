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

        float defValue;
        public float DefValue
        {
            get
            {
                return defValue;
            }
        }

        public UISlider(string name, float min, float max, float defValue = 0.0f) : base(name)
        {
            this.min = min;
            this.max = max;
        }
    }

    public class UIToggle : Option
    {
        bool defValue;
        public bool DefValue
        {
            get
            {
                return defValue;
            }
        }
        public UIToggle(string name, bool defValue = false) : base(name)
        {
            this.defValue = defValue;
        }
    }

    public class UIIntField : Option
    {
        int defValue;
        public int DefValue
        {
            get
            {
                return defValue;
            }
        }
        public UIIntField(string name, int defValue = 0) : base(name)
        {
            this.defValue = defValue;
        }
    }

    public class UITextField : Option
    {
        string defValue;
        public string DefValue
        {
            get
            {
                return defValue;
            }
        }
        public UITextField(string name, string defValue = "") : base(name)
        {
            this.defValue = defValue;
        }
    }

    [OptionCategory("Audio Options")]
    public class AudioOptions
    {
        [UISlider("Master Volume", 0.0f, 1.0f, 1.0f)]
        public float Master { get; set; }
        [UISlider("Background Music", 0.0f, 1.0f, 1.0f)]
        public float Bgm { get; set; }
        [UISlider("Sound Effects", 0.0f, 1.0f, 1.0f)]
        public float Sfx { get; set; }
    }

    [OptionCategory("Some Options")]
    public class SomeOptions
    {
        [UISlider("Some Float", -100.0f, 100.0f, 60.0f)]
        public float SomeFloat { get; set; }
        [UIToggle("Some Bool", true)]
        public bool SomeBool { get; set; }
        [UIIntField("Some Int", 123)]
        public int SomeInt { get; set; }
        [UITextField("Some String", "Okayama no kenboku")]
        public string SomeString { get; set; }
    }
}