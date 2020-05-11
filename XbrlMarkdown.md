Import the Xbrl namespace and components by placing the following code at the top of your code document:
~~~
using Xbrl;
~~~

### Opening an XBRL Instance Document
An Xbrl document can be opened through a byte Stream.  This stream can be sourced from a locally saved file or through the "SecuritiesExchangeCommission.Edgar" NuGet solution package.  I've used the named NuGet package here:
~~~
//Search for a 10-K XBRL document and then download to a Stream
EdgarSearch es = await EdgarSearch.CreateAsync("INTC", "10-K");
EdgarSearchResult esr = es.GetFirstResultOfFilingType("10-K");
Stream s = await esr.DownloadXbrlDocumentAsync();

//Read the XBRL document
XbrlInstanceDocument doc = XbrlInstanceDocument.Create(s);
~~~

### Listing all facts
~~~
//List all facts
foreach (XbrlFact fact in doc.Facts)
{
	Console.WriteLine(fact.Label + ": " + fact.Value.ToString("#,##0.00");
}
~~~

### Listing all Context References
Each Context Reference contains five properties:

`Id` - The ID of the Context Ref. This is used by Facts to reference their time period.  
`TimeType` - Specifies the time type for this context reference (either a period with a start or end date, or a "snapshop" on a specific day)  
`InstantDate` - If the time type is instant ("snapshot"), this marks the date of the snapshot.  
`StartDate` - If the time type is period (a period of time), this marks the period begin date.  
`EndDate` - If the time type is period (a period of time), this marks the period end date.

Example of listing all properties of each context reference in an Instance Document:
~~~
//List all context references
foreach (XbrlContext context in doc.Contexts)
{
    Console.WriteLine("ID: " + context.Id);
    Console.WriteLine("Time Type: " + context.TimeType.ToString());
    if (context.TimeType == XbrlTimeType.Instant)
    {
        Console.WriteLine("Instant Date: " + context.InstantDate.ToShortDateString());
    }
    else if (context.TimeType == XbrlTimeType.Period)
    {
        Console.WriteLine("Start Date: " + context.StartDate.ToShortDateString());
        Console.WriteLine("End Date: " + context.EndDate.ToShortDateString());
    }
}
~~~

### Referencing a Fact's Context Reference
The `XbrlInstanceDocument` class provides the `GetContextById` method to search through the array of Context References and return the match.  
You can pass the `ContextId` string to this method to return the `XbrlContext` object.
For example:
~~~
//Get a fact's Context Reference
XbrlContext context = doc.GetContextById(doc.Facts[0].ContextId);
~~~

### Using the XBRL Helper
Many companies will include multiple facts of the same type in their document, each for a different fiscal period. For example, including the annual revenue of 2014, 2015, and 2016, all with them same label name. This package also provides a helper class to assist with extracting of *current* facts, related directly to this period.  
Import the helper class by placing this at the top of your code file:
~~~
using Xbrl.Helpers;
~~~
This will add an extension method to an `XbrlInstanceDocument` object called `GetValueFromPriorities`. This method will look for facts that are only relevent to the documents current time period. You can provide multiple labels (separated by comma) to search for and the method will search for a label with the first label, move onto the next if it was unable to find one, and so forth.  
For example, if a company either labeled their Current Assets from their balance sheet as "AssetsCurrent" or "CurrentAssets", we would use the following code to return this fact.
~~~
XbrlFact current_assets = doc.GetValueFromPriorities("CurrentAssets,AssetsCurrent");
~~~

### Converting an Instance Document to a Common Financial Statement
Companies use different terms and keywords to refer to common items on their financial statements. The XBRL package also provides a way to convert an XBRL Instance Document into a "common" financial statement with data that is commonly found in every set of financial statemnets like Revenue, Net Income, Assets, Equity, etc.  
Place this line at the top of you code file:
~~~
using Xbrl.FinancialStatement;
~~~
Importing the namespace with the code below adds an extension method to the `XbrlInstanceDocument` class. Below is an example of converting an `XbrlInstanceDocument` into a Financial Statement.
~~~
FinancialStatement fs = doc.CreateFinancialStatement();
~~~
