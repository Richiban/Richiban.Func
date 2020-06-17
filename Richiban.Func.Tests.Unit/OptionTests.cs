using System;
using NUnit.Framework;
using Shouldly;

namespace Richiban.Func.Tests.Unit
{
    [TestFixture]
    public sealed class OptionTests
    {
        [Test]
        public void HasValue_returns_false_for_None()
        {
            var none = Option<string>.None;

            none.IsSome.ShouldBeFalse();
        }

        [Test]
        public void HasValue_returns_true_for_Some()
        {
            var randomString = Guid.NewGuid().ToString();
            var some = Option<string>.Create(randomString);

            some.IsSome.ShouldBeTrue();
        }

        [Test]
        public void Match_on_Some_allows_access_to_enclosed_value()
        {
            var enclosedValue = Guid.NewGuid().ToString();
            var some = Option<string>.Create(enclosedValue);

            var actual = some.Match(() => null, s => s);

            Assert.That(actual, Is.EqualTo(enclosedValue));
        }

        [Test]
        public void Match_on_None_does_not_allow_access_to_any_value()
        {
            var none = Option<string>.None;

            var expected = Guid.NewGuid().ToString();

            var actual = none.Match(() => expected, s => s);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ToString_on_Some_returns_string_representation_of_enclosed_value()
        {
            var value = Guid.NewGuid();
            var expectedResult = value.ToString();
            var option = Option<Guid>.Create(value);

            var actual = option.ToString();

            Assert.That(actual, Is.EqualTo(expectedResult));
        }

        [Test]
        public void ToString_on_None_returns_string_representation_of_None()
        {
            var option = Option<string>.None;
            var expectedResult = "<none>";
            var actual = option.ToString();

            Assert.That(actual, Is.EqualTo(expectedResult));
        }

        [Test]
        public void Equals_returns_true_for_None()
        {
            var leftNone = Option<object>.None;
            var rightNone = Option<object>.None;

            Assert.That(leftNone.Equals(rightNone));
        }

        [Test]
        public void Equals_returns_false_for_None_with_different_type_argument()
        {
            var leftNone = Option<object>.None;
            var rightNone = Option<string>.None;

            Assert.False(leftNone.Equals(rightNone));
        }

        [Test]
        public void Equals_returns_true_for_None_when_compared_with_null()
        {
            var leftNone = Option<object>.None;
            var rightNone = (Option<object>)null;

            Assert.True(leftNone.Equals(rightNone));
        }

        [Test]
        public void Equals_returns_false_for_None_when_compared_with_different_type()
        {
            var none = Option<object>.None;
            var obj = new object();

            Assert.That(none, Is.Not.EqualTo(obj));
        }

        [Test]
        public void Equals_returns_true_for_Some_with_same_enclosed_values()
        {
            var enclosedValue = new object();
            var leftNone = Option<object>.Create(enclosedValue);
            var rightNone = Option<object>.Create(enclosedValue);

            Assert.That(leftNone.Equals(rightNone));
        }

        [Test]
        public void Equals_returns_false_for_Some_with_different_enclosed_values()
        {
            var leftNone = Option<object>.Create(new object());
            var rightNone = Option<object>.Create(new object());

            Assert.False(leftNone.Equals(rightNone));
        }

        [Test]
        public void Equals_returns_false_for_Some_with_different_type_argument()
        {
            var enclosedValue = Guid.NewGuid().ToString();
            var leftNone = Option<object>.Create(enclosedValue);
            var rightNone = Option<string>.Create(enclosedValue);

            Assert.False(leftNone.Equals(rightNone));
        }

        [Test]
        public void Equals_returns_false_for_Some_when_compared_with_null()
        {
            var leftNone = Option<object>.Create(new object());
            var rightNone = (Option<object>)null;

            Assert.False(leftNone.Equals(rightNone));
        }

        [Test]
        public void Equals_returns_false_for_Some_when_compared_with_different_type()
        {
            var none = Option<object>.Create(new object());
            var obj = new object();

            Assert.False(none.Equals(obj));
        }

        [Test]
        public void Value_returns_enclosed_value_for_Some()
        {
            var enclosedValue = new object();
            var option = Option<object>.Create(enclosedValue);

            Assert.That(option.Force(), Is.EqualTo(enclosedValue));
        }

        [Test]
        public void Value_throws_exception_for_None()
        {
            var option = Option<object>.None;
            object optionalValue;
            Assert.Throws<InvalidOperationException>(() => optionalValue = option.Force());
        }

        [Test]
        public void New_Option_is_None_for_null_value()
        {
            var nullValue = (object)null;
            var actual = new Option<object>(nullValue);

            Assert.That(actual, Is.EqualTo(Option<object>.None));
        }

        [Test]
        public void Cast_to_Option_is_Some_for_non_null_value()
        {
            var enclosedValue = new object();
            Option<object> actual = enclosedValue;

            Assert.That(actual.IsSome, Is.True);
        }

        [Test]
        public void Cast_to_Option_is_None_for_null_value()
        {
            var enclosedValue = (object)null;
            Option<object> actual = enclosedValue;

            Assert.That(actual.IsSome, Is.False);
        }

        [Test]
        public void Select_returns_new_option_with_mapped_value_when_called_on_Some()
        {
            var enclosedValue = Guid.NewGuid().ToString();
            var some = new Option<string>(enclosedValue);

            var expected = new Option<int>(enclosedValue.Length);
            var actual = some.Select(s => s.Length);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Select_returns_None_when_called_on_None()
        {
            var none = new Option<string>(null);

            var expected = Option<int>.None;
            var actual = none.Select(s => s.Length);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void SelectMany_on_None_and_None_returns_None()
        {
            var none = new Option<Option<string>>();

            var result =
                from value in none
                from innerValue in value
                select innerValue;

            Assert.That(result, Is.EqualTo(Option<string>.None));
        }

        [Test]
        public void SelectMany_on_Some_and_None_returns_None()
        {
            var enclosedValue = new NestedOptionals(new Option<string>());
            var option = new Option<NestedOptionals>(enclosedValue);

            var result =
                from value in option
                from optionalString in value.OptionString
                select optionalString;

            Assert.That(result, Is.EqualTo(Option<string>.None));
        }

        [Test]
        public void SelectMany_on_None_and_Some_returns_None()
        {
            var option = new Option<NestedOptionals>();

            var result =
                from nestedOptionals in option
                from optionalString in nestedOptionals.OptionString
                select optionalString;

            Assert.That(result, Is.EqualTo(Option<string>.None));
        }

        [Test]
        public void SelectMany_on_Some_and_Some_returns_Some()
        {
            var enclosedValue = Guid.NewGuid().ToString();
            var nestedOptional = new NestedOptionals(new Option<string>(enclosedValue));
            var option = new Option<NestedOptionals>(nestedOptional);

            var result =
                from value in option
                from optionalString in value.OptionString
                select optionalString;

            Assert.That(result, Is.EqualTo(Option<string>.Create(enclosedValue)));
        }

        public class NestedOptionals
        {
            public NestedOptionals(Option<string> optionString)
            {
                OptionString = optionString;
            }

            public Option<string> OptionString { get; private set; }
        }

        [Test]
        public void Where_returns_none_on_None()
        {
            var none = new Option<object>();
            var actual = none.Where(_ => true);

            Assert.That(actual, Is.EqualTo(Option<object>.None));
        }

        [Test]
        public void Where_returns_None_when_inner_value_does_not_meet_predicate()
        {
            var enclosedValue = 0;
            var option = new Option<int>(enclosedValue);
            var actual = option.Where(i => i > 0);

            Assert.That(actual, Is.EqualTo(Option<int>.None));
        }

        [Test]
        public void Where_returns_Some_when_inner_value_meets_predicate()
        {
            var enclosedValue = 0;
            var option = new Option<int>(enclosedValue);
            var actual = option.Where(i => i == 0);

            Assert.That(actual, Is.EqualTo(Option<int>.Create(enclosedValue)));
        }

        [Test]
        public void Iter_has_no_effect_when_called_on_None()
        {
            var i = 0;
            var option = Option<int>.None;

            option.Iter(value => i += value);

            Assert.That(i, Is.EqualTo(0));
        }

        [Test]
        public void Iter_has_side_effect_when_called_on_Some()
        {
            var i = 0;
            var option = Option<int>.Create(2);

            option.Iter(value => i += value);

            Assert.That(i, Is.EqualTo(2));
        }

        [Test]
        public void GetValueOrDefault_when_called_on_Some_returns_inner_value()
        {
            var option = Option<int>.Create(2);
            var expected = 2;
            var actual = option.GetValueOrDefault();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetValueOrDefault_when_called_on_None_returns_default_for_type()
        {
            var option = Option<int>.None;
            var expected = default(int);
            var actual = option.GetValueOrDefault();

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void GetValueOrDefault_when_called_on_None_returns_default_value_passed_into_method()
        {
            var option = Option<int>.None;
            var randomNumber = Random.Next();
            var expected = randomNumber;
            var actual = option.GetValueOrDefault(randomNumber);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private static readonly Random Random = new Random();
    }
}