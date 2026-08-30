using System;
using System.Collections.Generic;
using System.Text;
using AlertEngine;
using AlertEngine.Json;

namespace AlertEngine.Tests
{
    public class PriceFileReaderTests
    {
        [Fact]
        public void Reads_Prices_In_Order()
        {
            string json = """
        {
          "currency": "TRY/MWh",
          "timezone": "Europe/Istanbul",
          "prices": [
            { "timestamp": "2026-08-15T17:00:00+03:00", "price": 2347.60 },
            { "timestamp": "2026-08-15T18:00:00+03:00", "price": 4200.00 }
          ]
        }
        """;

            var points = PriceFileReader.Read(json);

            Assert.Equal(2, points.Count);
            Assert.Equal(2347.60m, points[0].Price);
            Assert.Equal(4200.00m, points[1].Price);
        }

        [Fact]
        public void Preserves_Timestamp_With_Offset()
        {
            string json = """
        {
          "prices": [
            { "timestamp": "2026-08-15T18:00:00+03:00", "price": 4200.00 }
          ]
        }
        """;

            var points = PriceFileReader.Read(json);

            var expected = DateTimeOffset.Parse("2026-08-15T18:00:00+03:00");
            Assert.Equal(expected, points[0].Timestamp);
            Assert.Equal(TimeSpan.FromHours(3), points[0].Timestamp.Offset);
        }

        [Fact]
        public void Reads_Negative_Price_Correctly()
        {
            string json = """
        {
          "prices": [
            { "timestamp": "2026-08-13T14:00:00+03:00", "price": -50.00 }
          ]
        }
        """;

            var points = PriceFileReader.Read(json);

            Assert.Equal(-50.00m, points[0].Price);
        }

        [Fact]
        public void Throws_On_Invalid_Json_Structure()
        {
            string json = """
        { "currency": "TRY/MWh" }
        """;

            Assert.Throws<InvalidDataException>(() => PriceFileReader.Read(json));
        }
    }
}
