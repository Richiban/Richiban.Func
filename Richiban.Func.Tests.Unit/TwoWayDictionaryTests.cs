using System;
using NUnit.Framework;
using Shouldly;

namespace Richiban.Func.Tests.Unit
{
    [TestFixture]
    class TwoWayDictionaryTests
    {
        [Test]
        public void ReversingTwoWayDictionaryTwiceResultsInFirstDictionary()
        {
            var original = new TwoWayDictionary<string, int>();

            var doubleReversed = original.Reversed.Reversed;
            
            doubleReversed.ShouldBeSameAs(original);
        }

        [Test]
        public void ClearingATwoWayDictionaryShouldAlsoClearItsReverse()
        {
            var original = new TwoWayDictionary<string, int> { { "one", 1 } };
            
            original.Count.ShouldBe(1);
            original.Reversed.Count.ShouldBe(1);

            original.Clear();

            original.Count.ShouldBe(0);
            original.Reversed.Count.ShouldBe(0);
        }

        [Test]
        public void AddingASingleItemResultsInACountOfOne()
        {
            var left = Guid.NewGuid().ToString();
            var right = Guid.NewGuid().GetHashCode();
            var original = new TwoWayDictionary<string, int> { { left, right } };
            
            original.Count.ShouldBe(1);
        }

        [Test]
        public void ValueFromOriginalShouldBeUsableAsKeyInReversed()
        {
            var left = Guid.NewGuid().ToString();
            var right = Guid.NewGuid().GetHashCode();
            var original = new TwoWayDictionary<string, int> { { left, right } };

            original[left].ShouldBe(right);
            original.Reversed[right].ShouldBe(left);
        }
    }
}
