namespace FingerMath.Matching
{
    using System.Text;

    public class Matcher
    {
        public interface IMatcherEntity
        {
            BitSet Bits { get; }
        }

        private readonly BitSet allSet = new BitSet();
        private readonly BitSet exclusionSet = new BitSet();
        private readonly BitSet oneSet = new BitSet();

        public Matcher All(BitSet bits)
        {
            this.allSet.Or(bits);
            return this;
        }

        public Matcher Exclude(BitSet bits)
        {
            this.exclusionSet.Or(bits);
            return this;
        }

        public Matcher One(BitSet bits)
        {
            this.oneSet.Or(bits);
            return this;
        }

        public static Matcher Empty()
        {
            return new Matcher();
        }

        public bool IsMatched(BitSet bits)
        {

            // Check if the entity possesses ALL of the components defined in the aspect.
            if (!this.allSet.IsEmpty())
            {
                for (var i = this.allSet.NextSetBit(0); i >= 0; i = this.allSet.NextSetBit(i + 1))
                {
                    if (!bits.Get(i))
                    {
                        return false;
                    }
                }
            }

            // If we are STILL interested,
            // Check if the entity possesses ANY of the exclusion components, if it does then the system is not interested.
            if (!this.exclusionSet.IsEmpty() && this.exclusionSet.Intersects(bits))
            {
                return false;
            }

            // If we are STILL interested,
            // Check if the entity possesses ANY of the components in the oneSet. If so, the system is interested.
            if (!this.oneSet.IsEmpty() && !this.oneSet.Intersects(bits))
            {
                return false;
            }

            return true;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(1024);

            builder.AppendLine("Matcher:");
            builder.AppendLine($" -  Requires flags: {this.allSet}");
            builder.AppendLine($" -  Has none of the flags: {this.exclusionSet}");
            builder.AppendLine($" -  Has at least one of the flags: {this.oneSet}");

            return builder.ToString();
        }
    }
}
