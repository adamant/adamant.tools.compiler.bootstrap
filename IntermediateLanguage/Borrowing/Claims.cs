//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Linq;
//using Adamant.Tools.Compiler.Bootstrap.Framework;
//using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow;

//namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.Borrowing
//{
//    /// <summary>
//    /// A list of claims outstanding
//    /// </summary>
//    [SuppressMessage("Naming", "CA1724:Type names should not match namespaces", Justification = "Obscure system namespace")]
//    public class Claims
//    {
//        private readonly List<Claim> claimsList = new List<Claim>();
//        private readonly HashSet<Claim> claimsSet = new HashSet<Claim>();

//        public IEnumerable<Claim> AsEnumerable()
//        {
//            return claimsList;
//        }

//        public bool Add(Claim claim)
//        {
//            var added = claimsSet.Add(claim);
//            if (added)
//                claimsList.Add(claim);
//            return added;
//        }

//        public void AddRange(IEnumerable<Claim> claims)
//        {
//            foreach (var claim in claims)
//                Add(claim);
//        }

//        public void AddRange(Claims claims)
//        {
//            foreach (var claim in claims.claimsList)
//                Add(claim);
//        }
//        public bool Remove(Claim claim)
//        {
//            var removed = claimsSet.Remove(claim);
//            if (removed)
//                claimsList.Remove(claim);
//            return removed;
//        }

//        public Claim? ClaimBy(Variable variable)
//        {
//            return claimsList.SingleOrDefault(c => c.Holder.Equals(variable));
//        }

//        public Owns? OwnedBy(Variable variable)
//        {
//            return claimsList.OfType<Owns>().SingleOrDefault(o => o.Holder.Equals(variable));
//        }

//        public Owns? OwnerOf(Lifetime lifetime)
//        {
//            return claimsList.OfType<Owns>().SingleOrDefault(o => o.Lifetime == lifetime);
//        }

//        /// <summary>
//        /// An object lifetime is shared if it is borrowed or aliased
//        /// </summary>
//        public bool IsShared(Lifetime lifetime)
//        {
//            return claimsList.OfType<IShares>().Any(c => c.Lifetime == lifetime);
//        }

//        public bool SequenceEqual(Claims other)
//        {
//            if (ReferenceEquals(null, other))
//                return false;
//            if (ReferenceEquals(this, other))
//                return true;
//            return claimsList.SequenceEqual(other.claimsList);
//        }

//        public override int GetHashCode()
//        {
//            return claimsList.GetHashCode();
//        }

//        /// <summary>
//        /// Each variable should refer to at most one lifetime, this returns that lifetime
//        /// </summary>
//        public Lifetime? LifetimeOf(Variable variable)
//        {
//            return claimsList.SingleOrDefault(c => c.Holder.Equals(variable))?.Lifetime;
//        }

//        /// <summary>
//        /// Removes all claims held by the given variables. This may cause other claims to be removed if
//        /// they were held only by the lifetimes released.
//        /// </summary>
//        public void Release(IEnumerable<Variable> variables)
//        {
//            var variableSet = variables.Cast<IClaimHolder>().ToHashSet();
//            var affectedLifetimes = new HashSet<Lifetime>();

//            for (var i = claimsList.Count - 1; i >= 0; i--)
//            {
//                var claim = claimsList[i];
//                if (variableSet.Contains(claim.Holder))
//                {
//                    affectedLifetimes.Add(claim.Lifetime);
//                    claimsList.RemoveAt(i);
//                    claimsSet.Remove(claim);
//                }
//            }

//            var lifetimes = affectedLifetimes.ToQueue();
//            while (lifetimes.TryDequeue(out var lifetime))
//            {
//                if (claimsList.Any(c => c.Lifetime == lifetime))
//                    continue;

//                // Nothing is holding this lifetime, remove it
//                for (var i = claimsList.Count - 1; i >= 0; i--)
//                {
//                    var claim = claimsList[i];
//                    if (claim.Holder.Equals(lifetime))
//                    {
//                        claimsList.RemoveAt(i);
//                        claimsSet.Remove(claim);

//                        // That may release other lifetimes
//                        if (affectedLifetimes.Add(claim.Lifetime))
//                            lifetimes.Enqueue(claim.Lifetime);
//                    }
//                }
//            }
//        }

//        public bool Any()
//        {
//            return claimsList.Any();
//        }

//        public bool IsAliased(Lifetime lifetime)
//        {
//            return claimsList.OfType<Aliases>().Any(s => s.Lifetime == lifetime);
//        }

//        public IClaimHolder CurrentBorrower(Lifetime lifetime)
//        {
//            return claimsList.OfType<IExclusiveClaim>().Last(c => c.Lifetime == lifetime).Holder;
//        }

//        public IEnumerable<Claim> ClaimsTo(Lifetime lifetime)
//        {
//            return claimsList.Where(c => c.Lifetime == lifetime);
//        }
//    }
//}
