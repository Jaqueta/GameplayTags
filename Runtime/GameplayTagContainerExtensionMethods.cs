using System.Collections.Generic;
using UnityEngine.Pool;

namespace BandoWare.GameplayTags
{
   public static class GameplayTagContainerExtensionMethods
   {
      public static bool HasTag<T>(this T container, GameplayTag gameplayTag)
         where T : IReadOnlyGameplayTagContainer
      {
         return container.Indices.Implicit != null
            && BinarySearchUtility.Search(container.Indices.Implicit, gameplayTag.RuntimeIndex) >= 0;
      }

      public static bool HasTagExact<T>(this T container, GameplayTag gameplayTag)
         where T : IReadOnlyGameplayTagContainer
      {
         return container.Indices.Explicit != null
            && BinarySearchUtility.Search(container.Indices.Explicit, gameplayTag.RuntimeIndex) >= 0;
      }

      public static bool HasAny<T, U>(this T container, in U other)
         where T : IReadOnlyGameplayTagContainer
         where U : IReadOnlyGameplayTagContainer
      {
         return HasAnyInternal(container.Indices.Implicit, other?.Indices.Explicit);
      }

      public static bool HasAnyExact<T, U>(this T container, in U other)
         where T : IReadOnlyGameplayTagContainer
         where U : IReadOnlyGameplayTagContainer
      {
         return HasAnyInternal(container.Indices.Explicit, other?.Indices.Explicit);
      }

      private static bool HasAnyInternal(List<int> a, List<int> b)
      {
         if (a is null or { Count: 0 } || b is null or { Count: 0 })
            return false;

         // Early-out by range
         if (a[^1] < b[0] || b[^1] < a[0])
            return false;

         int i = 0, j = 0;
         while (i < a.Count && j < b.Count)
         {
            int av = a[i];
            int bv = b[j];

            if (av == bv)
               return true;
            if (av < bv)
               i++;
            else
               j++;
         }

         return false;
      }

      private static bool HasAllInternal(List<int> a, List<int> b)
      {
         if (b is null or { Count: 0 })
            return true;

         if (a is null or { Count: 0 })
            return false;

         // Early-out by range
         if (b[0] < a[0] || b[^1] > a[^1])
            return false;

         int i = 0, j = 0;
         while (i < a.Count && j < b.Count)
         {
            int av = a[i];
            int bv = b[j];

            if (av == bv)
            {
               j++;
               i++;
            }
            else if (av < bv)
               i++;
            else
               return false;
         }

         return j == b.Count;
      }

      public static bool HasAll<T, U>(this T container, in U other)
         where T : IReadOnlyGameplayTagContainer
         where U : IReadOnlyGameplayTagContainer
      {
         return HasAllInternal(container.Indices.Implicit, other?.Indices.Explicit);
      }

      public static bool HasAll<T, U, V>(this T container, in U otherA, in V otherB)
         where T : IReadOnlyGameplayTagContainer
         where U : IReadOnlyGameplayTagContainer
         where V : IReadOnlyGameplayTagContainer
      {
         if (otherA.IsEmpty && otherB.IsEmpty)
            return true;

         if (otherA.IsEmpty)
            return HasAll(container, otherB);

         if (otherB.IsEmpty)
            return HasAll(container, otherA);

         using (GenericPool<GameplayTagContainer>.Get(out GameplayTagContainer intersection))
         {
            intersection.AddIntersection(otherA, otherB);
            bool hasAll = HasAll(container, intersection);
            intersection.Clear();

            return hasAll;
         }
      }

      public static bool HasAllExact<T, U>(this T container, in U other)
         where T : IReadOnlyGameplayTagContainer
         where U : IReadOnlyGameplayTagContainer
      {
         return HasAllInternal(container.Indices.Explicit, other?.Indices.Explicit);
      }
   }
}
