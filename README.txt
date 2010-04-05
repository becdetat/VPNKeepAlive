VPNKeepAlive
Released into the public domain by Ben Scott / Belfry Images
http://blog.belfryimages.com.au
http://github.com/belfryimages/VPNKeepAlive

VPNKeepAlive keeps alive a dodgy VPN connection by periodically pinging a 
server or machine on the host side of the VPN. It also manages the connection
to reduce the number of clicks.

I found that if I left my VPN connection idle for a minute it would disconnect
me. I know that it is probably an issue with my router as using the same machine
at a different location with the same VPN connection it is almost 100% stable,
but I needed the connection _right then_ so rather than fix the router or buy a 
new one I just opened a console and ran 'ping -t servername'. That worked fine
but it was pinging far more than needed (although the traffic is insignificant)
and I needed to remember to set up the ping and leave the window open. Which is
why I spent Easter Sunday afternoon building a notify app that manages it for me.

The installer copies files to Program Files, creates a Start menu folder and adds 
a link to the executable to the Startup folder, and sets up an uninstaller, 
but as VPNKeepAlive.exe has no dependencies (apart from the .NET 3.5 framework)
you could just use the exe directly or copy it into the startup folder

VPNKeepAlive is written in C# using VS2008. The source is available at
http://github.com/belfryimages/VPNKeepAlive and is in the public domain. It uses
the system command 'rasdial' to connect and disconnect, and pings using the
built-in System.Net.NetworkInformation.Ping class. The installer is an NSIS script
(http://nsis.sourceforge.net) and requires the KillProc plugin 
(http://nsis.sourceforge.net/KillProc_plug-in).