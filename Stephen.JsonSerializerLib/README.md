# JsonSerializer

## Description

This package contains a JsonSerializer implementation.

It was meant to just test a theory - that serialization isn't nearly as hard as they make it out to be.

**Conclusion** - it isn't.

Built with **.NET 6.0** - no other dependencies.

The tests are **.NET 6.0**, referencing nunit, newtonsoft, system.text.json

It exposes a signature very similar to the Newtonsoft.Json and System.Text.Json libraries.

It should be possible to drop in with minimal changes.

Version 1.0 - Not quite complete or fully tested yet.

***

## Performance

Preliminary performance testing initially indicated much better performance, but now seems slightly less

Runs roughly `40%` of NewtonSoft.Json (`2.5x` faster)
Runs roughly `117%` of System.Text.Json (`1.1x` slower)

***

## Testing

Test project is provided - nunit

***

## Disclaimer

Go wild - use it, plagiarise, whatever.
No warranties of any kind are provided, including fitness for a particular purpose, blah, blah, blah.
