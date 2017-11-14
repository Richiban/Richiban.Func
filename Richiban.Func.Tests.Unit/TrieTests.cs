using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using static PowerAssert.PAssert;

namespace Richiban.Func.Tests.Unit
{
    [TestFixture]
    public sealed class TrieTests
    {
        private static readonly Random Random = new Random();

        [Test]
        public void GivenATrieThatContainsAValue_WhenIRetrieveThatKey_ThenTheValueIsReturned()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().GetHashCode();
            var trie = new Trie<int> { [key] = value };

            IsTrue(() => trie[key] == value);
        }

        [Test]
        public void GivenAnEmptyTrie_WhenISetAValueToAnEmptyKey_ThenThatValueIsSet()
        {
            var key = "";
            var value = Guid.NewGuid().GetHashCode();
            var trie = new Trie<int> { [key] = value };

            IsTrue(() => trie[key] == value);
        }

        [Test]
        public void GivenATrieThatContainsAValue_WhenIRemoveThatKeyThenRetrieveIt_ThenNoValueIsReturned()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().GetHashCode();
            var trie = new Trie<int> { [key] = value };

            trie.Remove(key);

            IsTrue(() => trie.TryGet(key).IsSome == false);
        }

        [Test]
        public void GivenAnEmptyTrie_WhenIRetrieveAKey_ThenNoneIsReturned()
        {
            var trie = new Trie<int>();
            var key = "hello";

            IsTrue(() => trie.TryGet(key).IsSome == false);
        }

        [Test]
        public void GivenATrieThatContainsAValue_WhenIEnumerateTheTrie_ThenAnSingletonSequenceIsReturned()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().GetHashCode();
            var trie = new Trie<int> { [key] = value };

            IsTrue(() => trie.SequenceEqual(new[] { value }));
        }

        [Test]
        public void
            GivenATrieThatContainsMultipleValues_WhenIEnumerateTheTrie_ThenTheCorrectSequenceIsReturned()
        {
            IEnumerable<(string, int)> GetSequence()
            {
                while (true)
                {
                    var key = Guid.NewGuid();
                    yield return (key.ToString(), key.GetHashCode());
                }
            }

            var items = new HashSet<(string, int)>(
                GetSequence().Take(Random.Next(maxValue: 30) + 20));

            var trie = new Trie<int>();

            foreach (var (key, value) in items)
            {
                trie[key] = value;
            }

            var originalItems = new HashSet<int>(items.Select(x => x.Item2));
            var trieItems = new HashSet<int>(trie);

            Assert.That(
                originalItems.SetEquals(trieItems), 
                $"{string.Join(", ", originalItems.Except(trieItems))} were not present in the Trie");
        }

        [Test]
        public void GivenAnEmptyTrie_WhenIEnumerateTheTrie_ThenAnEmptySequenceIsReturned()
        {
            var trie = new Trie<int>();

            IsTrue(() => trie.SequenceEqual(Enumerable.Empty<int>()));
        }
    }
}