namespace MoreLinq.Test.Pull
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MoreLinq.Pull;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Enumerable = MoreLinq.Pull.Enumerable;

    [TestFixture]
    public class GroupingTest
    {
        private static Tuple<TFirst, TSecond> Tuple<TFirst, TSecond>(TFirst a, TSecond b)
        {
            return new Tuple<TFirst, TSecond>(a, b);
        }
        
        #region Zip

        #region Default strategy (same tests as for ImbalancedZipStrategy.Truncate)
        [Test]
        public void BothSequencesDisposedWithUnequalLengths()
        {
            var longer = DisposeTestingSequence.Of(1, 2, 3);
            var shorter = DisposeTestingSequence.Of(1, 2);

            longer.Zip(shorter, (x, y) => x + y).Consume();
            longer.AssertDisposed();
            shorter.AssertDisposed();

            // Just in case it works one way but not the other...
            shorter.Zip(longer, (x, y) => x + y).Consume();
            longer.AssertDisposed();
            shorter.AssertDisposed();
        }

        [Test]
        public void ZipWithEqualLengthSequences()
        {
            var zipped = Enumerable.Zip(new[] {1, 2, 3}, new[] {4, 5, 6}, (x, y) => Tuple(x, y));
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5), Tuple(3, 6));
        }

        [Test]
        public void ZipWithFirstSequenceShorterThanSecond()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2 }, new[] { 4, 5, 6 }, (x, y) => Tuple(x, y));
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5));
        }

        [Test]
        public void ZipWithFirstSequnceLongerThanSecond()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2, 3 }, new[] { 4, 5 }, (x, y) => Tuple(x, y));
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5));
        }
        #endregion

        #region Handling of ImbalancedZipStrategy.Truncate
        [Test]
        public void BothSequencesDisposedWithUnequalLengthsTruncateStrategy()
        {
            var longer = DisposeTestingSequence.Of(1, 2, 3);
            var shorter = DisposeTestingSequence.Of(1, 2);

            longer.Zip(shorter, (x, y) => x + y, ImbalancedZipStrategy.Truncate).Consume();
            longer.AssertDisposed();
            shorter.AssertDisposed();

            // Just in case it works one way but not the other...
            shorter.Zip(longer, (x, y) => x + y, ImbalancedZipStrategy.Truncate).Consume();
            longer.AssertDisposed();
            shorter.AssertDisposed();
        }

        [Test]
        public void ZipWithEqualLengthSequencesTruncateStrategy()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2, 3 }, new[] { 4, 5, 6 },
                (x, y) => Tuple(x, y), ImbalancedZipStrategy.Truncate);
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5), Tuple(3, 6));
        }

        [Test]
        public void ZipWithFirstSequenceShorterThanSecondTruncateStrategy()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2 }, new[] { 4, 5, 6 },
                (x, y) => Tuple(x, y), ImbalancedZipStrategy.Truncate);
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5));
        }

        [Test]
        public void ZipWithFirstSequnceLongerThanSecondTruncateStrategy()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2, 3 }, new[] { 4, 5 },
                (x, y) => Tuple(x, y), ImbalancedZipStrategy.Truncate);
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5));
        }
        #endregion

        #region Handling of ImbalancedZipStrategy.Pad
        [Test]
        public void BothSequencesDisposedWithUnequalLengthsPadStrategy()
        {
            var longer = DisposeTestingSequence.Of(1, 2, 3);
            var shorter = DisposeTestingSequence.Of(1, 2);

            longer.Zip(shorter, (x, y) => x + y, ImbalancedZipStrategy.Pad).Consume();
            longer.AssertDisposed();
            shorter.AssertDisposed();

            // Just in case it works one way but not the other...
            shorter.Zip(longer, (x, y) => x + y, ImbalancedZipStrategy.Pad).Consume();
            longer.AssertDisposed();
            shorter.AssertDisposed();
        }

        [Test]
        public void ZipWithEqualLengthSequencesPadStrategy()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2, 3 }, new[] { 4, 5, 6 },
                (x, y) => Tuple(x, y), ImbalancedZipStrategy.Pad);
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5), Tuple(3, 6));
        }

        [Test]
        public void ZipWithFirstSequenceShorterThanSecondPadStrategy()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2 }, new[] { 4, 5, 6 },
                (x, y) => Tuple(x, y), ImbalancedZipStrategy.Pad);
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5), Tuple(0, 6));
        }

        [Test]
        public void ZipWithFirstSequnceLongerThanSecondPadStrategy()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2, 3 }, new[] { 4, 5 },
                (x, y) => Tuple(x, y), ImbalancedZipStrategy.Pad);
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5), Tuple(3, 0));
        }
        #endregion

        #region Handling of ImbalancedZipStrategy.Fail
        [Test]
        public void BothSequencesDisposedWithUnequalLengthsFailStrategy()
        {
            var longer = DisposeTestingSequence.Of(1, 2, 3);
            var shorter = DisposeTestingSequence.Of(1, 2);

            // Yes, this will throw... but then we should still have disposed both sequences
            try
            {
                longer.Zip(shorter, (x, y) => x + y, ImbalancedZipStrategy.Fail).Consume();
            }
            catch (InvalidOperationException)
            {
                // Expected
            }
            longer.AssertDisposed();
            shorter.AssertDisposed();

            // Just in case it works one way but not the other...
            try
            {
                shorter.Zip(longer, (x, y) => x + y, ImbalancedZipStrategy.Fail).Consume();
            }
            catch (InvalidOperationException)
            {
                // Expected
            }
            longer.AssertDisposed();
            shorter.AssertDisposed();
        }

        [Test]
        public void ZipWithEqualLengthSequencesFailStrategy()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2, 3 }, new[] { 4, 5, 6 },
                (x, y) => Tuple(x, y), ImbalancedZipStrategy.Fail);
            Assert.That(zipped, Is.Not.Null);
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5), Tuple(3, 6));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ZipWithFirstSequenceShorterThanSecondFailStrategy()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2 }, new[] { 4, 5, 6 },
                (x, y) => Tuple(x, y), ImbalancedZipStrategy.Fail);
            Assert.That(zipped, Is.Not.Null);
            zipped.Consume();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ZipWithFirstSequnceLongerThanSecondFailStrategy()
        {
            var zipped = Enumerable.Zip(new[] { 1, 2, 3 }, new[] { 4, 5 },
                (x, y) => Tuple(x, y), ImbalancedZipStrategy.Fail);
            Assert.That(zipped, Is.Not.Null);
            zipped.Consume();
            zipped.AssertSequenceEqual(Tuple(1, 4), Tuple(2, 5));
        }
        #endregion

        #region Invalid arguments
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ZipWithNullFirstSequence()
        {
            Enumerable.Zip(null, new[] { 4, 5, 6 }, BreakingFunc.Of<int, int, int>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ZipWithNullSecondSequence()
        {
            Enumerable.Zip(new[] { 1, 2, 3 }, null, BreakingFunc.Of<int, int, int>());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ZipWithNullResultSelector()
        {
            Enumerable.Zip<int, int, int>(new[] { 1, 2, 3 }, new[] { 4, 5, 6 }, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ZipWithInvalidStrategy()
        {
            Enumerable.Zip(new[] { 1, 2, 3 }, new[] { 4, 5, 6 },
                (x, y) => Tuple(x, y), (ImbalancedZipStrategy)10);
        }
        #endregion

        [Test]
        public void ZipIsLazy()
        {
            Enumerable.Zip<int, int, int>(
                new BreakingSequence<int>(), 
                new BreakingSequence<int>(), 
                delegate { throw new NotImplementedException(); });
        }
        
        #endregion

        #region Batch

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BatchNullSequence()
        {
            Enumerable.Batch<object>(null, 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BatchZeroSize()
        {
            Enumerable.Batch(new object[0], 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void BatchNegativeSize()
        {
            Enumerable.Batch(new object[0], -1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BatcWithhNullResultSelector()
        {
            Enumerable.Batch<object, object>(new object[0], 1, null);
        }

        [Test]
        public void BatchEvenlyDivisibleSequence()
        {
            var result = Enumerable.Batch(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 3);
            using (var reader = Read(result))
            {
                reader.Read().AssertSequenceEqual(1, 2, 3);
                reader.Read().AssertSequenceEqual(4, 5, 6);
                reader.Read().AssertSequenceEqual(7, 8, 9);
                reader.ReadEnd();
            }
        }

        [Test]
        public void BatchUnevenlyDivisbleSequence()
        {
            var result = Enumerable.Batch(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 4);
            using (var reader = Read(result))
            {
                reader.Read().AssertSequenceEqual(1, 2, 3, 4);
                reader.Read().AssertSequenceEqual(5, 6, 7, 8);
                reader.Read().AssertSequenceEqual(9);
                reader.ReadEnd();
            }
        }

        [Test]
        public void BatchSequenceTransformingResult()
        {
            var result = Enumerable.Batch(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, 4, batch => batch.Sum());
            result.AssertSequenceEqual(10, 26, 9);
        }

        [Test]
        public void BatchSequenceYieldsBatches()
        {
            var result = Enumerable.Batch(new[] { 1, 2, 3 }, 2);
            using (var reader = Read(result))
            {
                Assert.That(reader.Read(), Is.Not.InstanceOfType(typeof(ICollection<int>)));
                Assert.That(reader.Read(), Is.Not.InstanceOfType(typeof(ICollection<int>)));
                reader.ReadEnd();
            }
        }

        [Test]
        public void BatchIsLazy()
        {
            Enumerable.Batch(new BreakingSequence<object>(), 1);
        }

        private static SequenceReader<T> Read<T>(IEnumerable<T> source)
        {
            return new SequenceReader<T>(source);
        }

        #endregion
    }
}
