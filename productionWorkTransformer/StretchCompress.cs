using System;
using System.Collections.Generic;
using System.Linq;

namespace productionWorkTransformer
{
    public class StretchCompress
    {
        /// <summary>
        /// Compresses time spans to a target duration while maintaining km and amount values.
        /// </summary>
        public static List<ProductionWorkItem> Compress(List<ProductionWorkItem> items, TimeSpan targetDuration)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            
            if (targetDuration <= TimeSpan.Zero)
                throw new ArgumentException("Target duration must be positive", nameof(targetDuration));

            var result = new List<ProductionWorkItem>();
            
            foreach (var item in items)
            {
                var newItem = item.Clone();
                var currentDuration = item.Duration;
                
                // Only compress if current duration is greater than target
                if (currentDuration > targetDuration)
                {
                    newItem.EndDate = newItem.StartDate + targetDuration;
                }
                
                result.Add(newItem);
            }
            
            return result;
        }

        /// <summary>
        /// Stretches time spans to a target duration while maintaining km and amount values.
        /// </summary>
        public static List<ProductionWorkItem> Stretch(List<ProductionWorkItem> items, TimeSpan targetDuration)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            
            if (targetDuration <= TimeSpan.Zero)
                throw new ArgumentException("Target duration must be positive", nameof(targetDuration));

            var result = new List<ProductionWorkItem>();
            
            foreach (var item in items)
            {
                var newItem = item.Clone();
                var currentDuration = item.Duration;
                
                // Only stretch if current duration is less than target
                if (currentDuration < targetDuration)
                {
                    newItem.EndDate = newItem.StartDate + targetDuration;
                }
                
                result.Add(newItem);
            }
            
            return result;
        }

        /// <summary>
        /// Validates that a ProductionWorkItem has valid date ranges.
        /// </summary>
        public static bool ValidateDateRange(ProductionWorkItem item)
        {
            if (item == null)
                return false;
            
            return item.EndDate > item.StartDate;
        }

        /// <summary>
        /// Validates a list of ProductionWorkItems.
        /// </summary>
        public static bool ValidateItems(List<ProductionWorkItem> items)
        {
            if (items == null || items.Count == 0)
                return false;
            
            return items.All(item => ValidateDateRange(item));
        }
    }
}
