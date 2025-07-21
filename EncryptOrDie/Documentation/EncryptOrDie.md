# Encrypt Or Die Game

**Overview**
The user is trying to send a message to someone for rescue because someone is trying to kill them.
The user, before they can send the message, must encrypt the message and send it on a specific frequency.
The killer can guess the frequency that the user is using. When the killer guesses the frequency, 
the killer can decrypt the message and find the user. If the killer does not guess the user's frequency in
five tries, the user is safe and rescue is on the way.

**Learning Objectives**
I just read chapters 1-3 and I wanted to start simple. My goal for this project was to implement with
some C# concepts I was reading about such as:

- formatting numerical data
- using TryParse to parse data into specific data types
- string interpolation
- StringBuilder
- if/else statements
- loops

**Extra Concepts Implemented**
Having experience with Java and Kotlin, I wanted to also try to decouple my code into separate classes, even though
the chapters that I have read so far do not cover OOP concepts such as classes, inheritance, polymorphism, etc.
If I wanted to, I can try refactor this code to use OOP concepts later on, but for now, I just wanted to focus on
the main concepts as listed above. I feel that I can focus on said concepts in a later project that I find more
engaging and interesting. I also implemented AES encryption of the user's message using the 
`System.Security.Cryptography` namespace because I am currently working on an encryption project and I wanted to
learn how to implement encryption in C# ([EncryptedFileDrop](https://github.com/gabeamv/EncryptedFileDrop)). 
I would also like to implement RSA encryption in this project if I have the interest and the time to do so, 
since the same project I stated is going to be utilizing that.

Encryption was implemented with the [microsoft.learn.com](https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.cryptostream?view=net-9.0)

**Game Flow**
The user is prompted to enter their name. Afterwards, the user is prompted to enter a message which should contain
X, Y coordinates for their location (1-99). The user is prompted five times to enter a frequency (1-5), and each
time there is a check to see if the frequency matches the killer's randomly generated frequency. If the frequencies
match, the user loses. If the frequencies do not match, the user's message is encrypted, displayed, and the user
wins.

**Improvements To Be Made**
- separation of concerns for cleaner code
- use of a YAML/JSON configuration file to store game dialogue
- implement RSA encryption to simulate a more realistic encryption scenario by encrypting the AES key with RSA
- somehow make the frequency fighting more interesting and thoughtful instead of random number generation
