using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PowerAssert;

namespace Richiban.Func.Tests.Unit
{
    [TestFixture]
    public sealed class ChainTests
    {
        [SetUp]
        public void SetUp() { }

        [Test]
        public void CallCountIsRecordedWhenEnumeratingObservableSequenceMultipleTimes()
        {
            var callCount = 0;

            var sequence = new ObservableSequence(onGetEnumerator: () => { callCount++; });

            foreach (var item in sequence) { }
            foreach (var item in sequence) { }

            Assert.That(callCount, Is.EqualTo(2));
        }

        [Test]
        public void EnumeratingChainMultipleTimesEnumeratesSourceOnlyOnce()
        {
            var callCount = 0;

            var sequence = new ObservableSequence(onGetEnumerator: () => { callCount++; });
            var chain = new Chain<int>(sequence);

            foreach (var item in chain) { }
            foreach (var item in chain) { }

            Assert.That(callCount, Is.EqualTo(1));
        }

        [Test]
        public void EnumeratingChainMultipleTimesYieldsEachItemOnlyOnce()
        {
            var yieldedItems = new List<int>();

            var sequence = new ObservableSequence(onYield: i => yieldedItems.Add(i));
            var chain = new Chain<int>(sequence);

            foreach (var item in chain) { }

            var yieldedItemsCopy = yieldedItems.ToList();

            foreach (var item in chain) { }

            Assert.That(yieldedItemsCopy, Is.EquivalentTo(yieldedItems));
        }

        [Test]
        public void EnumeratingChainYieldsSameItemsAsOriginalSequence()
        {
            var count = Random.Next(100);
            var sequence = Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList();

            var chain = new Chain<Guid>(sequence);

            var zipped = sequence.Zip(chain, (x, y) => (x, y));

            var unequalItems = zipped.Where(t => t.Item1 != t.Item2);

            Assert.That(unequalItems, Is.Empty);
        }

        [Test]
        public void ChainIsSameLengthAsOriginalSequence()
        {
            var count = Random.Next(100);
            var sequence = Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList();
            var chain = new Chain<Guid>(sequence);

            var sequenceCount = sequence.Count();
            var chainCount = chain.Count();

            Assert.That(chainCount, Is.EqualTo(sequenceCount));
        }

        [Test]
        public void HeadThrowsExceptionWhenChainIsEmpty()
        {
            var emptyChain = new Chain<object>(Enumerable.Empty<object>());

            var ex = Assert.Throws<InvalidOperationException>(() => Console.Write(emptyChain.Head));

            Assert.That(ex.Message, Is.EqualTo("Chain is empty"));
        }

        [Test]
        public void HeadReturnsSingleElement()
        {
            var element = Random.Next();
            var chain = new Chain<int>(new [] { element });

            Assert.That(chain.Head, Is.EqualTo(element));
        }

        [Test]
        public void DeconstructionYieldsHeadAsFirstElement()
        {
            var element = Random.Next();
            var chain = new Chain<int>(new[] { element });
            var (first, _) = chain;

            PAssert.IsTrue(() => first == element);
        }

        [Test]
        public void DeconstructionYieldsTailAsSecondElement()
        {
            var element = Random.Next();
            var chain = new Chain<int>(new[] { element });
            var (_, second) = chain;

            PAssert.IsTrue(() => second == chain.Tail);
        }

        private static readonly Random Random = new Random();
        private sealed class ObservableSequence : IEnumerable<int>
        {
            private readonly Action _onGetEnumerator;
            private readonly Action<int> _onYield;
            private readonly Random _random = new Random();

            public ObservableSequence(Action onGetEnumerator = null, Action<int> onYield = null)
            {
                _onGetEnumerator = onGetEnumerator;
                _onYield = onYield;
            }

            public IEnumerator<int> GetEnumerator()
            {
                _onGetEnumerator?.Invoke();
                var count = _random.Next(100);

                while (count-- > 0)
                {
                    var item = _random.Next();
                    _onYield?.Invoke(item);
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}