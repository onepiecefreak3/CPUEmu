namespace architecture_avr.Support
{
    static class FlagHelper
    {
        public static bool IsTwoComplementCarry(byte v1, byte v2, byte res, bool add)
        {
            return !add ?
                 v1 >> 7 != v2 >> 7 && v1 >> 7 != res >> 7 :
                 v1 >> 7 == v2 >> 7 && v1 >> 7 != res >> 7;
        }
    }
}
