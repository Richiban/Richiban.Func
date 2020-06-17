using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

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

            classUnderTest.Count.ShouldBe(0);
        }

        [Test]
        public void Enumeration_returns_same_results_as_contained_enumerable()
        {
            var enumerable = GetRandomList();

            var classUnderTest = new LazyList<int>(enumerable);

            classUnderTest.ShouldBe(enumerable);
        }

        [Test]
        public void Retrieving_first_item_twice_does_not_reevaluate_enumerable()
        {
            var trackingCount = new Ref<int> { Value = 0 };

            var enumerable = MakeTrackingSequence(trackingCount);

            var classUnderTest = new LazyList<int>(enumerable);

            trackingCount.Value.ShouldBe(0);

            var a = classUnderTest[0];

            trackingCount.Value.ShouldBe(1);

            var b = classUnderTest[0];

            trackingCount.Value.ShouldBe(1);
        }

        [Test]
        public void Finishing_the_enumeration_disposes_the_given_enumerator()
        {
            var enumerable = new ObservableEnumerable<int>();

            var subjectUnderTest = new LazyList<int>(enumerable);

            var enumerator = enumerable.Enumerator;

            enumerator.IsDisposed.ShouldBeFalse();

            foreach (var item in subjectUnderTest)
            { }

            enumerator.IsDisposed.ShouldBeTrue();
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

        private class ObservableEnumerator<T> : IEnumerator<T>
        {
            public T Current { get; }
            public bool IsDisposed { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() => IsDisposed = true;

            public bool MoveNext() => false;

            public void Reset() => throw new NotImplementedException();
        }

        private class ObservableEnumerable<T> : IEnumerable<T>
        {
            public ObservableEnumerator<T> Enumerator = new ObservableEnumerator<T>();
            public IEnumerator<T> GetEnumerator() => Enumerator;

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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