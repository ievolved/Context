# Introduction

Sometimes contextual data needs to flow from function to function.  Doing so would often clutter the arguments of said functions.  It would be nice to have a way to store such info between a start and end point, that is automatically available to all functions between.

This is called an 'Ambient Context'.  It is described [here](https://aabs.wordpress.com/2007/12/31/the-ambient-context-design-pattern-in-net/).  It is a relatively simple idea but my needs take it to the extreme.  I need to create sub-contexts and flow them through web sessions, OWIN contexts, WCF requests, MSMQ calls, and more.

I've created a variation of this implementation at a previous employer where it was used for millions of requests successfully.  There I did not complete the implementation for MVC or OWIN contexts.  In this 'clean' re-implentation I will complete the feature list.