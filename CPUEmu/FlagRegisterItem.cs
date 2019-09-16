using System;

namespace CPUEmu
{
    public class FlagRegisterItem
    {
        public string Name { get; }
        public object Value { get; private set; }

        public FlagRegisterItem(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public void SetValue(object newValue)
        {
            try
            {
                Value = Convert.ChangeType(newValue, Value.GetType());
            }
            catch (Exception e)
            {
                // TODO: Give more information
                // catch any conversion exception and change nothing
            }
        }

        public override string ToString()
        {
            return $"[{Name}]: {Value:X8}";
        }
    }
}
