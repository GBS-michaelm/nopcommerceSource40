using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Checkout.GBS.Models
{

    public class Option
    {
        public int Id { get; set; }
    }
    public class SelectedOption
    {
        public Option Option { get; set; }
        public OptionValue[] Value { get; set; }
    }

    public abstract class OptionValue { }

    public class IdOptionValue : OptionValue
    {
        public int Id { get; set; }
        public string Input { get; set; }
        //public string Title { get; set; }
        //public float Price { get; set; }

        public override string ToString()
        {
            return Id.ToString();
        }
    }

    public class StringOptionValue : OptionValue
    {
        public string StringValue { get; set; }

        public override string ToString()
        {
            return StringValue;
        }
    }


}