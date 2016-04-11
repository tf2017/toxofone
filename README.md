# Toxofone

Toxofone is a simple audio/video TOX-client based on win32 native libtox.dll (see [https://github.com/tf2017/libtox-vs2015](https://github.com/tf2017/libtox-vs2015))

Application design is made as simple as possible. The number of entries in the phone book is limited to 10.

Source code is partially based on some components of full-featured .net client Toxy (see [https://github.com/Impyy/Toxy](https://github.com/Impyy/Toxy)).

Also Toxofone uses SharpTox ([https://github.com/Impyy/SharpTox](https://github.com/Impyy/SharpTox)) as managed wrapper over libtox.dll. In addition, the following libraries are used: NAudio ([https://github.com/naudio/NAudio](https://github.com/naudio/NAudio)), AForge.NET ([https://github.com/andrewkirillov/AForge.NET](https://github.com/andrewkirillov/AForge.NET)), Svg ([http://svg.codeplex.com/](http://svg.codeplex.com/)).


# Portable application

Toxofone is portable application which can be run on any Windows 7/8/10 32/64 bits OS with .NET 4.5 or 4.6 installed. No need to install Toxofone, just copy it into any directory and run.


# Portrait/landscape screen orientation

To change screen orientation manually, use key combination Ctrl+Right, Ctrl+Down, Ctrl+Left which turns application screen into LandscapeLeft, Portrait or LandscapeRight state. May be useful on notebooks with screen of small height. 


# Toxofone parameters

config.ini file contains Toxofone parameters. Some of these parameters must be edited manually before starting Toxofone.

Parameters editable manually:

1) __autoconnect.name = ToxUserName__ - when Toxofone receives call from user ToxUserName, user will be "autoconnected", i.e. it will not be the ringtone and this call will be accepted automatically without user interaction


----------


