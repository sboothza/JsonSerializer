# JsonSerializer

## Description

This package contains a JsonSerializer implementation.

It's not very robust, and was meant to just test a theory - that serialization isn't nearly as hard as they make it out to be.   

**Conclusion** - it isn't.

Built with **.NET Standard 2.1** - no other dependencies.

The tests are **.NET 6.0**, referencing nunit, newtonsoft

It exposes a signature very similar to the Newtonsoft.Json and System.Text.Json libraries.

It should be possible to drop in with minimal changes.

Version 1.0 - Not quite complete or fully tested yet.

Preliminary performance testing initially indicated much better performance, but now it seems to be on par with the System.Text.Json

***

## Testing

Test project is provided - nunit

***

## Disclaimer

Go wild - use it, plagiarise, whatever.
No warranties of any kind are provided, including fitness for a particular purpose, blah, blah, blah.
