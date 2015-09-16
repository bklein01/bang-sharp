# Bang# #
This project aims to create an open-source computer version of the popular card game [Bang!](http://en.wikipedia.org/wiki/Bang!). The game is written entirely in **C#**.

This project was inspired by (and actually based on) project [KBang](http://code.google.com/p/kbang/) which was written by [MacJariel](http://code.google.com/u/108640194566617880925/) in C++ for Qt4 libraries.

## Target Features ##
**Alpha version:**
  * online playing
  * a console-based [Test Client](BangSharpTestClient.md) with command history and autocompletion
  * useful game management using [sessions](Session.md)
  * games can optionally be watched by [spectators](Spectator.md)
  * optional [password protection](PasswordProtection.md) of sessions and players
  * optional [AI players](AI.md)
  * Dodge City expansion support
  * [official scoring system](http://www.bang.cz/en/rules-and-faq/special-rules/64-official-tournament-scoring-system.html)

**Beta Version:**
  * a [GUI client](BangSharpClient.md) for Bang#
  * High Noon and A Fistful of Cards expansions support `[`possibly also Wild West Show support`]`

**Other:**
  * a simple [API](API.md) for creating custom clients (only using [.NET Framework](http://msdn.microsoft.com/netframework/)/[Mono](http://www.mono-project.com/))

## History ##
I've actually started this project about a year before creating this project hosting page. I almost managed to finish the server part but then I stopped working on the project for a long time and when I finally decided to resume it I figured out that I had accidentally deleted all of my Visual Studio projects :-(

There wasn't actually nothing really of value, except of Bang# (that time I called it BANG.NET), but I really missed it - I have dedicated so much time to this project... I wanted to let it be but eventually I realised that I could still be able to build the project from scratch and even better than before.

I could still remember most of the principles and problem solutions I had used in the original project but it wasn't easy though. Many times I had to rediscover the same solutions, reconsider options and again making the same decision... but it was worth it! _The Bang# project is back alive!_

## Updates ##
**18.8.2011:** About two weeks ago, I discovered that the built-in .NET Remoting TCP channels cannot be used for two-way communication behind firewalls (see [issue 3](https://code.google.com/p/bang-sharp/issues/detail?id=3)). Remoting  is a very useful way of communication between applications and Bang# depends on it. I wanted to preserve existing structure of the code so I decided to try to create my own TCP channels that would work both ways. I checked out the [source code of Mono](https://github.com/mono/mono) and started to dig through the System.Runtime.Remoting.Channels.Tcp namespace. It took me some time to grasp all the internal remoting stuff but eventually I managed to code a perfectly working two-way TCP channels for .NET Remoting! I've also found and fixed several minor bugs and I will probably find many other bugs later but all of the alpha version target features have been already achieved. Now I'll have to start working on the GUI client and finish it as soon as possible.

**6.8.2011:** OK, the server is now almost fully functional and I have created also a console-based Test Client (it even has command autocompletion and history). The AI is working (I still plan some changes, but it's quite good already), sessions are being saved on the disk and I've even added the official scoring system! Oh - and the code is now in the repository, in all its beauty! Getting SVN to work as I want was a bit more challenging than I'd thought but eventually I made it.

**15.7.2011:** I've almost finished the server. All the main & Dodge City cards and characters are already implemented! I still need to solve few problems and implement the AI but then I should be ready to commit the code.

**2.7.2011:** I've already implemented most of the Bang# Server, but I still have a lot of work to do. I'll upload the code to the repository as soon as I finish at least the server part.

**30.7.2011:** I created this project hosting page.