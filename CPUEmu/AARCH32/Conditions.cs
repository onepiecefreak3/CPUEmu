using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPUEmu
{
    public partial class AARCH32
    {
        private bool CheckConditions(byte condition)
        {
            switch (condition)
            {
                case 0:
                    return _z;
                case 1:
                    return !_z;
                case 2:
                    return _c;
                case 3:
                    return !_c;
                case 4:
                    return _n;
                case 5:
                    return !_n;
                case 6:
                    return _v;
                case 7:
                    return !_v;
                case 8:
                    return _c && !_z;
                case 9:
                    return !_c || _z;
                case 10:
                    return _n == _v;
                case 11:
                    return _n != _v;
                case 12:
                    return !_z && (_n == _v);
                case 13:
                    return _z || (_n != _v);
                case 14:
                    return true;

                default:
                    Log?.Invoke(this, $"Unknown condition 0x{condition:X1}. Ignore instruction.");
                    return false;
            }
        }
        private void DisassembleConditions(byte condition)
        {
            switch (condition)
            {
                case 0:
                    _currentCondition = "EQ";
                    break;
                case 1:
                    _currentCondition = "NE";
                    break;
                case 2:
                    _currentCondition = "CS";
                    break;
                case 3:
                    _currentCondition = "CC";
                    break;
                case 4:
                    _currentCondition = "MI";
                    break;
                case 5:
                    _currentCondition = "PL";
                    break;
                case 6:
                    _currentCondition = "VS";
                    break;
                case 7:
                    _currentCondition = "VC";
                    break;
                case 8:
                    _currentCondition = "HI";
                    break;
                case 9:
                    _currentCondition = "LS";
                    break;
                case 10:
                    _currentCondition = "GE";
                    break;
                case 11:
                    _currentCondition = "LT";
                    break;
                case 12:
                    _currentCondition = "GT";
                    break;
                case 13:
                    _currentCondition = "LE";
                    break;
                case 14:
                case 15:
                    _currentCondition = "";
                    break;
            }
        }
    }
}
