@echo off
::  生成的c++文件目录 
set base_h=D:\SVN\ET\Example\Server\Example.Model\Proto
:: proto 协议目录
set base_proto=D:\SVN\ET\Example\Proto
:: target path
set targetProtoPath=D:\SVN\Release\Protos
copy  %base_h%\*.h  %targetProtoPath%
del  %base_h%\*.h
copy  %base_proto%\*.proto  %targetProtoPath%
:: svn commit 
cd /d %targetProtoPath%
del Inner*
svn up
svn add * --force
svn ci -m "update proto genrate"
pause