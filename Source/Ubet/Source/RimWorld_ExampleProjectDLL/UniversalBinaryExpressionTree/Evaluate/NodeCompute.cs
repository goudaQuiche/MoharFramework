﻿using Verse;

namespace Ubet
{
    public static class NodeCompute
    {
        public static bool TrunkNodeComputation(this Thing t, Leaf trunk, bool debug = false)
        {
            //return t.RecursiveNodeComputation(trunk);

            bool result = t.RecursiveNodeComputation(trunk, 0, debug);

            if (debug) Log.Warning("final result:" + result);

            return result;
        }

        public static bool RecursiveNodeComputation(this Thing t, Leaf branch, int depth = 0, bool debug = false)
        {
            if (depth == 0) {
                if (debug) Log.Warning("RecursiveNodeComputation - trunk detected");

                if (branch.HasInitialCondition) {
                    if (!t.MainCheck(branch.initialCondition, debug))
                    {
                        if (debug)
                            Log.Warning("Initial condition " + branch.initialCondition.Description + ", result=false");
                        return false;
                    } else if (branch.leaf == null)
                    {
                        if (debug)
                            Log.Warning("Initial condition " + branch.initialCondition.Description + ", result=true");
                        return true;
                    }
                }
            }
            else
            {
                if (debug) Log.Warning("RecursiveNodeComputation - leaf depth: " + depth);
            }

            bool branchValue = true;
            if (branch.operand == Operand.and)
                branchValue = true;
            if (branch.operand == Operand.or)
                branchValue = false;

            foreach (Leaf leaf in branch.leaf)
            {
                if (debug)
                {
                    string debugStr = "Browsing leaves; depth=" + depth;
                    debugStr += "; operand:" + branch.operand.DescriptionAttr();
                    debugStr += (branch.condition == null) ? ("; no condition") : ("; condition:" + branch.condition.Description);

                    Log.Warning(debugStr);
                }
                bool leafValue;

                if (leaf.leaf.NullOrEmpty() && leaf.condition != null)
                {
                    leafValue = t.MainCheck(leaf.condition, debug);
                }
                else
                {
                    leafValue = t.RecursiveNodeComputation(leaf, depth + 1, debug);
                }

                if (branch.operand == Operand.and)
                {
                    if (!leafValue)
                    {
                        if (debug) Log.Warning("depth=" + depth + " - AND && false : fast exit with false");
                        return false;
                    }

                    branchValue &= leafValue;

                } else if (branch.operand == Operand.or)
                {
                    if (leafValue)
                    {
                        if (debug) Log.Warning("depth=" + depth + " - OR && true : fast exit with true");
                        return true;
                    }

                    branchValue |= leafValue;
                }
                else 
                {
                    if (debug && branch.operand == Operand.not) Log.Warning("depth=" + depth + " - NOT");

                    return (leaf.operand == Operand.not) ? !leafValue : leafValue;
                }
            }

            return branchValue;
        }
    }
}
