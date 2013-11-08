% Building MSBuildTasks on Linux

> *Note:* The easiest way to build and install MSBuildTasks is with [NuGet](http://www.nuget.org/).
> However, if you don't want to spend time getting NuGet working on your system, MSBuildTasks
> can be built without NuGet, using these instructions.

## Overview ##

MSBuildTasks depends on two other projects:

 * [DotNetZip][1]
 * [ILMerge][2]

ILMerge doesn't run on Linux, but [ILRepack][3] can be used as a drop-in replacement.

[1]: http://dotnetzip.codeplex.com/
[2]: http://research.microsoft.com/en-us/people/mbarnett/ilmerge.aspx
[3]: http://github.com/gluck/il-repack

## Procedure ##

Make a directory to hold downloaded files:

	mkdir nuget
	cd nuget

Fetch the NuGet package files:

	wget -O dotnetzip.reduced.nupkg http://packages.nuget.org/api/v1/package/DotNetZip.Reduced
	wget -O ilrepack.nupkg http://packages.nuget.org/api/v1/package/ILRepack

Extract the needed binaries:

	unzip -j dotnetzip.reduced.nupkg lib/\*
	unzip -j ilrepack.nupkg tools/ILRepack.exe

Create a wrapper for ILRepack:

	cat >ILMerge <<-EOF
		#!/bin/sh
		exec mono "$PWD/ILRepack.exe" "\$@"
	EOF
	chmod a+x ILMerge

Put the files where the build can find them:

	mkdir -p ../Source/packages/DotNetZip.Reduced.1.9.1.8/lib/net20/
	ln Ionic.Zip.Reduced.dll ../Source/packages/DotNetZip.Reduced.1.9.1.8/lib/net20/
	mkdir -p ../Source/packages/ilmerge.2.12.0803/
	ln ILMerge ../Source/packages/ilmerge.2.12.0803/

Run the build:

	xbuild Master.proj

## Other platforms ##

This procedure will probably work on other Unix-based systems (BSD, OS X)
but hasn't been tested on them.
