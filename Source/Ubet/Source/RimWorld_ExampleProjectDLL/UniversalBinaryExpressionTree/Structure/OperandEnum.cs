using Verse;

namespace Ubet
{
    public enum Operand
    {
        [Description("logical AND")]
        and,
        [Description("logical OR")]
        or,
        [Description("logical NOT")]
        not,

        [Description("empty operand")]
        empty
    }
}
