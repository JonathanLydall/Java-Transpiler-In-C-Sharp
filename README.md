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

No detailed documentation on how to use it, yet.

But the Transpiler is pretty much at "good enough" for my needs. I also ended up making a GUI utility to help me configure which are the files I want to transpile and other details about transpiling particular files. I have included this tool into the repository as well now.
