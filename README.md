# PredicateBuilder
PredicateBuilder provides a concise means of building complex dynamic queries in LInQ.

This implementation was adapted from a [blog post](https://petemontgomery.wordpress.com/2011/02/10/a-universal-predicatebuilder/) by Pete Montgomery.

## Use Case
Given an IOT logging data set containing a columns DeviceName, DayOfWeek, and HourOfDay,
Select all rows where the device named "Toaster" logged a message during any of an arbitrary set of (day, hour) tuples.

## Example

    // Create an expression based on the device name
    var deviceNameExpression = PredicateBuilder.Create<IotLogEntry>(p => p.DeviceName == "Toaster");

    // Create a false expression to initialize the dynamic OR expression
    var orExpression = PredicateBuilder.False<IotLogEntry>();

    // Add an OR predicate to the expression for each item in the arbitrary set of (day, hour) tuples (queryObjects)
    foreach (var queryObject in queryObjects)
    {
        orExpression = orExpression.Or(p => p.DayOfWeek == queryObject.DayOfWeek && p.HourOfDay == queryObject.HourOfDay);
    }

    // Combine the two expressions using AND and materialize the query
    var result = dbContext.IotLogEntries.Where(deviceNameExpression.And(orExpression)).ToList();
