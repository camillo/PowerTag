PowerTag is a powershell SnapIn, that provides CmdLets to query and manipulate tags from media files (.mp3, .ogg, .avi ect...).

Install

Enter the directory, where you stored the binaries and type '.<path>\InstallUtil.exe PowerTag.dll'. 
On my system InstallUtil is located at 'c:\Windows\Microsoft.NET\Framework\v2.0.50727\'

If your policy does not allow unsigned scriptes, you need to sign the files 'PowerTagFormats.ps1.xml' and 'PowerTagTypes.ps1.xml'. 
See 'get-help about_signing' for more infos.

Use 'Add-PSSnapin PowerTag' to insert the installed snapin into your powershell session. 

To get a list of all provided CmdLets, use 'Get-Command -PsSnapin PowerTag'.

Use 'Get-Help <CmdLet>' to get help.

