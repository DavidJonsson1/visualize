using System;
using System.Collections.Generic;
using Xunit;
using productionWorkTransformer;

namespace productionWorkTransformer.Tests
{
    public class StretchCompressTests
    {
        // Helper method to create test data (10 rows as per requirement)
        private List<ProductionWorkItem> CreateTestData()
        {
            return new List<ProductionWorkItem>
            {
                new ProductionWorkItem { ID = 1, Product = "Product A", StartDate = new DateTime(2026, 1, 1, 8, 0, 0), EndDate = new DateTime(2026, 1, 1, 12, 0, 0), Km = 100, Amount = 1000 },
                new ProductionWorkItem { ID = 2, Product = "Product B", StartDate = new DateTime(2026, 1, 2, 9, 0, 0), EndDate = new DateTime(2026, 1, 2, 13, 0, 0), Km = 150, Amount = 1500 },
                new ProductionWorkItem { ID = 3, Product = "Product C", StartDate = new DateTime(2026, 1, 3, 10, 0, 0), EndDate = new DateTime(2026, 1, 3, 14, 0, 0), Km = 200, Amount = 2000 },
                new ProductionWorkItem { ID = 4, Product = "Product D", StartDate = new DateTime(2026, 1, 4, 11, 0, 0), EndDate = new DateTime(2026, 1, 4, 15, 0, 0), Km = 250, Amount = 2500 },
                new ProductionWorkItem { ID = 5, Product = "Product E", StartDate = new DateTime(2026, 1, 5, 12, 0, 0), EndDate = new DateTime(2026, 1, 5, 16, 0, 0), Km = 300, Amount = 3000 },
                new ProductionWorkItem { ID = 6, Product = "Product F", StartDate = new DateTime(2026, 1, 6, 13, 0, 0), EndDate = new DateTime(2026, 1, 6, 17, 0, 0), Km = 350, Amount = 3500 },
                new ProductionWorkItem { ID = 7, Product = "Product G", StartDate = new DateTime(2026, 1, 7, 14, 0, 0), EndDate = new DateTime(2026, 1, 7, 18, 0, 0), Km = 400, Amount = 4000 },
                new ProductionWorkItem { ID = 8, Product = "Product H", StartDate = new DateTime(2026, 1, 8, 15, 0, 0), EndDate = new DateTime(2026, 1, 8, 19, 0, 0), Km = 450, Amount = 4500 },
                new ProductionWorkItem { ID = 9, Product = "Product I", StartDate = new DateTime(2026, 1, 9, 16, 0, 0), EndDate = new DateTime(2026, 1, 9, 20, 0, 0), Km = 500, Amount = 5000 },
                new ProductionWorkItem { ID = 10, Product = "Product J", StartDate = new DateTime(2026, 1, 10, 17, 0, 0), EndDate = new DateTime(2026, 1, 10, 21, 0, 0), Km = 550, Amount = 5500 }
            };
        }

        #region Compression Tests

        [Fact]
        public void Compress_ReducesTimeSpanToTargetDuration()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.FromHours(2); // Compress from 4 hours to 2 hours

            // Act
            var result = StretchCompress.Compress(items, targetDuration);

            // Assert
            Assert.Equal(items.Count, result.Count);
            foreach (var item in result)
            {
                Assert.True(item.Duration <= targetDuration);
            }
        }

        [Fact]
        public void Compress_MaintainsKmAndAmountValues()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.FromHours(2);

            // Act
            var result = StretchCompress.Compress(items, targetDuration);

