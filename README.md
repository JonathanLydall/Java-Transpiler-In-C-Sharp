Java-Transpiler-In-C-Sharp
==========================

A "good enough" transpiler which will parse Java, and output it to JavaScript / TypeScript.

The goal is to be able to automatically convert Minecraft Block type source code from Java into TypeScript syntax automatically.

With the above in mind, I am aiming for now to make the parser merely good enough to achieve that. This means that if I don't need certain aspects of Java language implemented in the parser to acheive that, they won't be done.

Once it's working, I may work on it further to make it more complete.

Why I am doing it in C#?
========================

My job as a developer is with a mostly Microsoft shop and this allows me to use a powerful enough language I am familiar with as well as get practice in the language I am working in.

Current Status
==============

Tokenizer is complete and working.

Parse can handle "some" files, but not all yet. I have been having to make it more robust at function calls which have brackets around them and casting. As an example, my most recent work fixed the cast of parameters, but then I encountered a case where it's casting an object, then calling a function on it, like so ((Type)Object).doMethod(par1) so I will probably have to change my data structure to accomodate such a scenario.

Work on outputting to another langauge has not yet started.
