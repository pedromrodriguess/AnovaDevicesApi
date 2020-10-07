using DevicesApi.Domain;
using System.Collections.Generic;
using System.Linq;

namespace DevicesApi.TestsAuxiliaryTools
{
    public static class ReadingsComparer
    {
        public static bool CompareReadingsLists(List<Reading> readingsList1, List<Reading> readingsList2)
        {
            if (readingsList1 == null || readingsList2 == null)
                return false;

            if (readingsList1.Count != readingsList2.Count)
                return false;

            for (int i = 0; i < readingsList1.Count; i++)
            {
                if (!CompareReadings(readingsList1.ElementAt(i), readingsList2.ElementAt(i)))
                    return false;
            }
            return true;
        }

        public static bool DoesReadingsListContainSpecificReading(List<Reading> readingList, Reading expectedReading)
        {
            if (readingList == null)
                return false;

            return readingList.Any(reading => CompareReadings(reading, expectedReading));
        }

        public static bool CompareReadings(Reading reading1, Reading reading2)
        {
            if ((reading1.Device_id != reading2.Device_id
                || reading1.Timestamp != reading2.Timestamp
                || !reading1.Reading_type.Equals(reading2.Reading_type))
                || reading1.Raw_value != reading2.Raw_value)
            {
                return false;
            }
            return true;
        }
        
    }
}