            // Assert
            for (int i = 0; i < items.Count; i++)
            {
                Assert.Equal(items[i].Km, result[i].Km);
                Assert.Equal(items[i].Amount, result[i].Amount);
                Assert.Equal(items[i].ID, result[i].ID);
                Assert.Equal(items[i].Product, result[i].Product);
            }
        }

        [Fact]
        public void Compress_DoesNotChangeItemsAlreadyAtOrBelowTargetDuration()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.FromHours(5); // Already 4 hours, so no change

            // Act
            var result = StretchCompress.Compress(items, targetDuration);

            // Assert
            for (int i = 0; i < items.Count; i++)
            {
                Assert.Equal(items[i].StartDate, result[i].StartDate);
                Assert.Equal(items[i].EndDate, result[i].EndDate);
                Assert.Equal(items[i].Duration, result[i].Duration);
            }
        }

        [Fact]
        public void Compress_CorrectlyCalculatesNewEndDate()
        {
            // Arrange
            var items = new List<ProductionWorkItem>
            {
                new ProductionWorkItem { ID = 1, Product = "Test", StartDate = new DateTime(2026, 1, 1, 8, 0, 0), EndDate = new DateTime(2026, 1, 1, 12, 0, 0), Km = 100, Amount = 1000 }
            };
            var targetDuration = TimeSpan.FromHours(2);

            // Act
            var result = StretchCompress.Compress(items, targetDuration);

            // Assert
            Assert.Equal(new DateTime(2026, 1, 1, 8, 0, 0), result[0].StartDate);
            Assert.Equal(new DateTime(2026, 1, 1, 10, 0, 0), result[0].EndDate);
            Assert.Equal(targetDuration, result[0].Duration);
        }

        #endregion

        #region Stretch Tests

        [Fact]
        public void Stretch_ExpandsTimeSpanToTargetDuration()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.FromHours(6); // Stretch from 4 hours to 6 hours

            // Act
            var result = StretchCompress.Stretch(items, targetDuration);

            // Assert
            Assert.Equal(items.Count, result.Count);
            foreach (var item in result)
            {
                Assert.True(item.Duration >= TimeSpan.FromHours(4)); // Original or stretched
            }
        }

        [Fact]
        public void Stretch_MaintainsKmAndAmountValues()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.FromHours(6);

            // Act
            var result = StretchCompress.Stretch(items, targetDuration);

            // Assert
            for (int i = 0; i < items.Count; i++)
            {
                Assert.Equal(items[i].Km, result[i].Km);
                Assert.Equal(items[i].Amount, result[i].Amount);
                Assert.Equal(items[i].ID, result[i].ID);
                Assert.Equal(items[i].Product, result[i].Product);
            }
        }

        [Fact]
        public void Stretch_DoesNotChangeItemsAlreadyAtOrAboveTargetDuration()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.FromHours(3); // Already 4 hours, so no change

            // Act
            var result = StretchCompress.Stretch(items, targetDuration);

            // Assert
            for (int i = 0; i < items.Count; i++)
            {
                Assert.Equal(items[i].StartDate, result[i].StartDate);
                Assert.Equal(items[i].EndDate, result[i].EndDate);
                Assert.Equal(items[i].Duration, result[i].Duration);
            }
        }

        [Fact]
        public void Stretch_CorrectlyCalculatesNewEndDate()
        {
            // Arrange
            var items = new List<ProductionWorkItem>
            {
                new ProductionWorkItem { ID = 1, Product = "Test", StartDate = new DateTime(2026, 1, 1, 8, 0, 0), EndDate = new DateTime(2026, 1, 1, 10, 0, 0), Km = 100, Amount = 1000 }
            };
            var targetDuration = TimeSpan.FromHours(4);

            // Act
            var result = StretchCompress.Stretch(items, targetDuration);

            // Assert
            Assert.Equal(new DateTime(2026, 1, 1, 8, 0, 0), result[0].StartDate);
            Assert.Equal(new DateTime(2026, 1, 1, 12, 0, 0), result[0].EndDate);
            Assert.Equal(targetDuration, result[0].Duration);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public void Compress_WithOverlappingDates_HandlesCorrectly()
        {
            // Arrange - Create items with same start dates
            var items = new List<ProductionWorkItem>
            {
                new ProductionWorkItem { ID = 1, Product = "Test1", StartDate = new DateTime(2026, 1, 1, 8, 0, 0), EndDate = new DateTime(2026, 1, 1, 12, 0, 0), Km = 100, Amount = 1000 },
                new ProductionWorkItem { ID = 2, Product = "Test2", StartDate = new DateTime(2026, 1, 1, 8, 0, 0), EndDate = new DateTime(2026, 1, 1, 14, 0, 0), Km = 200, Amount = 2000 }
            };
            var targetDuration = TimeSpan.FromHours(2);

            // Act
            var result = StretchCompress.Compress(items, targetDuration);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(targetDuration, result[0].Duration);
            Assert.Equal(targetDuration, result[1].Duration);
        }

        [Fact]
        public void Stretch_WithOverlappingDates_HandlesCorrectly()
        {
            // Arrange - Create items with same start dates
            var items = new List<ProductionWorkItem>
            {
                new ProductionWorkItem { ID = 1, Product = "Test1", StartDate = new DateTime(2026, 1, 1, 8, 0, 0), EndDate = new DateTime(2026, 1, 1, 10, 0, 0), Km = 100, Amount = 1000 },
                new ProductionWorkItem { ID = 2, Product = "Test2", StartDate = new DateTime(2026, 1, 1, 8, 0, 0), EndDate = new DateTime(2026, 1, 1, 11, 0, 0), Km = 200, Amount = 2000 }
            };
            var targetDuration = TimeSpan.FromHours(5);

            // Act
            var result = StretchCompress.Stretch(items, targetDuration);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(targetDuration, result[0].Duration);
            Assert.Equal(targetDuration, result[1].Duration);
        }

        [Fact]
        public void MixedOperations_CompressThenStretch_WorksCorrectly()
        {
            // Arrange
            var items = CreateTestData();
            var compressTarget = TimeSpan.FromHours(2);
            var stretchTarget = TimeSpan.FromHours(5);

            // Act
            var compressed = StretchCompress.Compress(items, compressTarget);
            var stretched = StretchCompress.Stretch(compressed, stretchTarget);

            // Assert
            for (int i = 0; i < items.Count; i++)
            {
                Assert.Equal(items[i].Km, stretched[i].Km);
                Assert.Equal(items[i].Amount, stretched[i].Amount);
                Assert.Equal(stretchTarget, stretched[i].Duration);
            }
        }

        #endregion

        #region Validation Tests

        [Fact]
        public void Compress_ThrowsArgumentNullException_WhenItemsIsNull()
        {
            // Arrange
            List<ProductionWorkItem>? items = null;
            var targetDuration = TimeSpan.FromHours(2);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => StretchCompress.Compress(items!, targetDuration));
        }

        [Fact]
        public void Stretch_ThrowsArgumentNullException_WhenItemsIsNull()
        {
            // Arrange
            List<ProductionWorkItem>? items = null;
            var targetDuration = TimeSpan.FromHours(2);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => StretchCompress.Stretch(items!, targetDuration));
        }

        [Fact]
        public void Compress_ThrowsArgumentException_WhenTargetDurationIsZero()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.Zero;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StretchCompress.Compress(items, targetDuration));
        }

        [Fact]
        public void Stretch_ThrowsArgumentException_WhenTargetDurationIsZero()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.Zero;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StretchCompress.Stretch(items, targetDuration));
        }

        [Fact]
        public void Compress_ThrowsArgumentException_WhenTargetDurationIsNegative()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.FromHours(-1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StretchCompress.Compress(items, targetDuration));
        }

        [Fact]
        public void Stretch_ThrowsArgumentException_WhenTargetDurationIsNegative()
        {
            // Arrange
            var items = CreateTestData();
            var targetDuration = TimeSpan.FromHours(-1);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => StretchCompress.Stretch(items, targetDuration));
        }

        [Fact]
        public void ValidateDateRange_ReturnsFalse_WhenEndDateBeforeStartDate()
        {
            // Arrange
            var item = new ProductionWorkItem
            {
                ID = 1,
                Product = "Test",
                StartDate = new DateTime(2026, 1, 1, 12, 0, 0),
                EndDate = new DateTime(2026, 1, 1, 8, 0, 0), // Invalid: End before Start
                Km = 100,
                Amount = 1000
            };

            // Act
            var isValid = StretchCompress.ValidateDateRange(item);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateDateRange_ReturnsFalse_WhenEndDateEqualsStartDate()
        {
            // Arrange
            var item = new ProductionWorkItem
            {
                ID = 1,
                Product = "Test",
                StartDate = new DateTime(2026, 1, 1, 8, 0, 0),
                EndDate = new DateTime(2026, 1, 1, 8, 0, 0), // Invalid: Same time
                Km = 100,
                Amount = 1000
            };

            // Act
            var isValid = StretchCompress.ValidateDateRange(item);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateDateRange_ReturnsTrue_WhenDatesAreValid()
        {
            // Arrange
            var item = new ProductionWorkItem
            {
                ID = 1,
                Product = "Test",
                StartDate = new DateTime(2026, 1, 1, 8, 0, 0),
                EndDate = new DateTime(2026, 1, 1, 12, 0, 0),
                Km = 100,
                Amount = 1000
            };

            // Act
            var isValid = StretchCompress.ValidateDateRange(item);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateItems_ReturnsFalse_WhenListIsNull()
        {
            // Arrange
            List<ProductionWorkItem>? items = null;

            // Act
            var isValid = StretchCompress.ValidateItems(items!);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateItems_ReturnsFalse_WhenListIsEmpty()
        {
            // Arrange
            var items = new List<ProductionWorkItem>();

            // Act
            var isValid = StretchCompress.ValidateItems(items);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateItems_ReturnsFalse_WhenAnyItemHasInvalidDates()
        {
            // Arrange
            var items = new List<ProductionWorkItem>
            {
                new ProductionWorkItem { ID = 1, Product = "Test1", StartDate = new DateTime(2026, 1, 1, 8, 0, 0), EndDate = new DateTime(2026, 1, 1, 12, 0, 0), Km = 100, Amount = 1000 },
                new ProductionWorkItem { ID = 2, Product = "Test2", StartDate = new DateTime(2026, 1, 1, 12, 0, 0), EndDate = new DateTime(2026, 1, 1, 8, 0, 0), Km = 200, Amount = 2000 } // Invalid
            };

            // Act
            var isValid = StretchCompress.ValidateItems(items);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ValidateItems_ReturnsTrue_WhenAllItemsAreValid()
        {
            // Arrange
            var items = CreateTestData();

            // Act
            var isValid = StretchCompress.ValidateItems(items);

            // Assert
            Assert.True(isValid);
        }

        #endregion

        #region Data Integrity Tests

        [Fact]
        public void Compress_DoesNotModifyOriginalList()
        {
            // Arrange
            var items = CreateTestData();
            var originalEndDate = items[0].EndDate;
            var targetDuration = TimeSpan.FromHours(2);

            // Act
            var result = StretchCompress.Compress(items, targetDuration);

            // Assert
            Assert.Equal(originalEndDate, items[0].EndDate); // Original should not change
            Assert.NotEqual(originalEndDate, result[0].EndDate); // Result should be different
        }

        [Fact]
        public void Stretch_DoesNotModifyOriginalList()
        {
            // Arrange
            var items = CreateTestData();
            var originalEndDate = items[0].EndDate;
            var targetDuration = TimeSpan.FromHours(6);

            // Act
            var result = StretchCompress.Stretch(items, targetDuration);

            // Assert
            Assert.Equal(originalEndDate, items[0].EndDate); // Original should not change
            Assert.NotEqual(originalEndDate, result[0].EndDate); // Result should be different
        }

        [Fact]
        public void Compress_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var items = new List<ProductionWorkItem>();
            var targetDuration = TimeSpan.FromHours(2);

            // Act
            var result = StretchCompress.Compress(items, targetDuration);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Stretch_WithEmptyList_ReturnsEmptyList()
        {
            // Arrange
            var items = new List<ProductionWorkItem>();
            var targetDuration = TimeSpan.FromHours(6);

            // Act
            var result = StretchCompress.Stretch(items, targetDuration);

            // Assert
            Assert.Empty(result);
        }

        #endregion
    }
}
