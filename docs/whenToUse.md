## When to use this
This is intended for use on flickering tests, where the reason for failure is an external 
dependency and the failure is transient, e.g:
 - HTTP request over the network
 - Database call that could deadlock, timeout etc...

Whenever a test includes real-world infrastructure, particularly when communicated with via the
internet, there is a risk of the test randomly failing so we want to try and run it again. 
This is the intended use case of the library.  

If you have a test that covers some flaky code, where sporadic failures are caused by a bug, 
this library should **not** be used to cover it up!