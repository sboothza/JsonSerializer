# JsonSerializer

## Description

This package contains a JsonSerializer implementation.

It was meant to just test a theory - that serialization isn't nearly as hard as they make it out to be.

**Conclusion** - it isn't.

**However** - Deserialization is a bit of a bugger.

Built with **.NET 8.0** - no other dependencies.

The tests are **.NET 8.0**, referencing nunit, newtonsoft, system.text.json

It exposes a signature very similar to the Newtonsoft.Json and System.Text.Json libraries.

It should be possible to drop in with minimal changes.

Version 2.0 - Not quite complete or fully tested yet.

***

## Performance

Preliminary performance testing initially indicated much better performance, but now is only slightly better.  
Which is still pretty cool.

Looping 1000 times

#### Serialization

- MeasureCustomSerialize - 25.81ms
- MeasureNewtonsoftSerialize - 35.89ms
- MeasureMicrosoftSerialize - 30.74ms

#### Deserialization

- MeasureCustomDeserialize - 20.10ms
- MeasureNewtonsoftDeserialize - 24.96ms
- MeasureMicrosoftDeserialize - 22.94ms


***

## Testing

Test project is provided - nunit

***

## Disclaimer

Go wild - use it, plagiarise, whatever.
No warranties of any kind are provided, including fitness for a particular purpose, blah, blah, blah.
