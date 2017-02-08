using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PowerAssert;

namespace Richiban.Func.Tests.Unit
{
    [TestFixture]
    public sealed class LazyListTests
    {
        [Test]
        public void Count_returns_zero_for_empty_enumerable()
        {
            var enumerable = Enumerable.Empty<object>();

            var classUnderTest = new LazyList<object>(enumerable);

            PAssert.IsTrue(() => classUnderTest.Count == 0);
        }

        [Test]
        public void Enumeration_returns_same_results_as_contained_enumerable()
        {
            var enumerable = GetRandomList();

            var classUnderTest = new LazyList<int>(enumerable);

            PAssert.IsTrue(() => classUnderTest.SequenceEqual(enumerable));
        }

        [Test]
        public void Retrieving_first_item_twice_does_not_reevaluate_enumerable()
        {
            var trackingCount = new Ref<int> {Value = 0};

            var enumerable = MakeTrackingSequence(trackingCount);

            var classUnderTest = new LazyList<int>(enumerable);

            PAssert.IsTrue(() => trackingCount.Value == 0);

            var a = classUnderTest[0];

            PAssert.IsTrue(() => trackingCount.Value == 1);

            var b = classUnderTest[0];
            
            PAssert.IsTrue(() => trackingCount.Value == 1);
        }

        private IEnumerable<int> MakeTrackingSequence(Ref<int> trackingCount)
        {
            trackingCount.Value = trackingCount.Value + 1;
            yield return 0;
        }

        private class Ref<T>
        {
            public T Value { get; set; }
        }

        private static readonly Random Random = new Random();

        private static int[] GetRandomList()
        {
            var count = Random.Next(maxValue: 100);

            var ints = new int[count];

            for (var i = 0; i < count; i++)
            {
                ints[i] = Random.Next(maxValue: 1000);
            }

            return ints;
        }
    }
}