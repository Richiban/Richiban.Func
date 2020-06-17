using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shouldly;

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
        public void ChainDoesNotEnumerateSourceFurtherThanItNeedsTo()
        {
            var yieldCount = 0;

            var sequence = new ObservableSequence(onYield: _ => { yieldCount++; }, count: 10);
            var chain = new Chain<int>(sequence);

            foreach (var _ in chain.Take(5)) { }

            Assert.That(yieldCount, Is.EqualTo(5));
        }

        [Test]
        public void EnumeratingChainMultipleTimesYieldsEachItemOnlyOnce()
        {
            var yieldedItems = new List<int>();

            var sequence = new ObservableSequence(onYield: i => yieldedItems.Add(i));
            var chain = new Chain<int>(sequence);

            foreach (var _ in chain) { }

            var yieldedItemsCopy = yieldedItems.ToList();

            foreach (var _ in chain) { }

            Assert.That(yieldedItemsCopy, Is.EquivalentTo(yieldedItems));
        }

        [Test]
        public void EnumeratingChainYieldsSameItemsAsOriginalSequence()
        {
            var count = Random.Next(100);
            var sequence = Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList();

            var chain = new Chain<Guid>(sequence);

            var zipped = sequence.Zip(chain, (x, y) => (x: x, y: y));

            var unequalItems = zipped.Where(t => t.x != t.y);

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
            var chain = new Chain<int>(new[] { element });

            Assert.That(chain.Head, Is.EqualTo(element));
        }

        [Test]
        public void DeconstructionYieldsHeadAsFirstElement()
        {
            var element = Random.Next();
            var chain = new Chain<int>(new[] { element });
            var (first, _) = chain;

            first.ShouldBe(element);
        }

        [Test]
        public void DeconstructionYieldsTailAsSecondElement()
        {
            var element = Random.Next();
            var chain = new Chain<int>(new[] { element });
            var (_, second) = chain;

            second.ShouldBe(chain.Tail);
        }

        [Test]
        public void ManuallyWalkingListToRandomPointYieldsCorrectResultAsHead()
        {
            var count = Random.Next(20);
            var items = RandomNumbers().Take(count).ToList();

            var chain = new Chain<int>(items);

            var index = Random.Next(count - 1);

            for (var i = 0; i < index && chain.IsEmpty == false; i++)
            {
                chain = chain.Tail;
            }

            chain.Head.ShouldBe(items[index]);
        }

        [Test]
        public void CallingHeadReturnsFirstItemInOriginalSequence()
        {
            var items = RandomNumbers().Take(2).ToList();

            var chain = new Chain<int>(items);

            Assert.That(chain.Head, Is.EqualTo(items.First()));
        }

        [Test]
        public void CallingTailThenHeadReturnsSecondItemInOriginalSequence()
        {
            var items = RandomNumbers().Take(2).ToList();

            var chain = new Chain<int>(items);

            Assert.That(chain.Tail.Head, Is.EqualTo(items.Skip(1).First()));
        }

        [Test]
        public void CallingTailThenTailThenHeadReturnsThirdItemInOriginalSequence()
        {
            var items = RandomNumbers().Take(3).ToList();

            var chain = new Chain<int>(items);

            Assert.That(chain.Tail.Tail.Head, Is.EqualTo(items.Skip(2).First()));
        }

        private static readonly Random Random = new Random();

        private static IEnumerable<int> RandomNumbers(int maxNumber = 100)
        {
            while (true)
            {
                yield return Random.Next(maxNumber);
            }
        }

        private sealed class ObservableSequence : IEnumerable<int>
        {
            private readonly Action _onGetEnumerator;
            private readonly Action<int> _onYield;
            private readonly int _count;

            public ObservableSequence(
                Action onGetEnumerator = null,
                Action<int> onYield = null,
                int count = -1)
            {
                _onGetEnumerator = onGetEnumerator;
                _onYield = onYield;
                _count = count >= 0 ? count : Random.Next(maxValue: 100);
            }

            public IEnumerator<int> GetEnumerator()
            {
                _onGetEnumerator?.Invoke();
                var count = _count;

                while (count-- > 0)
                {
                    var item = Random.Next();
                    _onYield?.Invoke(item);
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}