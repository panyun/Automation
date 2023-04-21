set current_path=%~dp0
sc create PIPSystemServer binPath= %current_path%EL.PIPSystemServer.exe
sc config PIPSystemServer start= auto
sc start PIPSystemServer
exit