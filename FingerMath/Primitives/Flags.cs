namespace FingerMath.Primitives
{
    using System;

    public class Flags
    {
        private int value;

        public bool IsFlagSet(int flag)
        {
            flag = 1 << flag;
            return (this.value & flag) != 0;
        }

        public void SetFlagExclusive(int flag)
        {
            this.value = 1 << flag;
        }

        public void SetFlag(int flag)
        {
            this.value = this.value | 1 << flag;
        }

        public void UnsetFlag(int flag)
        {
            flag = 1 << flag;
            this.value = this.value & ~flag;
        }

        public void InvertFlags()
        {
            this.value = ~this.value;
        }

        public override string ToString()
        {
            return Convert.ToString(this.value, 2);
        }
    }
}